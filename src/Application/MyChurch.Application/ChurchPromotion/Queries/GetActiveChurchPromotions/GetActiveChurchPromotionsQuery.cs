using MediatR;
using Microsoft.EntityFrameworkCore;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MyChurch.Application.ChurchPromotion.Queries.GetActiveChurchPromotions
{
    public class GetActiveChurchPromotionsQuery : IRequest<List<ChurchPromotionDto>>
    {
        /// <summary>
        /// Tipo de promoção para filtrar (opcional)
        /// </summary>
        public PromotionType? Type { get; set; }
        
        /// <summary>
        /// Região para filtrar (opcional) - Ex: "SP"
        /// </summary>
        public string? Region { get; set; }
        
        /// <summary>
        /// Número máximo de promoções a retornar
        /// </summary>
        public int? Limit { get; set; }
    }

    public class ChurchPromotionDto
    {
        public int Id { get; set; }
        public int ChurchId { get; set; }
        public string ChurchName { get; set; }
        public string? ChurchLogo { get; set; }
        public PromotionType Type { get; set; }
        public string? CustomBannerUrl { get; set; }
        public string? CustomText { get; set; }
        public int Views { get; set; }
        public int Clicks { get; set; }
        public double ClickThroughRate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

    public class GetActiveChurchPromotionsQueryHandler : IRequestHandler<GetActiveChurchPromotionsQuery, List<ChurchPromotionDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetActiveChurchPromotionsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<ChurchPromotionDto>> Handle(GetActiveChurchPromotionsQuery request, CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow;

            var query = _unitOfWork.ChurchPromotions.Query()
                .Include(p => p.Church)
                .Where(p => 
                    p.Status == PromotionStatus.Active &&
                    p.StartDate <= now &&
                    p.EndDate >= now);

            // Filtrar por tipo se especificado
            if (request.Type.HasValue)
            {
                query = query.Where(p => p.Type == request.Type.Value);
            }

            // Filtrar por região se especificado
            if (!string.IsNullOrEmpty(request.Region))
            {
                query = query.Where(p => 
                    p.TargetRegion == null || // Promoções nacionais
                    p.TargetRegion.Contains(request.Region)); // Promoções regionais
            }

            // Ordenar por número de views (menos exibidas aparecem primeiro para balancear)
            query = query.OrderBy(p => p.Views)
                         .ThenByDescending(p => p.Created);

            // Limitar resultados se especificado
            if (request.Limit.HasValue && request.Limit.Value > 0)
            {
                query = query.Take(request.Limit.Value);
            }

            var promotions = await query.ToListAsync(cancellationToken);

            return promotions.Select(p => new ChurchPromotionDto
            {
                Id = p.Id,
                ChurchId = p.ChurchId,
                ChurchName = p.Church.Name,
                ChurchLogo = p.Church.LogoFileName,
                Type = p.Type,
                CustomBannerUrl = p.CustomBannerUrl,
                CustomText = p.CustomText,
                Views = p.Views,
                Clicks = p.Clicks,
                ClickThroughRate = p.ClickThroughRate,
                StartDate = p.StartDate,
                EndDate = p.EndDate
            }).ToList();
        }
    }
}
