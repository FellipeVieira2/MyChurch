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

namespace MyChurch.Application.Subscription.Commands.CreateSubscription
{
    public class CreateSubscriptionCommand : JwtMemberDto, IRequest<CreateSubscriptionResultDto>
    {
        public int PlanId { get; set; }
        public string BillingType { get; set; } // Ex:"PIX", "CREDIT_CARD"
        public DateTime? FirstPaymentDate { get; set; }

        public CreditCardDto? CreditCard { get; set; }
        public CreditCardHolderInfoDto? CreditCardHolderInfo { get; set; }
        public int? CreditCardInfoId { get; set; } // Novo: permite usar cartão já cadastrado
    }
    public class CreateSubscriptionResultDto
    {
        public string? CheckoutUrl { get; set; }
        public string? PixQrCode { get; set; }
        public string Payload { get; set; }
    }
    public class CreateSubscriptionCommandHandler : IRequestHandler<CreateSubscriptionCommand, CreateSubscriptionResultDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAsaasWebClient _asaasWebClient;
        private readonly ILogger<CreateSubscriptionCommandHandler> _logger;

        public CreateSubscriptionCommandHandler(
            IUnitOfWork unitOfWork,
            IAsaasWebClient asaasWebClient,
            ILogger<CreateSubscriptionCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _asaasWebClient = asaasWebClient;
            _logger = logger;
        }

        public async Task<CreateSubscriptionResultDto> Handle(CreateSubscriptionCommand request, CancellationToken cancellationToken)
        {
            // Busca o membro logado para obter o ChurchId
            var loggedMember = await _unitOfWork.Members.Query()
                .FirstOrDefaultAsync(m => m.Id == request.UserId, cancellationToken);

            if (loggedMember == null)
                ValidationException.ThrowException("User", "Usuário não encontrado.");

            int churchId = loggedMember.ChurchId;

            // Buscar igreja e plano
            var church = await _unitOfWork.Churchs.Query().FirstOrDefaultAsync(x => x.Id == churchId, cancellationToken);
            var plan = await _unitOfWork.Plans.Query().FirstOrDefaultAsync(x => x.Id == request.PlanId);

            if (church == null || plan == null)
                ValidationException.ThrowException("User", "Igreja ou plano não encontrado.");

            string? checkoutUrl = null;
            string? transactionId = null;

            // Se o plano for grátis (exemplo: ID == 1), não cria fatura e deixa a assinatura ativa por 1 mês
            if (plan.Price == 0)
            {
                var subscription = new Domain.Entities.Subscription(
                    planId: plan.Id,
                    startDate: DateTime.UtcNow,
                    endDate: DateTime.UtcNow.AddMonths(1)
                )
                {
                    ChurchId = church.Id,
                    Created = DateTime.UtcNow,
                    ExternalReference = null
                };

                _unitOfWork.Subscriptions.Create(subscription);
                await _unitOfWork.CommitAsync();

                _logger.LogInformation("Assinatura gratuita criada para a igreja {ChurchId} no plano {PlanId}.", church.Id, plan.Id);

                // Não há link de checkout para plano grátis
                return null;
            }

            // Monta o request para o Asaas
            var cobrancaRequest = new AsaasChargeRequestDto
            {
                AsaasCustomerId = church.AsaasCustomerId,
                Value = plan.Price,
                BillingType = request.BillingType,
                DueDate = (request.FirstPaymentDate ?? DateTime.UtcNow.Date),
                Description = $"Assinatura do plano {plan.Name}",
                ExternalReference = $"{church.Id}-{plan.Id}-{DateTime.UtcNow:yyyyMMddHHmmss}"
            };

            // Se for cartão de crédito, pode ser novo ou já cadastrado
            if (request.BillingType == "CREDIT_CARD")
            {
                if (request.CreditCardInfoId.HasValue)
                {
                    // Busca o cartão já cadastrado
                    var cardInfo = await _unitOfWork.CreditCardInfos.Query()
                        .FirstOrDefaultAsync(c => c.Id == request.CreditCardInfoId.Value && c.MemberId == loggedMember.Id, cancellationToken);

                    if (cardInfo == null)
                        ValidationException.ThrowException("CreditCard", "Cartão não encontrado para este usuário.");

                    cobrancaRequest.CreditCardToken = cardInfo.CardHash;
                    cobrancaRequest.CreditCardHolderInfo = request.CreditCardHolderInfo;
                }
                else
                {
                    cobrancaRequest.CreditCard = request.CreditCard;
                    cobrancaRequest.CreditCardHolderInfo = request.CreditCardHolderInfo;
                }
            }

            // Cria a cobrança no Asaas
            var cobrancaResponse = await _asaasWebClient.CriarCobrancaAsync(cobrancaRequest);

            PixQrCodeResponseDto? pixQrCode = null;
            if (request.BillingType == "PIX" && !string.IsNullOrEmpty(cobrancaResponse.Id))
            {
                try
                {
                    pixQrCode = await _asaasWebClient.GerarPixQrCodeAsync(cobrancaResponse.Id);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao buscar QRCode PIX no Asaas.");
                }
            }

            // Extrai o link do checkout e o id da cobrança
            checkoutUrl = cobrancaResponse.InvoiceUrl ?? cobrancaResponse?.BankSlipUrl;
            transactionId = cobrancaResponse.Id;

            if (string.IsNullOrEmpty(checkoutUrl) || string.IsNullOrEmpty(transactionId))
                ValidationException.ThrowException("Asaas", "Não foi possível obter o link de pagamento do Asaas.");

            // Cria a assinatura local (inativa)
            var paidSubscription = new Domain.Entities.Subscription(
                planId: plan.Id,
                startDate: DateTime.UtcNow,
                endDate: DateTime.UtcNow.AddDays(-1) // Data inferior ao dia de hoje para não ficar ativa
            )
            {
                ChurchId = church.Id,
                Created = DateTime.UtcNow,
                ExternalReference = transactionId
            };

            _unitOfWork.Subscriptions.Create(paidSubscription);
            await _unitOfWork.CommitAsync();

            // Salva o pagamento localmente
            var payment = new Payment(
                   amount: plan.Price,
                   date: DateTime.UtcNow,
                   paymentStatus: PaymentStatus.Pending.ToString(),
                   transactionId: transactionId,
                   request.BillingType
               );
            payment.SubscriptionId = paidSubscription.Id;

            _unitOfWork.Payments.Create(payment);

            // Salva cartão novo se for cartão de crédito e não foi usado um já cadastrado
            if (request.BillingType == "CREDIT_CARD" && !request.CreditCardInfoId.HasValue && cobrancaResponse.CreditCard != null)
            {
                var last4 = request.CreditCard?.Number?.Length >= 4
                    ? request.CreditCard.Number[^4..]
                    : "";

                var cardInfo = new CreditCardInfo()
                {
                    CardBrand = cobrancaResponse.CreditCard.CreditCardBrand,
                    CardHash = cobrancaResponse.CreditCard.CreditCardToken,
                    Created = DateTime.UtcNow,
                    Last4Digits = last4,
                    MemberId = loggedMember.Id
                };
                _unitOfWork.CreditCardInfos.Create(cardInfo);
            }


            await _unitOfWork.CommitAsync();

            _logger.LogInformation("Assinatura solicitada para a igreja {ChurchId} no plano {PlanId}. Link: {CheckoutUrl}", church.Id, plan.Id, checkoutUrl);

            return new CreateSubscriptionResultDto
            {
                CheckoutUrl = checkoutUrl,
                PixQrCode = pixQrCode?.EncodedImage,
                Payload = pixQrCode?.Payload
            };
        }
    }
}