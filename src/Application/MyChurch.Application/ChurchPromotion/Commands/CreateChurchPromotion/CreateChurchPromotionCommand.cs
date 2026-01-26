using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyChurch.Application.Dtos;
using MyChurch.Application.Plans.Services;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;
using MyChurch.Domain.Enum;
using MyChurch.Domain.Exceptions;
using Mychurch.Common.WebClients.Asaas;
using Mychurch.Common.WebClients.Asaas.Models.Requests;
using Mychurch.Common.WebClients.Asaas.Models.Responses;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MyChurch.Application.ChurchPromotion.Commands.CreateChurchPromotion
{
    public class CreateChurchPromotionCommand : JwtMemberDto, IRequest<int>
    {
        /// <summary>
        /// Tipo de promoção desejada
        /// </summary>
        public PromotionType Type { get; set; }
        
        /// <summary>
        /// Duração da promoção em dias
        /// </summary>
        public int DurationInDays { get; set; }
        
        /// <summary>
        /// Data de início da promoção (opcional - padrão: hoje)
        /// </summary>
        public DateTime? StartDate { get; set; }
        
        /// <summary>
        /// Região alvo (opcional) - Ex: "SP" ou "SP,São Paulo"
        /// </summary>
        public string? TargetRegion { get; set; }
        
        /// <summary>
        /// Banner personalizado (Base64 ou URL)
        /// </summary>
        public string? CustomBannerUrl { get; set; }
        
        /// <summary>
        /// Texto personalizado para a promoção
        /// </summary>
        public string? CustomText { get; set; }
        
        /// <summary>
        /// Método de pagamento (PIX, CREDIT_CARD, etc)
        /// </summary>
        public string PaymentMethod { get; set; } = "PIX";
        
        /// <summary>
        /// Dados do cartão de crédito (se for o método escolhido)
        /// </summary>
        public CreditCardDto? CreditCard { get; set; }
        
        public CreditCardHolderInfoDto? CreditCardHolderInfo { get; set; }
        
        /// <summary>
        /// ID de cartão já salvo (opcional)
        /// </summary>
        public int? CreditCardInfoId { get; set; }
    }

    public class CreateChurchPromotionCommandHandler : IRequestHandler<CreateChurchPromotionCommand, int>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CreateChurchPromotionCommandHandler> _logger;
        private readonly IAsaasWebClient _asaasWebClient;
        private readonly IPlanLimitService _planLimits;

        public CreateChurchPromotionCommandHandler(
            IUnitOfWork unitOfWork,
            ILogger<CreateChurchPromotionCommandHandler> logger,
            IAsaasWebClient asaasWebClient,
            IPlanLimitService planLimits)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _asaasWebClient = asaasWebClient;
            _planLimits = planLimits;
        }

        public async Task<int> Handle(CreateChurchPromotionCommand request, CancellationToken cancellationToken)
        {
            // 1. Buscar membro e igreja
            var member = await _unitOfWork.Members.Query()
                .AsNoTracking()
                .Include(m => m.Church)
                .FirstOrDefaultAsync(m => m.Id == request.UserId, cancellationToken);

            if (member == null)
                ValidationException.ThrowException("Member", "Usuário não encontrado.");

            if (member.Role != UserRole.Admin)
                ValidationException.ThrowException("Member", "Apenas administradores podem criar promoções.");

            // ?? Plano: precisa permitir promoções de igreja
            // Reutiliza o member já carregado no handler (não redeclara)
            await _planLimits.EnsurePromotionAllowedAsync(member.ChurchId, MyChurch.Application.Plans.Services.PlanPromotionType.Church, cancellationToken);

            var church = member.Church;
            if (church == null)
                ValidationException.ThrowException("Church", "Igreja não encontrada.");

            // 2. ?? VALIDAR SE O PLANO PERMITE PROMOÇÕES (PREMIUM)
            var subscription = church.Subscription;
            if (subscription == null || subscription.Plan == null)
                ValidationException.ThrowException("Subscription", "Igreja não possui assinatura ativa.");

            if (!subscription.Plan.CanPromoteChurch)
                ValidationException.ThrowException("Plan", 
                    "? Seu plano FREE não permite promoções. Faça upgrade para PREMIUM (R$ 79,90/mês) para desbloquear:\n" +
                    "? Promoção de igreja e eventos\n" +
                    "? Remoção de anúncios internos\n" +
                    "? Analytics avançados\n" +
                    "? Badge verificado");

            // 3. Calcular datas baseado em duração
            var startDate = request.StartDate ?? DateTime.UtcNow;
            var endDate = startDate.AddDays(request.DurationInDays);

            // 4. Validar datas
            if (startDate < DateTime.UtcNow)
                ValidationException.ThrowException("Promotion", "Data de início não pode ser no passado.");

            if (request.DurationInDays < 1)
                ValidationException.ThrowException("Promotion", "Duração mínima é de 1 dia.");

            if (request.DurationInDays > 365)
                ValidationException.ThrowException("Promotion", "Duração máxima é de 365 dias.");

            // 5. Calcular valor da promoção
            var amountToPay = CalculatePromotionPrice(request.Type, request.DurationInDays);

            // 6. ?? CRIAR COBRANÇA NO ASAAS (INTEGRAÇÃO REAL)
            if (string.IsNullOrEmpty(church.AsaasCustomerId))
                ValidationException.ThrowException("Church", "Igreja não possui integração com Asaas.");

            var chargeRequest = new AsaasChargeRequestDto
            {
                AsaasCustomerId = church.AsaasCustomerId,
                Value = amountToPay,
                BillingType = request.PaymentMethod ?? "PIX",
                DueDate = DateTime.UtcNow.Date,
                Description = $"Promoção de Igreja - {request.Type} ({request.DurationInDays} dias)",
                ExternalReference = $"church-promo-{Guid.NewGuid()}"
            };

            // Se for cartão de crédito
            if (request.PaymentMethod == "CREDIT_CARD")
            {
                if (request.CreditCardInfoId.HasValue)
                {
                    var cardInfo = await _unitOfWork.CreditCardInfos.Query()
                        .FirstOrDefaultAsync(c => c.Id == request.CreditCardInfoId.Value && c.MemberId == member.Id, cancellationToken);
                    
                    if (cardInfo == null)
                        ValidationException.ThrowException("CreditCard", "Cartão não encontrado.");

                    chargeRequest.CreditCardToken = cardInfo.CardHash;
                    chargeRequest.CreditCardHolderInfo = request.CreditCardHolderInfo;
                }
                else
                {
                    chargeRequest.CreditCard = request.CreditCard;
                    chargeRequest.CreditCardHolderInfo = request.CreditCardHolderInfo;
                }
            }

            // ?? CRIA A COBRANÇA NO ASAAS
            var asaasPayment = await _asaasWebClient.CriarCobrancaAsync(chargeRequest);

            // 7. Criar a promoção (inicialmente PendingApproval até o pagamento ser confirmado)
            var promotion = new Domain.Entities.ChurchPromotion
            {
                ChurchId = church.Id,
                Type = request.Type,
                StartDate = startDate,
                EndDate = endDate,
                AmountPaid = amountToPay,
                Status = PromotionStatus.PendingApproval, // Aguardando confirmação do pagamento
                TargetRegion = request.TargetRegion,
                CustomBannerUrl = request.CustomBannerUrl,
                CustomText = request.CustomText,
                Created = DateTime.UtcNow
            };

            _unitOfWork.ChurchPromotions.Create(promotion);
            await _unitOfWork.CommitAsync();

            // 8. ?? CRIAR PAGAMENTO LOCAL
            var payment = new Payment(
                amount: amountToPay,
                date: DateTime.UtcNow,
                paymentStatus: asaasPayment.Status == "RECEIVED" ? PaymentStatus.Completed.ToString() : PaymentStatus.Pending.ToString(),
                transactionId: asaasPayment.Id,
                billingType: request.PaymentMethod ?? "PIX"
            );

            _unitOfWork.Payments.Create(payment);
            await _unitOfWork.CommitAsync();

            // 9. Vincular o pagamento à promoção
            promotion.PaymentId = payment.Id;
            _unitOfWork.ChurchPromotions.Update(promotion);
            await _unitOfWork.CommitAsync();

            // 10. Se pagamento for PIX, gera QR Code
            PixQrCodeResponseDto? pixQrCode = null;
            if (request.PaymentMethod == "PIX")
            {
                try
                {
                    pixQrCode = await _asaasWebClient.GerarPixQrCodeAsync(asaasPayment.Id);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao gerar QR Code PIX");
                }
            }

            // 11. ? SE CARTÃO FOI APROVADO NA HORA, ATIVA AUTOMATICAMENTE
            if (request.PaymentMethod == "CREDIT_CARD" && asaasPayment.Status == "RECEIVED")
            {
                promotion.Status = PromotionStatus.Active;
                _unitOfWork.ChurchPromotions.Update(promotion);
                await _unitOfWork.CommitAsync();
                
                _logger.LogInformation("Promoção {PromotionId} ativada automaticamente após pagamento por cartão", promotion.Id);
            }

            _logger.LogInformation(
                "Promoção criada para igreja {ChurchId}. Tipo: {Type}, Duração: {Days} dias, Valor: R$ {Amount}, PaymentId: {PaymentId}", 
                church.Id, request.Type, request.DurationInDays, amountToPay, payment.Id);

            return promotion.Id;
        }

        /// <summary>
        /// Calcula o preço da promoção baseado no tipo e duração em DIAS
        /// </summary>
        private decimal CalculatePromotionPrice(PromotionType type, int durationInDays)
        {
            // Preço base por mês
            var pricePerMonth = type switch
            {
                PromotionType.FeaturedInSearch => 150m,  // R$ 150/mês
                PromotionType.CarouselHome => 200m,      // R$ 200/mês
                PromotionType.TopSearch => 250m,         // R$ 250/mês
                PromotionType.SidebarBanner => 50m,      // R$ 50/mês
                PromotionType.RegionalPromotion => 100m, // R$ 100/mês
                _ => 100m
            };

            // Cálculo proporcional por dia
            var pricePerDay = pricePerMonth / 30m;
            var totalPrice = pricePerDay * durationInDays;

            return Math.Round(totalPrice, 2);
        }
    }
}
