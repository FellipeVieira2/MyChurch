using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyChurch.Application.EventPromotion.Commands.CreateEventPromotion;

namespace MyChurch.Api.Web.Controllers
{
    /// <summary>
    /// Gerenciamento de promoções de eventos
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class EventPromotionController : BaseController
    {
        /// <summary>
        /// Cria uma nova promoção para um evento
        /// </summary>
        /// <remarks>
        /// Apenas igrejas com plano Premium podem promover eventos.
        /// 
        /// Tipos de promoção disponíveis:
        /// - FeaturedEvent (0): Destaque na lista de eventos
        /// - CarouselEvent (1): Aparece no carousel de eventos
        /// - ChurchFeedBanner (2): Banner no feed da igreja
        /// - PushNotification (3): Notificação push para membros próximos
        /// 
        /// O orçamento será consumido conforme os cliques (R$ 0,50 por clique).
        /// </remarks>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(int))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> CreateEventPromotion([FromBody] CreateEventPromotionCommand command)
        {
            var cmd = AuthorizationRequestCreate<CreateEventPromotionCommand>();
            cmd.EventId = command.EventId;
            cmd.Type = command.Type;
            cmd.StartDate = command.StartDate;
            cmd.EndDate = command.EndDate;
            cmd.Budget = command.Budget;
            cmd.TargetRegion = command.TargetRegion;
            cmd.TargetRadiusKm = command.TargetRadiusKm;
            cmd.CustomBannerUrl = command.CustomBannerUrl;
            cmd.PaymentMethod = command.PaymentMethod;

            var promotionId = await Mediator.Send(cmd);
            return Ok(new { 
                promotionId, 
                message = "Promoção de evento criada com sucesso! Aguardando aprovação.",
                estimatedClicks = (int)(command.Budget / 0.50m) // Estimativa de cliques com R$ 0,50 cada
            });
        }

        /// <summary>
        /// Busca as promoções de eventos da minha igreja
        /// </summary>
        [HttpGet("my-promotions")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMyEventPromotions()
        {
            // TODO: Implementar GetMyEventPromotionsQuery
            return Ok(new { message = "Funcionalidade em desenvolvimento" });
        }

        /// <summary>
        /// Busca detalhes de uma promoção de evento
        /// </summary>
        [HttpGet("{promotionId}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetEventPromotion(int promotionId)
        {
            // TODO: Implementar GetEventPromotionByIdQuery
            return Ok(new { message = "Funcionalidade em desenvolvimento" });
        }

        /// <summary>
        /// Registra uma visualização da promoção de evento
        /// </summary>
        [HttpPost("{promotionId}/view")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> TrackView(int promotionId)
        {
            // TODO: Implementar TrackEventPromotionViewCommand
            return Ok();
        }

        /// <summary>
        /// Registra um clique na promoção (consome do orçamento)
        /// </summary>
        [HttpPost("{promotionId}/click")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> TrackClick(int promotionId)
        {
            // TODO: Implementar TrackEventPromotionClickCommand (cobra R$ 0,50 do orçamento)
            return Ok();
        }

        /// <summary>
        /// Registra uma conversão (pessoa confirmou presença)
        /// </summary>
        [HttpPost("{promotionId}/conversion")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> TrackConversion(int promotionId)
        {
            // TODO: Implementar TrackEventPromotionConversionCommand
            return Ok();
        }

        /// <summary>
        /// Pausa uma promoção de evento
        /// </summary>
        [HttpPost("{promotionId}/pause")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> PausePromotion(int promotionId)
        {
            // TODO: Implementar PauseEventPromotionCommand
            return Ok(new { message = "Funcionalidade em desenvolvimento" });
        }
    }
}
