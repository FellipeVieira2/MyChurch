using MyChurch.Domain.Enum;
using System;

namespace MyChurch.Domain.Entities
{
    /// <summary>
    /// Promoção de uma igreja (aparecer em destaque, carousel, etc)
    /// </summary>
    public class ChurchPromotion
    {
        public int Id { get; set; }
        
        /// <summary>
        /// Igreja sendo promovida
        /// </summary>
        public int ChurchId { get; set; }
        public Church Church { get; set; }
        
        /// <summary>
        /// Tipo de promoção (Destaque, Carousel, etc)
        /// </summary>
        public PromotionType Type { get; set; }
        
        /// <summary>
        /// Data de início da promoção
        /// </summary>
        public DateTime StartDate { get; set; }
        
        /// <summary>
        /// Data de término da promoção
        /// </summary>
        public DateTime EndDate { get; set; }
        
        /// <summary>
        /// Valor pago pela promoção
        /// </summary>
        public decimal AmountPaid { get; set; }
        
        /// <summary>
        /// Status atual da promoção
        /// </summary>
        public PromotionStatus Status { get; set; }
        
        /// <summary>
        /// Região alvo (opcional - para promoções regionais)
        /// Formato: "Estado" ou "Estado,Cidade" ou null para nacional
        /// </summary>
        public string? TargetRegion { get; set; }
        
        /// <summary>
        /// URL da imagem/banner personalizado (opcional)
        /// </summary>
        public string? CustomBannerUrl { get; set; }
        
        /// <summary>
        /// Texto personalizado para a promoção (opcional)
        /// </summary>
        public string? CustomText { get; set; }
        
        /// <summary>
        /// Número de visualizações da promoção
        /// </summary>
        public int Views { get; set; }
        
        /// <summary>
        /// Número de cliques na promoção
        /// </summary>
        public int Clicks { get; set; }
        
        /// <summary>
        /// ID do pagamento relacionado
        /// </summary>
        public int? PaymentId { get; set; }
        public Payment? Payment { get; set; }
        
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
        
        /// <summary>
        /// Calcula a taxa de conversão (cliques/visualizações)
        /// </summary>
        public double ClickThroughRate => Views > 0 ? (double)Clicks / Views * 100 : 0;
        
        /// <summary>
        /// Verifica se a promoção está ativa no momento
        /// </summary>
        public bool IsActiveNow => 
            Status == PromotionStatus.Active && 
            DateTime.UtcNow >= StartDate && 
            DateTime.UtcNow <= EndDate;
        
        /// <summary>
        /// Registra uma visualização
        /// </summary>
        public void RegisterView()
        {
            Views++;
            Updated = DateTime.UtcNow;
        }
        
        /// <summary>
        /// Registra um clique
        /// </summary>
        public void RegisterClick()
        {
            Clicks++;
            Updated = DateTime.UtcNow;
        }
        
        /// <summary>
        /// Pausa a promoção
        /// </summary>
        public void Pause()
        {
            if (Status != PromotionStatus.Active)
                throw new InvalidOperationException("Apenas promoções ativas podem ser pausadas.");
            
            Status = PromotionStatus.Paused;
            Updated = DateTime.UtcNow;
        }
        
        /// <summary>
        /// Retoma uma promoção pausada
        /// </summary>
        public void Resume()
        {
            if (Status != PromotionStatus.Paused)
                throw new InvalidOperationException("Apenas promoções pausadas podem ser retomadas.");
            
            Status = PromotionStatus.Active;
            Updated = DateTime.UtcNow;
        }
        
        /// <summary>
        /// Finaliza a promoção
        /// </summary>
        public void Complete()
        {
            Status = PromotionStatus.Completed;
            Updated = DateTime.UtcNow;
        }
    }
}
