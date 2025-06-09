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
using System;

namespace MyChurch.Application.Subscription.Commands.CreateSubscription
{
    public class CreateSubscriptionCommand : JwtMemberDto, IRequest<CreateSubscriptionResultDto>
    {
        public int PlanId { get; set; }
        public string BillingType { get; set; } // Ex:"PIX", "CREDIT_CARD"
        public DateTime? FirstPaymentDate { get; set; }

        // Adicione os campos para cartão de crédito
        public CreditCardDto? CreditCard { get; set; }
        public CreditCardHolderInfoDto? CreditCardHolderInfo { get; set; }
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

            if (request.BillingType == "CREDIT_CARD")
            {
                cobrancaRequest.CreditCard = request.CreditCard;
                cobrancaRequest.CreditCardHolderInfo = request.CreditCardHolderInfo;
            }


            // Cria a cobrança no Asaas
            var cobrancaResponse = await _asaasWebClient.CriarCobrancaAsync(cobrancaRequest);

            PixQrCodeResponseDto? pixQrCode = null;
            if (request.BillingType == "PIX" && !string.IsNullOrEmpty(transactionId))
            {
                try
                {
                    pixQrCode = await _asaasWebClient.GerarPixQrCodeAsync(transactionId);
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

            _unitOfWork.Payments.Create(payment);

            // Salva o token do cartão de crédito se for cartão
            if (request.BillingType == "CREDIT_CARD")
            {
                loggedMember.CreditCardHash = cobrancaResponse.CreditCard?.CreditCardToken;
                _unitOfWork.Members.Update(loggedMember);
            }

            await _unitOfWork.CommitAsync();

            _logger.LogInformation("Assinatura solicitada para a igreja {ChurchId} no plano {PlanId}. Link: {CheckoutUrl}", church.Id, plan.Id, checkoutUrl);

            return new CreateSubscriptionResultDto
            {
                CheckoutUrl = checkoutUrl,
                PixQrCode = pixQrCode.EncodedImage,
                Payload = pixQrCode.Payload
            };
        }
    }
}