using MyChurch.Domain.Enum;
using System;

namespace MyChurch.Domain.Entities
{
    /// <summary>
    /// Promoção de um evento específico
    /// </summary>
    public class EventPromotion
    {
        public int Id { get; set; }
        
        /// <summary>
        /// Evento sendo promovido
        /// </summary>
        public int EventId { get; set; }
        public Event Event { get; set; }
        
        /// <summary>
        /// Igreja dona do evento (para validações)
        /// </summary>
        public int ChurchId { get; set; }
        public Church Church { get; set; }
        
        /// <summary>
        /// Tipo de promoção do evento
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
        /// Orçamento total da promoção
        /// </summary>
        public decimal Budget { get; set; }
        
        /// <summary>
        /// Valor já gasto (baseado em cliques/visualizações)
        /// </summary>
        public decimal AmountSpent { get; set; }
        
        /// <summary>
        /// Status da promoção
        /// </summary>
        public PromotionStatus Status { get; set; }
        
        /// <summary>
        /// Região alvo (opcional)
        /// </summary>
        public string? TargetRegion { get; set; }
        
        /// <summary>
        /// Raio em KM para promoção regional (se aplicável)
        /// </summary>
        public double? TargetRadiusKm { get; set; }
        
        /// <summary>
        /// Banner personalizado do evento
        /// </summary>
        public string? CustomBannerUrl { get; set; }
        
        /// <summary>
        /// Número de visualizações
        /// </summary>
        public int Views { get; set; }
        
        /// <summary>
        /// Número de cliques
        /// </summary>
        public int Clicks { get; set; }
        
        /// <summary>
        /// Número de pessoas que confirmaram presença via promoção
        /// </summary>
        public int Conversions { get; set; }
        
        /// <summary>
        /// ID do pagamento relacionado
        /// </summary>
        public int? PaymentId { get; set; }
        public Payment? Payment { get; set; }
        
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
        
        /// <summary>
        /// Taxa de conversão de cliques
        /// </summary>
        public double ClickThroughRate => Views > 0 ? (double)Clicks / Views * 100 : 0;
        
        /// <summary>
        /// Taxa de conversão para presença
        /// </summary>
        public double ConversionRate => Clicks > 0 ? (double)Conversions / Clicks * 100 : 0;
        
        /// <summary>
        /// Custo por clique
        /// </summary>
        public decimal CostPerClick => Clicks > 0 ? AmountSpent / Clicks : 0;
        
        /// <summary>
        /// Verifica se ainda há orçamento disponível
        /// </summary>
        public bool HasBudgetAvailable => AmountSpent < Budget;
        
        /// <summary>
        /// Registra uma visualização
        /// </summary>
        public void RegisterView()
        {
            Views++;
            Updated = DateTime.UtcNow;
        }
        
        /// <summary>
        /// Registra um clique (cobra do orçamento)
        /// </summary>
        public void RegisterClick(decimal costPerClick = 0.50m)
        {
            if (!HasBudgetAvailable)
                throw new InvalidOperationException("Orçamento da promoção esgotado.");
            
            Clicks++;
            AmountSpent += costPerClick;
            Updated = DateTime.UtcNow;
            
            // Se esgotou o orçamento, pausa automaticamente
            if (AmountSpent >= Budget)
            {
                Status = PromotionStatus.Paused;
            }
        }
        
        /// <summary>
        /// Registra uma conversão (pessoa confirmou presença)
        /// </summary>
        public void RegisterConversion()
        {
            Conversions++;
            Updated = DateTime.UtcNow;
        }
    }
}
