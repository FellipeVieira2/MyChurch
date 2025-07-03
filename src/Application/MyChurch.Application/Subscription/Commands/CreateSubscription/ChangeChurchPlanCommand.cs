using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Mychurch.Common.WebClients.Asaas;
using Mychurch.Common.WebClients.Asaas.Models.Requests;
using Mychurch.Common.WebClients.Asaas.Models.Responses;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Exceptions;

namespace MyChurch.Application.Subscription.Commands.CreateSubscription
{
    public class ChangeChurchPlanCommand : JwtMemberDto, IRequest<ChangeChurchPlanResultDto>
    {
        public int NewPlanId { get; set; }
        public string? Reason { get; set; }
        public string? BillingType { get; set; } = "PIX";
        public CreditCardDto? CreditCard { get; set; }
        public CreditCardHolderInfoDto? CreditCardHolderInfo { get; set; }
        public int? CreditCardInfoId { get; set; }
    }

    public class ChangeChurchPlanResultDto
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public decimal? ChargedAmount { get; set; }
        public string? PaymentLink { get; set; }
        public string? PixQrCode { get; set; }
        public string? Payload { get; set; }
    }

    public class ChangeChurchPlanCommandHandler : IRequestHandler<ChangeChurchPlanCommand, ChangeChurchPlanResultDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAsaasWebClient _asaasWebClient;
        private readonly ILogger<ChangeChurchPlanCommandHandler> _logger;

        public ChangeChurchPlanCommandHandler(
            IUnitOfWork unitOfWork,
            IAsaasWebClient asaasWebClient,
            ILogger<ChangeChurchPlanCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _asaasWebClient = asaasWebClient;
            _logger = logger;
        }

        public async Task<ChangeChurchPlanResultDto> Handle(ChangeChurchPlanCommand request, CancellationToken cancellationToken)
        {
            var loggedMember = await _unitOfWork.Members.Query()
                .FirstOrDefaultAsync(m => m.Id == request.UserId, cancellationToken);
            if (loggedMember == null)
                ValidationException.ThrowException("User", "Usuário não encontrado.");
            int churchId = loggedMember.ChurchId;

            var subscription = await _unitOfWork.Subscriptions.Query()
                .Include(s => s.Plan)
                .FirstOrDefaultAsync(s => s.ChurchId == churchId && s.EndDate > DateTime.UtcNow, cancellationToken);
            var newPlan = await _unitOfWork.Plans.Query().FirstOrDefaultAsync(x => x.Id == request.NewPlanId, cancellationToken);
            if (subscription == null || subscription.Plan == null || newPlan == null)
                ValidationException.ThrowException("Plan", "Assinatura ou plano não encontrado.");

            var currentPlan = subscription.Plan;
            if (newPlan.Id == currentPlan.Id)
                return new ChangeChurchPlanResultDto { Success = false, Message = "O plano já está ativo." };

            // Downgrade
            if (newPlan.Price < currentPlan.Price)
            {
                subscription.PlanId = newPlan.Id;
                subscription.Updated = DateTime.UtcNow;
                _unitOfWork.Subscriptions.Update(subscription);
                await _unitOfWork.CommitAsync();
                _logger.LogInformation($"Downgrade de plano para igreja {churchId}: {currentPlan.Name} -> {newPlan.Name}");
                return new ChangeChurchPlanResultDto { Success = true, Message = "Plano alterado com sucesso (downgrade). Nenhuma cobrança realizada." };
            }

            // Upgrade
            if (newPlan.Price > currentPlan.Price)
            {
                var today = DateTime.UtcNow.Date;
                var daysInMonth = DateTime.DaysInMonth(today.Year, today.Month);
                var daysLeft = (subscription.EndDate.Date - today).Days;
                if (daysLeft < 1) daysLeft = 1;
                var priceDiff = newPlan.Price - currentPlan.Price;
                var proportionalAmount = Math.Round(priceDiff * daysLeft / daysInMonth, 2);

                var church = await _unitOfWork.Churchs.Query().FirstOrDefaultAsync(x => x.Id == churchId, cancellationToken);
                if (church == null || string.IsNullOrEmpty(church.AsaasCustomerId))
                    ValidationException.ThrowException("Church", "Igreja não encontrada ou sem integração Asaas.");

                var chargeRequest = new AsaasChargeRequestDto
                {
                    AsaasCustomerId = church.AsaasCustomerId,
                    Value = proportionalAmount,
                    BillingType = request.BillingType ?? "PIX",
                    DueDate = today,
                    Description = $"Upgrade de plano: {currentPlan.Name} -> {newPlan.Name} (proporcional)",
                    ExternalReference = $"upgrade-{churchId}-{Guid.NewGuid()}"
                };

                // Se for cartão de crédito, pode ser novo ou já cadastrado
                if (request.BillingType == "CREDIT_CARD")
                {
                    if (request.CreditCardInfoId.HasValue)
                    {
                        var cardInfo = await _unitOfWork.CreditCardInfos.Query()
                            .FirstOrDefaultAsync(c => c.Id == request.CreditCardInfoId.Value && c.MemberId == loggedMember.Id, cancellationToken);
                        if (cardInfo == null)
                            ValidationException.ThrowException("CreditCard", "Cartão não encontrado para este usuário.");
                        chargeRequest.CreditCardToken = cardInfo.CardHash;
                        chargeRequest.CreditCardHolderInfo = request.CreditCardHolderInfo;
                    }
                    else
                    {
                        chargeRequest.CreditCard = request.CreditCard;
                        chargeRequest.CreditCardHolderInfo = request.CreditCardHolderInfo;
                    }
                }

                var asaasPayment = await _asaasWebClient.CriarCobrancaAsync(chargeRequest);

                PixQrCodeResponseDto? pixQrCode = null;
                if (request.BillingType == "PIX" && !string.IsNullOrEmpty(asaasPayment.Id))
                {
                    try
                    {
                        pixQrCode = await _asaasWebClient.GerarPixQrCodeAsync(asaasPayment.Id);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Erro ao buscar QRCode PIX no Asaas.");
                    }
                }

                subscription.PlanId = newPlan.Id;
                subscription.Updated = DateTime.UtcNow;
                _unitOfWork.Subscriptions.Update(subscription);
                await _unitOfWork.CommitAsync();

                _logger.LogInformation($"Upgrade de plano para igreja {churchId}: {currentPlan.Name} -> {newPlan.Name}. Valor proporcional: {proportionalAmount}");
                return new ChangeChurchPlanResultDto
                {
                    Success = true,
                    Message = "Plano alterado com sucesso (upgrade). Cobrança proporcional gerada.",
                    ChargedAmount = proportionalAmount,
                    PaymentLink = asaasPayment.InvoiceUrl,
                    PixQrCode = pixQrCode?.EncodedImage,
                    Payload = pixQrCode?.Payload
                };
            }

            return new ChangeChurchPlanResultDto { Success = false, Message = "Operação inválida." };
        }
    }
}
