using System;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Mychurch.Common.WebClients.Asaas;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;
using MyChurch.Domain.Enum;
using MyChurch.Domain.Exceptions;

namespace MyChurch.Application.Subscription.Commands.CreateSubscription
{
    public class CreateSubscriptionCommand : JwtMemberDto, IRequest<string>
    {
        public int PlanId { get; set; }
        public string BillingType { get; set; } // Ex:"PIX", "CREDIT_CARD"
        public DateTime? FirstPaymentDate { get; set; }
    }

    public class CreateSubscriptionCommandHandler : IRequestHandler<CreateSubscriptionCommand, string>
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

        public async Task<string> Handle(CreateSubscriptionCommand request, CancellationToken cancellationToken)
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
            var cobrancaRequest = new
            {
                customer = church.AsaasCustomerId,
                value = plan.Price,
                billingType = request.BillingType,
                dueDate = (request.FirstPaymentDate ?? DateTime.UtcNow.Date).ToString("yyyy-MM-dd"),
                description = $"Assinatura do plano {plan.Name}",
                externalReference = $"{church.Id}-{plan.Id}-{DateTime.UtcNow:yyyyMMddHHmmss}"
            };

            // Cria a cobrança no Asaas
            var cobrancaResponse = await _asaasWebClient.CriarCobrancaAsync(cobrancaRequest);

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
                   subscriptionId: paidSubscription.Id,
                   amount: plan.Price,
                   date: DateTime.UtcNow,
                   paymentStatus: PaymentStatus.Pending.ToString(),
                   transactionId: transactionId
               );

            _unitOfWork.Payments.Create(payment);
            await _unitOfWork.CommitAsync();

            _logger.LogInformation("Assinatura solicitada para a igreja {ChurchId} no plano {PlanId}. Link: {CheckoutUrl}", church.Id, plan.Id, checkoutUrl);

            return checkoutUrl;
        }
    }
}