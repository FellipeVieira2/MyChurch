using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;
using MyChurch.Domain.Enum;
using MyChurch.Domain.Exceptions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MyChurch.Application.EventPromotion.Commands.CreateEventPromotion
{
    public class CreateEventPromotionCommand : JwtMemberDto, IRequest<int>
    {
        /// <summary>
        /// ID do evento a ser promovido
        /// </summary>
        public int EventId { get; set; }
        
        /// <summary>
        /// Tipo de promoção
        /// </summary>
        public EventPromotionType Type { get; set; }
        
        /// <summary>
        /// Data de início da promoção
        /// </summary>
        public DateTime StartDate { get; set; }
        
        /// <summary>
        /// Data de término da promoção
        /// </summary>
        public DateTime EndDate { get; set; }
        
        /// <summary>
        /// Orçamento total para a promoção
        /// </summary>
        public decimal Budget { get; set; }
        
        /// <summary>
        /// Região alvo (opcional)
        /// </summary>
        public string? TargetRegion { get; set; }
        
        /// <summary>
        /// Raio em KM para promoção regional (opcional)
        /// </summary>
        public double? TargetRadiusKm { get; set; }
        
        /// <summary>
        /// Banner personalizado
        /// </summary>
        public string? CustomBannerUrl { get; set; }
        
        /// <summary>
        /// Método de pagamento
        /// </summary>
        public string PaymentMethod { get; set; } = "PIX";
    }

    public class CreateEventPromotionCommandHandler : IRequestHandler<CreateEventPromotionCommand, int>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CreateEventPromotionCommandHandler> _logger;

        public CreateEventPromotionCommandHandler(
            IUnitOfWork unitOfWork,
            ILogger<CreateEventPromotionCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<int> Handle(CreateEventPromotionCommand request, CancellationToken cancellationToken)
        {
            // 1. Buscar membro e igreja
            var member = await _unitOfWork.Members.Query()
                .Include(m => m.Church)
                .ThenInclude(c => c.Subscription)
                .ThenInclude(s => s.Plan)
                .FirstOrDefaultAsync(m => m.Id == request.UserId, cancellationToken);

            if (member == null)
                ValidationException.ThrowException("Member", "Usuário não encontrado.");

            if (member.Role != UserRole.Admin)
                ValidationException.ThrowException("Member", "Apenas administradores podem criar promoções.");

            var church = member.Church;
            if (church == null)
                ValidationException.ThrowException("Church", "Igreja não encontrada.");

            // 2. Validar plano
            var subscription = church.Subscription;
            if (subscription == null || subscription.Plan == null)
                ValidationException.ThrowException("Subscription", "Igreja não possui assinatura ativa.");

            if (!subscription.Plan.CanPromoteEvents)
                ValidationException.ThrowException("Plan", "Seu plano não permite promoção de eventos. Faça upgrade para Premium.");

            // 3. Validar evento
            var eventEntity = await _unitOfWork.Events.Query()
                .FirstOrDefaultAsync(e => e.Id == request.EventId && e.ChurchId == church.Id, cancellationToken);

            if (eventEntity == null)
                ValidationException.ThrowException("Event", "Evento não encontrado ou não pertence à sua igreja.");

            // 4. Validar datas
            if (request.StartDate < DateTime.UtcNow)
                ValidationException.ThrowException("Promotion", "Data de início não pode ser no passado.");

            if (request.EndDate <= request.StartDate)
                ValidationException.ThrowException("Promotion", "Data de término deve ser após a data de início.");

            if (request.EndDate > eventEntity.Date)
                ValidationException.ThrowException("Promotion", "Promoção não pode terminar após a data do evento.");

            // 5. Validar orçamento
            if (request.Budget < 10m)
                ValidationException.ThrowException("Promotion", "Orçamento mínimo é R$ 10,00.");

            // 6. Criar a promoção
            var promotion = new Domain.Entities.EventPromotion
            {
                EventId = request.EventId,
                ChurchId = church.Id,
                Type = request.Type,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                Budget = request.Budget,
                AmountSpent = 0,
                Status = PromotionStatus.PendingApproval,
                TargetRegion = request.TargetRegion,
                TargetRadiusKm = request.TargetRadiusKm,
                CustomBannerUrl = request.CustomBannerUrl,
                Created = DateTime.UtcNow
            };

            _unitOfWork.EventPromotions.Create(promotion);
            await _unitOfWork.CommitAsync();

            _logger.LogInformation(
                "Promoção de evento criada. EventId: {EventId}, Tipo: {Type}, Orçamento: R$ {Budget}",
                request.EventId, request.Type, request.Budget);

            return promotion.Id;
        }
    }
}
