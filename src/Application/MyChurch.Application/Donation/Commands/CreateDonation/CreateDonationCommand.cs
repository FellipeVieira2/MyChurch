using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Mychurch.Common.WebClients.Asaas;
using Mychurch.Common.WebClients.Asaas.Models.Requests;
using Mychurch.Common.WebClients.Asaas.Models.Responses;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;
using MyChurch.Domain.Enum;
using MyChurch.Domain.Exceptions;

namespace MyChurch.Application.Donation.Commands.CreateDonation
{
    public class CreateDonationCommand : JwtMemberDto, IRequest<CreateDonationResultDto>
    {
        public decimal Value { get; set; }
        public string? Description { get; set; }
        public string? BillingType { get; set; } = "PIX"; // Default para PIX, pode ser "BOLETO", "CREDIT_CARD", etc.
        public DateTime? DueDate { get; set; }
        public CreditCardDto? CreditCard { get; set; }
        public CreditCardHolderInfoDto? CreditCardHolderInfo { get; set; }
        public int? CreditCardInfoId { get; set; } // Novo: permite usar cartão já cadastrado
    }

    public class CreateDonationResultDto
    {
        public int DonationId { get; set; }
        public string? PaymentLink { get; set; }
        public string? Status { get; set; }
        public object? PixQrCode { get; set; }
        public string Description { get; set; }
        public decimal Value { get; set; }
    }

    public class CreateDonationCommandHandler : IRequestHandler<CreateDonationCommand, CreateDonationResultDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CreateDonationCommandHandler> _logger;
        private readonly IAsaasWebClient _asaasWebClient;

        public CreateDonationCommandHandler(
            IUnitOfWork unitOfWork,
            ILogger<CreateDonationCommandHandler> logger,
            IAsaasWebClient asaasWebClient)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _asaasWebClient = asaasWebClient;
        }

        public async Task<CreateDonationResultDto> Handle(CreateDonationCommand request, CancellationToken cancellationToken)
        {
            // 1. Buscar o membro e sua igreja
            var member = await _unitOfWork.Members
                .Query()
                .Include(x => x.Church)
                .FirstOrDefaultAsync(m => m.Id == request.UserId, cancellationToken);

            if (member == null)
            {
                _logger.LogWarning("Usuário não encontrado ou sem permissão.");
                ValidationException.ThrowException("CreateDonation", "Usuário não encontrado ou sem permissão.");
            }

            var church = member.Church;
            if (church == null || string.IsNullOrEmpty(church.AsaasCustomerId))
            {
                _logger.LogWarning("Igreja não encontrada ou sem integração com Asaas.");
                ValidationException.ThrowException("CreateDonation", "Igreja não encontrada ou sem integração com Asaas.");
            }

            // 2. Montar o request para o Asaas
            var chargeRequest = new AsaasChargeRequestDto
            {
                AsaasCustomerId = church.AsaasCustomerId,
                Value = request.Value,
                BillingType = request.BillingType ?? "PIX",
                DueDate = request.DueDate ?? DateTime.UtcNow.Date,
                Description = request.Description ?? $"Doação de {member.Name}",
                ExternalReference = Guid.NewGuid().ToString()
            };

            // Se for cartão de crédito, pode ser novo ou já cadastrado
            if (request.BillingType == "CREDIT_CARD")
            {
                if (request.CreditCardInfoId.HasValue)
                {
                    // Busca o cartão já cadastrado
                    var cardInfo = await _unitOfWork.CreditCardInfos.Query()
                        .FirstOrDefaultAsync(c => c.Id == request.CreditCardInfoId.Value && c.MemberId == member.Id, cancellationToken);

                    if (cardInfo == null)
                        ValidationException.ThrowException("CreditCard", "Cartão não encontrado para este usuário.");

                    // Envia apenas o token/hash do cartão já salvo
                    chargeRequest.CreditCardToken = cardInfo.CardHash;
                    // O Asaas pode exigir também o CreditCardHolderInfo
                    chargeRequest.CreditCardHolderInfo = request.CreditCardHolderInfo;
                }
                else
                {
                    chargeRequest.CreditCard = request.CreditCard;
                    chargeRequest.CreditCardHolderInfo = request.CreditCardHolderInfo;
                }
            }

            // 3. Criar cobrança no Asaas
            AsaasPaymentResponseDto asaasPayment;
            try
            {
                asaasPayment = await _asaasWebClient.CriarCobrancaAsync(chargeRequest);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar cobrança no Asaas.");
                throw;
            }
            object? pixQrCode = null;
            if (request.BillingType == "PIX")
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

            // 4. Mapear status do Asaas para PaymentStatus
            var paymentStatus = asaasPayment.Status switch
            {
                "PENDING" => PaymentStatus.Pending,
                "RECEIVED" => PaymentStatus.Completed,
                "CONFIRMED" => PaymentStatus.Completed,
                "RECEIVED_IN_CASH" => PaymentStatus.Completed,
                "OVERDUE" => PaymentStatus.Pending,
                "REFUNDED" => PaymentStatus.Cancelled,
                "RECEIVED_PIX" => PaymentStatus.Completed,
                "CHARGEBACK_REQUESTED" => PaymentStatus.Failed,
                "CHARGEBACK_DISPUTE" => PaymentStatus.Failed,
                "AWAITING_CHARGEBACK_REVERSAL" => PaymentStatus.Failed,
                "DISPUTE" => PaymentStatus.Failed,
                "CANCELLED" => PaymentStatus.Cancelled,
                _ => PaymentStatus.Pending
            };

            // 5. Criar a doação e o pagamento
            var donation = new Domain.Entities.Donation
            {
                MemberId = member.Id,
                Amount = asaasPayment.NetValue,
                Date = DateTime.UtcNow
            };

            var payment = new Payment
            (
                asaasPayment.NetValue,
                asaasPayment.DueDate,
                paymentStatus.ToString(),
                asaasPayment.Id,
                request.BillingType
            )
            {
                Donation = donation,
                DonationId = donation.Id
            };

            donation.Payments ??= new List<Payment>();
            donation.Payments.Add(payment);

            var platformFeeValue = donation.Amount * church.PlatformFee;

            donation.Amount -= platformFeeValue;
            donation.PlatformFee = platformFeeValue;

            // Salva cartão novo se for cartão de crédito e não foi usado um já cadastrado
            if (request.BillingType == "CREDIT_CARD" && !request.CreditCardInfoId.HasValue && asaasPayment.CreditCard != null)
            {
                var last4 = request.CreditCard?.Number?.Length >= 4
                    ? request.CreditCard.Number[^4..]
                    : "";

                var cardInfo = new CreditCardInfo()
                {
                    CardBrand = asaasPayment.CreditCard.CreditCardBrand,
                    CardHash = asaasPayment.CreditCard.CreditCardToken,
                    Created = DateTime.UtcNow,
                    Last4Digits = last4,
                    MemberId = member.Id
                };
                _unitOfWork.CreditCardInfos.Create(cardInfo);
            }

            _unitOfWork.Members.Update(member);
            // 6. Persistir no banco
            _unitOfWork.Donations.Create(donation);
            await _unitOfWork.CommitAsync();

            return new CreateDonationResultDto
            {
                DonationId = donation.Id,
                PaymentLink = asaasPayment.InvoiceUrl,
                Status = asaasPayment.Status,
                PixQrCode = pixQrCode,
                Description = request.Description,
                Value = request.Value
            };
        }
    }
}