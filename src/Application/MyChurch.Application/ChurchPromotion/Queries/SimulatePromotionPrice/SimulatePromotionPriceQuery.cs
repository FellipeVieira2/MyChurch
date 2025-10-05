using MediatR;
using MyChurch.Domain.Enum;
using System;

namespace MyChurch.Application.ChurchPromotion.Queries.SimulatePromotionPrice
{
    /// <summary>
    /// Simula o preço de uma promoção baseado no tipo e duração
    /// </summary>
    public class SimulatePromotionPriceQuery : IRequest<PromotionPriceSimulationDto>
    {
        /// <summary>
        /// Tipo de promoção desejada
        /// </summary>
        public PromotionType Type { get; set; }
        
        /// <summary>
        /// Número de dias que a promoção ficará ativa
        /// </summary>
        public int DurationInDays { get; set; }
        
        /// <summary>
        /// Data de início (opcional - padrão: hoje)
        /// </summary>
        public DateTime? StartDate { get; set; }
    }

    public class PromotionPriceSimulationDto
    {
        public PromotionType Type { get; set; }
        public string TypeName { get; set; }
        public string TypeDescription { get; set; }
        public int DurationInDays { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal PricePerDay { get; set; }
        public decimal PricePerWeek { get; set; }
        public decimal PricePerMonth { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        
        /// <summary>
        /// Estimativa de visualizações baseada em dados históricos
        /// </summary>
        public int EstimatedViews { get; set; }
        
        /// <summary>
        /// Estimativa de cliques (2-5% de CTR)
        /// </summary>
        public int EstimatedClicks { get; set; }
        
        /// <summary>
        /// Custo estimado por clique
        /// </summary>
        public decimal CostPerClick { get; set; }
    }

    public class SimulatePromotionPriceQueryHandler : IRequestHandler<SimulatePromotionPriceQuery, PromotionPriceSimulationDto>
    {
        public Task<PromotionPriceSimulationDto> Handle(SimulatePromotionPriceQuery request, CancellationToken cancellationToken)
        {
            var startDate = request.StartDate ?? DateTime.UtcNow;
            var endDate = startDate.AddDays(request.DurationInDays);

            // ?? Tabela de Preços por Tipo
            var (pricePerMonth, description, avgViewsPerDay) = request.Type switch
            {
                PromotionType.FeaturedInSearch => (150m, "Destaque principal na busca de igrejas", 500),
                PromotionType.CarouselHome => (200m, "Aparece no carousel da página inicial", 800),
                PromotionType.TopSearch => (250m, "Fixado no topo dos resultados", 600),
                PromotionType.SidebarBanner => (50m, "Banner lateral em páginas internas", 200),
                PromotionType.RegionalPromotion => (100m, "Promoção para região específica", 300),
                _ => (100m, "Promoção padrão", 250)
            };

            // ?? Cálculo Proporcional
            var pricePerDay = pricePerMonth / 30m;
            var totalPrice = Math.Round(pricePerDay * request.DurationInDays, 2);

            // ?? Estimativas de Performance
            var estimatedViews = (int)(avgViewsPerDay * request.DurationInDays);
            var estimatedClicks = (int)(estimatedViews * 0.03m); // 3% CTR médio
            var costPerClick = estimatedClicks > 0 ? totalPrice / estimatedClicks : 0;

            var result = new PromotionPriceSimulationDto
            {
                Type = request.Type,
                TypeName = request.Type.ToString(),
                TypeDescription = description,
                DurationInDays = request.DurationInDays,
                TotalPrice = totalPrice,
                PricePerDay = Math.Round(pricePerDay, 2),
                PricePerWeek = Math.Round(pricePerDay * 7, 2),
                PricePerMonth = pricePerMonth,
                StartDate = startDate,
                EndDate = endDate,
                EstimatedViews = estimatedViews,
                EstimatedClicks = estimatedClicks,
                CostPerClick = Math.Round(costPerClick, 2)
            };

            return Task.FromResult(result);
        }
    }
}
