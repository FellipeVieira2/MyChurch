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
                    var donation = await _unitOfWork.Donations.Query()
                        .Include(d => d.Member)
                        .FirstOrDefaultAsync(d => d.Id == payment.DonationId, cancellationToken);

                    if (donation != null)
                    {
                        var churchId = donation.Member?.ChurchId; // null se visitante
                        if (churchId.HasValue)
                        {
                            var category = await _unitOfWork.CashFlowCategories.Query()
                                .FirstOrDefaultAsync(
                                    c => c.ChurchId == churchId.Value &&
                                         c.Name.ToLower() == "doacao",
                                    cancellationToken);

                            if (category == null)
                            {
                                category = new CashFlowCategory
                                {
                                    Name = "Doacao",
                                    ChurchId = churchId.Value
                                };
                                _unitOfWork.CashFlowCategories.Create(category);
                                await _unitOfWork.CommitAsync();
                            }

                            await _mediator.Send(new CreateCashFlowEntryCommand
                            {
                                UserId = donation.MemberId ?? 0,
                                Amount = donation.Amount,
                                Date = DateTime.UtcNow,
                                Description = "Doação recebida",
                                Type = CashFlowType.Income,
                                CategoryId = category.Id
                            }, cancellationToken);
                        }
                    }
                }
                if (payment.Subscription != null && request.Status == PaymentStatus.Completed || request.Status == PaymentStatus.Received || PaymentStatus.Confirmed == request.Status)
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