using MediatR;
using System;

namespace MyChurch.Application.ChurchPromotion.Events
{
    /// <summary>
    /// Evento disparado quando uma promoção é ativada após confirmação de pagamento
    /// </summary>
    public class PromotionActivatedEvent : INotification
    {
        public int PromotionId { get; set; }
        public int ChurchId { get; set; }
        public string ChurchName { get; set; }
        public string AdminEmail { get; set; }
        public string AdminName { get; set; }
        public string PromotionType { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal AmountPaid { get; set; }
        public int EstimatedViews { get; set; }
    }
}
