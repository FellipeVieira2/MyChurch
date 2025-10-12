using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Mychurch.Common.WebClients.Asaas;
using MyChurch.Application.CashFlow.Commands.CreateCashFlowEntry;
using MyChurch.Application.CashFlow.Services;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;
using MyChurch.Domain.Enum;

namespace MyChurch.Application.Webhook.Commands
{
    public class ConfirmPaymentCommand : IRequest<bool>
    {
        public string PaymentId { get; set; } // ID do pagamento no Asaas
        public PaymentStatus Status { get; set; }
    }

    public class ConfirmPaymentCommandHandler : IRequestHandler<ConfirmPaymentCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ConfirmPaymentCommandHandler> _logger;
        private readonly IAsaasWebClient _asaasWebClient;
        private readonly IMediator _mediator;
        private readonly ICashFlowAutomationService _cashFlowAutomation; // 🔥 NOVO

        public ConfirmPaymentCommandHandler(
            IUnitOfWork unitOfWork,
            ILogger<ConfirmPaymentCommandHandler> logger,
            IAsaasWebClient asaasWebClient,
            IMediator mediator,
            ICashFlowAutomationService cashFlowAutomation) // 🔥 NOVO
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _asaasWebClient = asaasWebClient;
            _mediator = mediator;
            _cashFlowAutomation = cashFlowAutomation; // 🔥 NOVO
        }

        public async Task<bool> Handle(ConfirmPaymentCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var payment = await _unitOfWork.Payments.Query()
                    .Include(p => p.Subscription)
                        .ThenInclude(s => s.Plan)
                    .Include(p => p.Subscription)
                        .ThenInclude(s => s.Church)
                    .Include(p => p.Donation) // 🔥 NOVO
                    .FirstOrDefaultAsync(p => p.TransactionId == request.PaymentId, cancellationToken);

                if (payment == null)
                {
                    _logger.LogWarning($"Pagamento não encontrado para o ID Asaas: {request.PaymentId}");
                    return false;
                }

                var oldStatus = payment.PaymentStatus;
                payment.PaymentStatus = request.Status.ToString();
                payment.Date = DateTime.UtcNow;
                _unitOfWork.Payments.Update(payment);

                // 🔥 NOVA LÓGICA: Automação de Doações → CashFlow
                if (payment.DonationId != null)
                {
                    var donation = await _unitOfWork.Donations.Query()
                        .Include(d => d.Member)
                        .FirstOrDefaultAsync(d => d.Id == payment.DonationId, cancellationToken);

                    if (donation != null)
                    {
                        // ✅ SE PAGAMENTO FOI CONFIRMADO, CRIA LANÇAMENTO AUTOMÁTICO
                        var isPaid = request.Status == PaymentStatus.Completed ||
                                    request.Status == PaymentStatus.Received ||
                                    request.Status == PaymentStatus.Confirmed;

                        if (isPaid && oldStatus != request.Status.ToString())
                        {
                            _logger.LogInformation(
                                "💰 Pagamento confirmado! Criando lançamento automático para doação {DonationId}",
                                donation.Id);

                            try
                            {
                                await _cashFlowAutomation.CreateEntryFromDonationAsync(
                                    donation.Id,
                                    cancellationToken);
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex,
                                    "❌ Erro ao criar lançamento automático para doação {DonationId}",
                                    donation.Id);
                                // Não falha o webhook por causa disso
                            }
                        }
                    }
                }

                // ⚠️ CÓDIGO ANTIGO REMOVIDO (duplicação)
                // A criação de CashFlowEntry agora é feita APENAS pelo CashFlowAutomationService
                // Isso garante:
                // 1. Não duplicar lançamentos
                // 2. Usar valor líquido correto
                // 3. Centralizar lógica de automação

                if (payment.Subscription != null && 
                    (request.Status == PaymentStatus.Completed || 
                     request.Status == PaymentStatus.Received || 
                     request.Status == PaymentStatus.Confirmed))
                {
                    var subscription = payment.Subscription;

                    if (string.IsNullOrEmpty(subscription.ExternalReference))
                    {
                        var assinaturaRequest = new
                        {
                            customer = subscription.Church.AsaasCustomerId,
                            billingType = payment.BillingType,
                            value = subscription.Plan.Price,
                            nextDueDate = DateTime.UtcNow.AddMonths(1).ToString("yyyy-MM-dd"),
                            description = $"Assinatura do plano {subscription.Plan.Name}",
                            externalReference = $"sub-{subscription.Id}-{Guid.NewGuid()}",
                            cycle = "MONTHLY"
                        };

                        var asaasSubscription = await _asaasWebClient.CriarAssinaturaAsync(assinaturaRequest);
                        subscription.ExternalReference = asaasSubscription?.Id;
                    }

                    subscription.Created = subscription.Created == default ? DateTime.UtcNow : subscription.Created;
                    subscription.Updated = DateTime.UtcNow;
                    if (subscription.EndDate < DateTime.UtcNow)
                        subscription.StartDate = DateTime.UtcNow;
                    subscription.EndDate = DateTime.UtcNow.AddMonths(1);

                    _unitOfWork.Subscriptions.Update(subscription);
                }

                await _unitOfWork.CommitAsync();

                _logger.LogInformation($"✅ Pagamento {request.PaymentId} atualizado: {oldStatus} → {request.Status}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ Erro ao confirmar pagamento {request.PaymentId}");
                throw;
            }
        }
    }
}