using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Mychurch.Common.WebClients.Asaas;
using MyChurch.Application.CashFlow.Commands.CreateCashFlowEntry;
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

        public ConfirmPaymentCommandHandler(
            IUnitOfWork unitOfWork,
            ILogger<ConfirmPaymentCommandHandler> logger,
            IAsaasWebClient asaasWebClient,
            IMediator mediator)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _asaasWebClient = asaasWebClient;
            _mediator = mediator;
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
                    .FirstOrDefaultAsync(p => p.TransactionId == request.PaymentId, cancellationToken);

                if (payment == null)
                {
                    _logger.LogWarning($"Pagamento não encontrado para o ID Asaas: {request.PaymentId}");
                    return false;
                }

                payment.PaymentStatus = request.Status.ToString();
                payment.Date = DateTime.UtcNow;
                _unitOfWork.Payments.Update(payment);

                if (payment.DonationId != null)
                {
                    // Busca a doação para pegar valores e descrição
                    var donation = await _unitOfWork.Donations.Query()
                        .Include(d => d.Member)
                        .FirstOrDefaultAsync(d => d.Id == payment.DonationId, cancellationToken);

                    if (donation != null)
                    {
                        // Busca a categoria "Doacao" (case-insensitive) para a igreja
                        var category = await _unitOfWork.CashFlowCategories.Query()
                            .FirstOrDefaultAsync(
                                c => c.ChurchId == donation.Member.ChurchId &&
                                     c.Name.ToLower() == "doacao",
                                cancellationToken);

                        // Se não existir, cria a categoria "Doacao"
                        if (category == null)
                        {
                            category = new CashFlowCategory
                            {
                                Name = "Doacao",
                                ChurchId = donation.Member.ChurchId
                            };
                            _unitOfWork.CashFlowCategories.Create(category);
                            await _unitOfWork.CommitAsync();
                        }

                        await _mediator.Send(new CreateCashFlowEntryCommand
                        {
                            UserId = donation.MemberId,
                            Amount = donation.Amount,
                            Date = DateTime.UtcNow,
                            Description = $"Doação recebida: {donation.Member.Name} Dia: {donation.Date}",
                            Type = CashFlowType.Income,
                            CategoryId = category.Id
                        }, cancellationToken);
                    }
                }
                // Se for pagamento de assinatura e status for Completed
                if (payment.Subscription != null && request.Status == PaymentStatus.Completed || request.Status == PaymentStatus.Received)
                {
                    var subscription = payment.Subscription;

                    // Se ainda não existe assinatura no Asaas (ExternalReference == null ou vazia)
                    if (string.IsNullOrEmpty(subscription.ExternalReference))
                    {
                        // Monta o request para criar assinatura no Asaas
                        var assinaturaRequest = new
                        {
                            customer = subscription.Church.AsaasCustomerId,
                            billingType = payment.BillingType, // ajuste conforme seu modelo
                            value = subscription.Plan.Price,
                            nextDueDate = DateTime.UtcNow.AddMonths(1).ToString("yyyy-MM-dd"),
                            description = $"Assinatura do plano {subscription.Plan.Name}",
                            externalReference = $"sub-{subscription.Id}-{Guid.NewGuid()}",
                            cycle = "MONTHLY"
                        };

                        // Cria assinatura no Asaas
                        var asaasSubscription = await _asaasWebClient.CriarAssinaturaAsync(assinaturaRequest);

                        // Salva o ID da assinatura do Asaas no campo ExternalReference
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

                _logger.LogInformation($"Pagamento {request.PaymentId} atualizado para status {request.Status} com sucesso.");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao confirmar pagamento {request.PaymentId}");
                throw;
            }
        }
    }
}