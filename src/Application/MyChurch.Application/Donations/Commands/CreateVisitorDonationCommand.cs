using MediatR;
using Microsoft.EntityFrameworkCore;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Exceptions;
using Mychurch.Common.WebClients.Asaas;

namespace MyChurch.Application.Donations.Commands
{
    public class CreateVisitorDonationCommand : IRequest<CreateVisitorDonationResultDto>
    {
        public int VisitorId { get; set; }
        public decimal Amount { get; set; }
        public string BillingType { get; set; } = "PIX"; // PIX, CREDIT_CARD, BOLETO
        public int? WorshipServiceId { get; set; }
        public int? CampaignId { get; set; }
        public string? Description { get; set; }
    }

    public class CreateVisitorDonationResultDto
    {
        public int DonationId { get; set; }
        public string PaymentId { get; set; }
        public string? PaymentLink { get; set; }
        public string? PixQrCodeBase64 { get; set; }
        public string? PixPayload { get; set; }
        public decimal Amount { get; set; }
        public string BillingType { get; set; }
    }

    public class CreateVisitorDonationCommandHandler : IRequestHandler<CreateVisitorDonationCommand, CreateVisitorDonationResultDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAsaasWebClient _asaas;
        public CreateVisitorDonationCommandHandler(IUnitOfWork unitOfWork, IAsaasWebClient asaas)
        {
            _unitOfWork = unitOfWork;
            _asaas = asaas;
        }
        public async Task<CreateVisitorDonationResultDto> Handle(CreateVisitorDonationCommand request, CancellationToken cancellationToken)
        {
            if (request.Amount <= 0)
                ValidationException.ThrowException("Donation", "Valor inválido");

            var visitor = await _unitOfWork.Visitors.Query().FirstOrDefaultAsync(v => v.Id == request.VisitorId, cancellationToken);
            if (visitor == null)
                ValidationException.ThrowException("Visitor", "Visitante não encontrado");

            if (request.WorshipServiceId.HasValue)
            {
                var worship = await _unitOfWork.WorshipServices.Query().FirstOrDefaultAsync(w => w.Id == request.WorshipServiceId.Value, cancellationToken);
                if (worship == null)
                    ValidationException.ThrowException("Worship", "Culto inválido");
            }

            var donation = new MyChurch.Domain.Entities.Donation
            {
                VisitorId = request.VisitorId,
                Amount = request.Amount,
                Date = DateTime.UtcNow,
                PlatformFee = request.Amount * 0.05m
            };

            if (request.CampaignId.HasValue)
            {
                donation.GetType().GetProperty("CampaignId")?.SetValue(donation, request.CampaignId.Value);
            }

            _unitOfWork.Donations.Create(donation);
            await _unitOfWork.CommitAsync();

            if (request.WorshipServiceId.HasValue)
            {
                var link = new MyChurch.Domain.Entities.DonationWorshipService
                {
                    DonationId = donation.Id,
                    WorshipServiceId = request.WorshipServiceId.Value
                };
                _unitOfWork.DonationWorshipServices.Create(link);
                await _unitOfWork.CommitAsync();
            }

            // Cria payment interno pendente
            var internalPayment = new MyChurch.Domain.Entities.Payment(request.Amount, DateTime.UtcNow, "Pending", Guid.NewGuid().ToString("N"), request.BillingType)
            {
                DonationId = donation.Id
            };
            _unitOfWork.Payments.Create(internalPayment);
            await _unitOfWork.CommitAsync();

            string? paymentLink = null;
            string? pixQrCode = null;
            string? pixPayload = null;

            // Cria cobrança Asaas se houver customer
            if (!string.IsNullOrEmpty(visitor.AsaasCustomerId))
            {
                var cobrancaRequest = new
                {
                    customer = visitor.AsaasCustomerId,
                    billingType = request.BillingType,
                    value = request.Amount,
                    description = request.Description ?? "Doação visitante",
                    dueDate = DateTime.UtcNow.Date.AddDays(1).ToString("yyyy-MM-dd"),
                    externalReference = $"don-{donation.Id}-{internalPayment.TransactionId}" // referência cruzada
                };
                try
                {
                    var asaasPayment = await _asaas.CriarCobrancaAsync(cobrancaRequest);
                    paymentLink = asaasPayment?.InvoiceUrl ?? asaasPayment?.PaymentLink;
                    // Atualiza transaction id local para rastrear webhook
                    internalPayment.TransactionId = asaasPayment.Id;
                    _unitOfWork.Payments.Update(internalPayment);
                    await _unitOfWork.CommitAsync();

                    if (request.BillingType == "PIX")
                    {
                        try
                        {
                            var pix = await _asaas.GerarPixQrCodeAsync(asaasPayment.Id);
                            pixQrCode = pix?.EncodedImage; // base64
                            pixPayload = pix?.Payload;
                        }
                        catch { }
                    }
                }
                catch
                {
                    // tolerar falha externa, doação segue como pending interna
                }
            }

            return new CreateVisitorDonationResultDto
            {
                DonationId = donation.Id,
                PaymentId = internalPayment.TransactionId,
                PaymentLink = paymentLink,
                PixQrCodeBase64 = pixQrCode,
                PixPayload = pixPayload,
                Amount = request.Amount,
                BillingType = request.BillingType
            };
        }
    }
}
