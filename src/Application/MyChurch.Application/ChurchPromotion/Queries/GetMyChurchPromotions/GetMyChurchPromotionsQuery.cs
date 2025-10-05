using MediatR;
using Microsoft.EntityFrameworkCore;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Enum;
using MyChurch.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MyChurch.Application.ChurchPromotion.Queries.GetMyChurchPromotions
{
    public class GetMyChurchPromotionsQuery : JwtMemberDto, IRequest<List<MyChurchPromotionDto>>
    {
        /// <summary>
        /// Filtrar por status (opcional)
        /// </summary>
        public PromotionStatus? Status { get; set; }
        
        /// <summary>
        /// Incluir promoções expiradas?
        /// </summary>
        public bool IncludeExpired { get; set; } = false;
    }

    public class MyChurchPromotionDto
    {
        public int Id { get; set; }
        public PromotionType Type { get; set; }
        public string TypeName => Type.ToString();
        public PromotionStatus Status { get; set; }
        public string StatusName => Status.ToString();
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal AmountPaid { get; set; }
        public int Views { get; set; }
        public int Clicks { get; set; }
        public double ClickThroughRate { get; set; }
        public string? TargetRegion { get; set; }
        public string? CustomBannerUrl { get; set; }
        public string? CustomText { get; set; }
        public bool IsActive { get; set; }
        public int DaysRemaining { get; set; }
        public DateTime Created { get; set; }
    }

    public class GetMyChurchPromotionsQueryHandler : IRequestHandler<GetMyChurchPromotionsQuery, List<MyChurchPromotionDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetMyChurchPromotionsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<MyChurchPromotionDto>> Handle(GetMyChurchPromotionsQuery request, CancellationToken cancellationToken)
        {
            var member = await _unitOfWork.Members.Query()
                .FirstOrDefaultAsync(m => m.Id == request.UserId, cancellationToken);

            if (member == null)
                ValidationException.ThrowException("Member", "Usuário não encontrado.");

            var query = _unitOfWork.ChurchPromotions.Query()
                .Where(p => p.ChurchId == member.ChurchId);

            // Filtrar por status se especificado
            if (request.Status.HasValue)
            {
                query = query.Where(p => p.Status == request.Status.Value);
            }

            // Excluir expiradas se configurado
            if (!request.IncludeExpired)
            {
                var now = DateTime.UtcNow;
                query = query.Where(p => p.EndDate >= now);
            }

            var promotions = await query
                .OrderByDescending(p => p.Created)
                .ToListAsync(cancellationToken);

            return promotions.Select(p => new MyChurchPromotionDto
            {
                Id = p.Id,
                Type = p.Type,
                Status = p.Status,
                StartDate = p.StartDate,
                EndDate = p.EndDate,
                AmountPaid = p.AmountPaid,
                Views = p.Views,
                Clicks = p.Clicks,
                ClickThroughRate = p.ClickThroughRate,
                TargetRegion = p.TargetRegion,
                CustomBannerUrl = p.CustomBannerUrl,
                CustomText = p.CustomText,
                IsActive = p.IsActiveNow,
                DaysRemaining = (p.EndDate - DateTime.UtcNow).Days,
                Created = p.Created
            }).ToList();
        }
    }
}
