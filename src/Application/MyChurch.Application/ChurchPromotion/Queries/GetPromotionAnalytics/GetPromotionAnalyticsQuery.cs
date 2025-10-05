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

namespace MyChurch.Application.ChurchPromotion.Queries.GetPromotionAnalytics
{
    /// <summary>
    /// Busca analytics detalhados de uma promoção específica
    /// </summary>
    public class GetPromotionAnalyticsQuery : JwtMemberDto, IRequest<PromotionAnalyticsDto>
    {
        public int PromotionId { get; set; }
    }

    public class PromotionAnalyticsDto
    {
        public int PromotionId { get; set; }
        public string ChurchName { get; set; }
        public string PromotionType { get; set; }
        public string Status { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal AmountPaid { get; set; }
        
        // Métricas
        public int TotalViews { get; set; }
        public int TotalClicks { get; set; }
        public double ClickThroughRate { get; set; } // CTR %
        public decimal CostPerView { get; set; }
        public decimal CostPerClick { get; set; }
        
        // Performance
        public int DaysActive { get; set; }
        public int DaysRemaining { get; set; }
        public double AverageViewsPerDay { get; set; }
        public double AverageClicksPerDay { get; set; }
        
        // Projeções
        public int ProjectedTotalViews { get; set; }
        public int ProjectedTotalClicks { get; set; }
        
        // Histórico diário
        public List<DailyMetricDto> DailyMetrics { get; set; } = new();
    }

    public class DailyMetricDto
    {
        public DateTime Date { get; set; }
        public int Views { get; set; }
        public int Clicks { get; set; }
        public double ClickThroughRate { get; set; }
    }

    public class GetPromotionAnalyticsQueryHandler : IRequestHandler<GetPromotionAnalyticsQuery, PromotionAnalyticsDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetPromotionAnalyticsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PromotionAnalyticsDto> Handle(GetPromotionAnalyticsQuery request, CancellationToken cancellationToken)
        {
            var promotion = await _unitOfWork.ChurchPromotions.Query()
                .Include(p => p.Church)
                .FirstOrDefaultAsync(p => p.Id == request.PromotionId, cancellationToken);

            if (promotion == null)
                return null;

            // Verificar se o usuário tem permissão (admin da igreja)
            var member = await _unitOfWork.Members.Query()
                .FirstOrDefaultAsync(m => m.Id == request.UserId && m.ChurchId == promotion.ChurchId, cancellationToken);

            if (member == null || member.Role != UserRole.Admin)
                return null;

            var now = DateTime.UtcNow;
            var daysActive = (now - promotion.StartDate).Days;
            if (daysActive < 1) daysActive = 1; // Evita divisão por zero

            var totalDuration = (promotion.EndDate - promotion.StartDate).Days;
            var daysRemaining = (promotion.EndDate - now).Days;
            if (daysRemaining < 0) daysRemaining = 0;

            var avgViewsPerDay = promotion.Views / (double)daysActive;
            var avgClicksPerDay = promotion.Clicks / (double)daysActive;

            var projectedTotalViews = promotion.Status == PromotionStatus.Active
                ? promotion.Views + (int)(avgViewsPerDay * daysRemaining)
                : promotion.Views;

            var projectedTotalClicks = promotion.Status == PromotionStatus.Active
                ? promotion.Clicks + (int)(avgClicksPerDay * daysRemaining)
                : promotion.Clicks;

            var ctr = promotion.Views > 0 ? (promotion.Clicks / (double)promotion.Views) * 100 : 0;

            var result = new PromotionAnalyticsDto
            {
                PromotionId = promotion.Id,
                ChurchName = promotion.Church.Name,
                PromotionType = promotion.Type.ToString(),
                Status = promotion.Status.ToString(),
                StartDate = promotion.StartDate,
                EndDate = promotion.EndDate,
                AmountPaid = promotion.AmountPaid,
                
                TotalViews = promotion.Views,
                TotalClicks = promotion.Clicks,
                ClickThroughRate = Math.Round(ctr, 2),
                CostPerView = promotion.Views > 0 ? Math.Round(promotion.AmountPaid / promotion.Views, 2) : 0,
                CostPerClick = promotion.Clicks > 0 ? Math.Round(promotion.AmountPaid / promotion.Clicks, 2) : 0,
                
                DaysActive = daysActive,
                DaysRemaining = daysRemaining,
                AverageViewsPerDay = Math.Round(avgViewsPerDay, 1),
                AverageClicksPerDay = Math.Round(avgClicksPerDay, 1),
                
                ProjectedTotalViews = projectedTotalViews,
                ProjectedTotalClicks = projectedTotalClicks,
                
                // TODO: Implementar histórico diário real (requer tabela de tracking)
                DailyMetrics = GenerateMockDailyMetrics(promotion.StartDate, now, avgViewsPerDay, avgClicksPerDay)
            };

            return result;
        }

        private List<DailyMetricDto> GenerateMockDailyMetrics(DateTime startDate, DateTime endDate, double avgViews, double avgClicks)
        {
            var metrics = new List<DailyMetricDto>();
            var currentDate = startDate;

            while (currentDate <= endDate && currentDate <= DateTime.UtcNow)
            {
                var random = new Random(currentDate.GetHashCode());
                var views = (int)(avgViews * (0.8 + random.NextDouble() * 0.4)); // ±20% variação
                var clicks = (int)(avgClicks * (0.8 + random.NextDouble() * 0.4));
                
                metrics.Add(new DailyMetricDto
                {
                    Date = currentDate,
                    Views = views,
                    Clicks = clicks,
                    ClickThroughRate = views > 0 ? Math.Round((clicks / (double)views) * 100, 2) : 0
                });

                currentDate = currentDate.AddDays(1);
            }

            return metrics;
        }
    }
}
