//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Extensions.Logging;
//using MyChurch.Application.Webhook.Commands;
//using MyChurch.Domain.Enum;
//using MyChurch.Application.Dtos;

//namespace MyChurch.Api.Web.Controllers
//{
//    [ApiController]
//    [Route("api/[controller]")]
//    public class WebhookController : BaseController
//    {
//        private readonly ILogger<WebhookController> _logger;

//        public WebhookController(ILogger<WebhookController> logger)
//        {
//            _logger = logger;
//        }

//        /// <summary>
//        /// Recebe notificações de eventos do Asaas
//        /// </summary>
//        /// <response code="200">Webhook processado com sucesso</response>
//        /// <response code="500">Erro ao processar webhook</response>
//        [HttpPost("asaas")]
//        [ProducesResponseType(StatusCodes.Status200OK)]
//        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
//        public async Task<IActionResult> AsaasWebhook([FromBody] AsaasWebhookDto webhook)
//        {
//            try
//            {
//                switch (webhook.Event)
//                {
//                    case "PAYMENT_RECEIVED":
//                    case "PAYMENT_CONFIRMED":
//                        if (Enum.TryParse<AsaasPaymentStatus>(webhook.Payment.Status, out var paymentStatus))
//                        {
//                            await Mediator.Send(new ConfirmPaymentCommand
//                            {
//                                PaymentId = webhook.Payment.Id,
//                                Status = paymentStatus
//                            });
//                        }
//                        break;

//                    case "PAYMENT_OVERDUE":
//                        if (Enum.TryParse<AsaasSubscriptionStatus>(webhook.Payment.Status, out var subscriptionStatus))
//                        {
//                            await Mediator.Send(new UpdateSubscriptionStatusCommand
//                            {
//                                SubscriptionId = webhook.Payment.Subscription,
//                                Status = subscriptionStatus.ToString()
//                            });
//                        }
//                        break;
//                }

//                return Ok();
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Erro ao processar webhook do Asaas");
//                return StatusCode(500, "Erro ao processar webhook");
//            }
//        }
//    }
//}