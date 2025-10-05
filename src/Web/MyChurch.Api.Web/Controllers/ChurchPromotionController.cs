using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyChurch.Application.ChurchPromotion.Commands.CreateChurchPromotion;
using MyChurch.Application.ChurchPromotion.Commands.TrackPromotionClick;
using MyChurch.Application.ChurchPromotion.Commands.TrackPromotionView;
using MyChurch.Application.ChurchPromotion.Queries.GetActiveChurchPromotions;
using MyChurch.Application.ChurchPromotion.Queries.GetMyChurchPromotions;
using MyChurch.Application.ChurchPromotion.Queries.SimulatePromotionPrice;
using MyChurch.Application.ChurchPromotion.Queries.GetPromotionsDashboard;
using MyChurch.Application.ChurchPromotion.Queries.GetPromotionAnalytics;
using MyChurch.Domain.Enum;

namespace MyChurch.Api.Web.Controllers
{
    /// <summary>
    /// Gerenciamento de promoções de igrejas
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ChurchPromotionController : BaseController
    {
        /// <summary>
        /// ?? Simula o preço de uma promoção ANTES de criar
        /// </summary>
        /// <remarks>
        /// Use este endpoint para mostrar o preço ao usuário antes de ele confirmar a compra.
        /// 
        /// Exemplo: "7 dias de promoção custará R$ 35,00"
        /// 
        /// **Tipos de promoção:**
        /// - FeaturedInSearch (0): R$ 150/mês = R$ 5,00/dia
        /// - CarouselHome (1): R$ 200/mês = R$ 6,67/dia
        /// - TopSearch (2): R$ 250/mês = R$ 8,33/dia
        /// - SidebarBanner (3): R$ 50/mês = R$ 1,67/dia
        /// - RegionalPromotion (4): R$ 100/mês = R$ 3,33/dia
        /// </remarks>
        [HttpPost("simulate")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PromotionPriceSimulationDto))]
        public async Task<IActionResult> SimulatePrice([FromBody] SimulatePromotionPriceQuery query)
        {
            var simulation = await Mediator.Send(query);
            return Ok(simulation);
        }

        /// <summary>
        /// Cria uma nova promoção para a igreja
        /// </summary>
        /// <remarks>
        /// ?? **Apenas igrejas com plano PREMIUM podem criar promoções.**
        /// 
        /// **Nova forma de uso (com duração em dias):**
        /// ```json
        /// {
        ///   "type": 0,
        ///   "durationInDays": 7,
        ///   "targetRegion": "SP",
        ///   "customText": "Venha conhecer nossa igreja!",
        ///   "paymentMethod": "PIX"
        /// }
        /// ```
        /// 
        /// O sistema calculará automaticamente:
        /// - `startDate` = hoje
        /// - `endDate` = hoje + 7 dias
        /// - `totalPrice` = (R$ 150 / 30 dias) * 7 = R$ 35,00
        /// </remarks>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(int))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> CreatePromotion([FromBody] CreateChurchPromotionCommand command)
        {
            var cmd = AuthorizationRequestCreate<CreateChurchPromotionCommand>();
            cmd.Type = command.Type;
            cmd.DurationInDays = command.DurationInDays;
            cmd.StartDate = command.StartDate;
            cmd.TargetRegion = command.TargetRegion;
            cmd.CustomBannerUrl = command.CustomBannerUrl;
            cmd.CustomText = command.CustomText;
            cmd.PaymentMethod = command.PaymentMethod;
            cmd.CreditCard = command.CreditCard;
            cmd.CreditCardHolderInfo = command.CreditCardHolderInfo;
            cmd.CreditCardInfoId = command.CreditCardInfoId;

            var promotionId = await Mediator.Send(cmd);
            return Ok(new { promotionId, message = "Promoção criada com sucesso! Aguardando aprovação." });
        }

        /// <summary>
        /// Busca promoções ativas para exibição pública
        /// </summary>
        /// <param name="type">Tipo de promoção (opcional)</param>
        /// <param name="region">Região para filtrar (opcional) - Ex: "SP"</param>
        /// <param name="limit">Número máximo de resultados (padrão: 10)</param>
        [HttpGet("active")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<ChurchPromotionDto>))]
        public async Task<IActionResult> GetActivePromotions(
            [FromQuery] PromotionType? type = null,
            [FromQuery] string? region = null,
            [FromQuery] int limit = 10)
        {
            var query = new GetActiveChurchPromotionsQuery
            {
                Type = type,
                Region = region,
                Limit = limit
            };

            var promotions = await Mediator.Send(query);
            return Ok(promotions);
        }

        /// <summary>
        /// Busca as promoções da minha igreja
        /// </summary>
        /// <param name="status">Filtrar por status (opcional)</param>
        /// <param name="includeExpired">Incluir promoções expiradas?</param>
        [HttpGet("my-promotions")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<MyChurchPromotionDto>))]
        public async Task<IActionResult> GetMyPromotions(
            [FromQuery] PromotionStatus? status = null,
            [FromQuery] bool includeExpired = false)
        {
            var query = AuthorizationRequestCreate<GetMyChurchPromotionsQuery>();
            query.Status = status;
            query.IncludeExpired = includeExpired;

            var promotions = await Mediator.Send(query);
            return Ok(promotions);
        }

        /// <summary>
        /// Registra uma visualização de promoção (usado internamente pelo frontend)
        /// </summary>
        [HttpPost("{promotionId}/view")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> TrackView(int promotionId)
        {
            var command = new TrackChurchPromotionViewCommand { PromotionId = promotionId };
            await Mediator.Send(command);
            return Ok();
        }

        /// <summary>
        /// Registra um clique em uma promoção
        /// </summary>
        [HttpPost("{promotionId}/click")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> TrackClick(int promotionId)
        {
            var command = new TrackChurchPromotionClickCommand { PromotionId = promotionId };
            await Mediator.Send(command);
            return Ok();
        }

        /// <summary>
        /// Pausa uma promoção ativa
        /// </summary>
        [HttpPost("{promotionId}/pause")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PausePromotion(int promotionId)
        {
            // TODO: Implementar PauseChurchPromotionCommand
            return Ok(new { message = "Funcionalidade em desenvolvimento" });
        }

        /// <summary>
        /// Retoma uma promoção pausada
        /// </summary>
        [HttpPost("{promotionId}/resume")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ResumePromotion(int promotionId)
        {
            // TODO: Implementar ResumeChurchPromotionCommand
            return Ok(new { message = "Funcionalidade em desenvolvimento" });
        }

        /// <summary>
        /// ?? Dashboard completo de analytics de promoções da igreja
        /// </summary>
        /// <remarks>
        /// Retorna visão geral de TODAS as promoções da igreja:
        /// - Promoções ativas
        /// - Promoções concluídas
        /// - Métricas consolidadas
        /// - Top performers
        /// - Estatísticas por tipo
        /// 
        /// **Exemplo de resposta:**
        /// ```json
        /// {
        ///   "totalActivePromotions": 2,
        ///   "totalInvested": 300.00,
        ///   "totalViews": 5420,
        ///   "totalClicks": 163,
        ///   "averageClickThroughRate": 3.01,
        ///   "activePromotions": [...],
        ///   "completedPromotions": [...],
        ///   "statsByType": [...]
        /// }
        /// ```
        /// </remarks>
        [HttpGet("dashboard")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetDashboard()
        {
            var query = AuthorizationRequestCreate<GetPromotionsDashboardQuery>();
            var dashboard = await Mediator.Send(query);
            return Ok(dashboard);
        }

        /// <summary>
        /// ?? Analytics detalhados de uma promoção específica
        /// </summary>
        /// <remarks>
        /// Retorna métricas detalhadas de UMA promoção:
        /// - Views e clicks totais
        /// - CTR (Click-Through Rate)
        /// - Custo por view/click
        /// - Performance diária
        /// - Projeções de resultados finais
        /// 
        /// **Exemplo de resposta:**
        /// ```json
        /// {
        ///   "promotionId": 1,
        ///   "totalViews": 3500,
        ///   "totalClicks": 105,
        ///   "clickThroughRate": 3.0,
        ///   "costPerView": 0.04,
        ///   "costPerClick": 1.43,
        ///   "daysActive": 15,
        ///   "daysRemaining": 15,
        ///   "projectedTotalViews": 7000,
        ///   "projectedTotalClicks": 210,
        ///   "dailyMetrics": [...]
        /// }
        /// ```
        /// </remarks>
        [HttpGet("{promotionId}/analytics")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAnalytics([FromRoute] int promotionId)
        {
            var query = AuthorizationRequestCreate<GetPromotionAnalyticsQuery>();
            query.PromotionId = promotionId;
            
            var analytics = await Mediator.Send(query);
            
            if (analytics == null)
                return NotFound(new { message = "Promoção não encontrada ou você não tem permissão para visualizá-la" });
            
            return Ok(analytics);
        }
    }
}
