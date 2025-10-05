using System;

namespace MyChurch.Domain.Enum
{
    public enum PromotionType
    {
        /// <summary>
        /// Destaque principal na busca de igrejas
        /// </summary>
        FeaturedInSearch = 0,
        
        /// <summary>
        /// Aparece no carousel da home
        /// </summary>
        CarouselHome = 1,
        
        /// <summary>
        /// Fixa no topo dos resultados de busca
        /// </summary>
        TopSearch = 2,
        
        /// <summary>
        /// Banner lateral
        /// </summary>
        SidebarBanner = 3,
        
        /// <summary>
        /// Promoção regional (aparece para usuários próximos)
        /// </summary>
        RegionalPromotion = 4
    }

    public enum PromotionStatus
    {
        /// <summary>
        /// Aguardando aprovação da equipe
        /// </summary>
        PendingApproval = 0,
        
        /// <summary>
        /// Ativa e sendo exibida
        /// </summary>
        Active = 1,
        
        /// <summary>
        /// Pausada temporariamente
        /// </summary>
        Paused = 2,
        
        /// <summary>
        /// Finalizada (expirou ou foi cancelada)
        /// </summary>
        Completed = 3,
        
        /// <summary>
        /// Rejeitada pela equipe
        /// </summary>
        Rejected = 4
    }

    public enum EventPromotionType
    {
        /// <summary>
        /// Destaque na lista de eventos
        /// </summary>
        FeaturedEvent = 0,
        
        /// <summary>
        /// Aparece no carousel de eventos
        /// </summary>
        CarouselEvent = 1,
        
        /// <summary>
        /// Banner no feed da igreja
        /// </summary>
        ChurchFeedBanner = 2,
        
        /// <summary>
        /// Notificação push para membros próximos
        /// </summary>
        PushNotification = 3
    }
}
