using MediatR;
using Microsoft.EntityFrameworkCore;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MyChurch.Application.ChurchPromotion.Queries.GetPromotionsDashboard
{
    /// <summary>
    /// Dashboard geral com todas as promoções da igreja e métricas consolidadas
    /// </summary>
    public class GetPromotionsDashboardQuery : JwtMemberDto, IRequest<PromotionsDashboardDto>
    {
    }

    public class PromotionsDashboardDto
    {
        // Resumo Geral
        public int TotalActivePromotions { get; set; }
        public int TotalCompletedPromotions { get; set; }
        public decimal TotalInvested { get; set; }
        public int TotalViews { get; set; }
        public int TotalClicks { get; set; }
        public double AverageClickThroughRate { get; set; }
        
        // Promoções Ativas
        public List<PromotionSummaryDto> ActivePromotions { get; set; } = new();
        
        // Promoções Concluídas (últimas 10)
        public List<PromotionSummaryDto> CompletedPromotions { get; set; } = new();
        
        // Top Performers
        public PromotionSummaryDto BestPerformingPromotion { get; set; }
        
        // Estatísticas por Tipo
        public List<PromotionTypeStatsDto> StatsByType { get; set; } = new();
    }

    public class PromotionSummaryDto
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int DaysRemaining { get; set; }
        public decimal AmountPaid { get; set; }
        public int Views { get; set; }
        public int Clicks { get; set; }
        public double ClickThroughRate { get; set; }
        public decimal CostPerClick { get; set; }
    }

    public class PromotionTypeStatsDto
    {
        public string Type { get; set; }
        public int Count { get; set; }
        public decimal TotalInvested { get; set; }
        public int TotalViews { get; set; }
        public int TotalClicks { get; set; }
        public double AverageCTR { get; set; }
    }

    public class GetPromotionsDashboardQueryHandler : IRequestHandler<GetPromotionsDashboardQuery, PromotionsDashboardDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetPromotionsDashboardQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PromotionsDashboardDto> Handle(GetPromotionsDashboardQuery request, CancellationToken cancellationToken)
        {
            var member = await _unitOfWork.Members.Query()
                .FirstOrDefaultAsync(m => m.Id == request.UserId, cancellationToken);

            if (member == null || member.Role != UserRole.Admin)
                return null;

            var allPromotions = await _unitOfWork.ChurchPromotions.Query()
                .Where(p => p.ChurchId == member.ChurchId)
                .OrderByDescending(p => p.Created)
                .ToListAsync(cancellationToken);

            var activePromotions = allPromotions
                .Where(p => p.Status == PromotionStatus.Active)
                .ToList();

            var completedPromotions = allPromotions
                .Where(p => p.Status == PromotionStatus.Completed)
                .Take(10)
                .ToList();

            var totalViews = allPromotions.Sum(p => p.Views);
            var totalClicks = allPromotions.Sum(p => p.Clicks);
            var avgCTR = totalViews > 0 ? (totalClicks / (double)totalViews) * 100 : 0;

            var bestPromotion = allPromotions
                .Where(p => p.Clicks > 0)
                .OrderByDescending(p => p.Clicks / (double)p.Views)
                .FirstOrDefault();

            var statsByType = allPromotions
                .GroupBy(p => p.Type)
                .Select(g => new PromotionTypeStatsDto
                {
                    Type = g.Key.ToString(),
                    Count = g.Count(),
                    TotalInvested = g.Sum(p => p.AmountPaid),
                    TotalViews = g.Sum(p => p.Views),
                    TotalClicks = g.Sum(p => p.Clicks),
                    AverageCTR = g.Sum(p => p.Views) > 0 
                        ? Math.Round((g.Sum(p => p.Clicks) / (double)g.Sum(p => p.Views)) * 100, 2) 
                        : 0
                })
                .ToList();

            var result = new PromotionsDashboardDto
            {
                TotalActivePromotions = activePromotions.Count,
                TotalCompletedPromotions = completedPromotions.Count,
                TotalInvested = allPromotions.Sum(p => p.AmountPaid),
                TotalViews = totalViews,
                TotalClicks = totalClicks,
                AverageClickThroughRate = Math.Round(avgCTR, 2),
                
                ActivePromotions = activePromotions.Select(MapToSummary).ToList(),
                CompletedPromotions = completedPromotions.Select(MapToSummary).ToList(),
                BestPerformingPromotion = bestPromotion != null ? MapToSummary(bestPromotion) : null,
                StatsByType = statsByType
            };

            return result;
        }

        private PromotionSummaryDto MapToSummary(Domain.Entities.ChurchPromotion promotion)
        {
            var now = DateTime.UtcNow;
            var daysRemaining = (promotion.EndDate - now).Days;
            if (daysRemaining < 0) daysRemaining = 0;

            var ctr = promotion.Views > 0 ? (promotion.Clicks / (double)promotion.Views) * 100 : 0;
            var costPerClick = promotion.Clicks > 0 ? promotion.AmountPaid / promotion.Clicks : 0;

            return new PromotionSummaryDto
            {
                Id = promotion.Id,
                Type = promotion.Type.ToString(),
                Status = promotion.Status.ToString(),
                StartDate = promotion.StartDate,
                EndDate = promotion.EndDate,
                DaysRemaining = daysRemaining,
                AmountPaid = promotion.AmountPaid,
                Views = promotion.Views,
                Clicks = promotion.Clicks,
                ClickThroughRate = Math.Round(ctr, 2),
                CostPerClick = Math.Round(costPerClick, 2)
            };
        }
    }
}
