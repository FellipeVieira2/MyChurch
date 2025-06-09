using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyChurch.Application.Dtos;
using MyChurch.Application.Webhook.Commands;
using MyChurch.Domain.Enum;
using System.Text.Json;

namespace MyChurch.Api.Web.Controllers
{
    public class WebhookController : BaseController
    {
        private readonly ILogger<WebhookController> _logger;

        public WebhookController(ILogger<WebhookController> logger)
        {
            _logger = logger;
        }

        [HttpPost("asaas")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AsaasWebhook()
        {
            try
            {
                using var reader = new StreamReader(Request.Body);
                var body = await reader.ReadToEndAsync();

                var webhook = JsonSerializer.Deserialize<AsaasWebhookEventDto>(body);

                // Exemplo de processamento:
                switch (webhook?.Event)
                {
                    case "PAYMENT_CONFIRMED":
                    case "PAYMENT_RECEIVED":
                        if (Enum.TryParse<PaymentStatus>(webhook.Payment.Status, true, out var paymentStatus))
                        {
                            await Mediator.Send(new ConfirmPaymentCommand
                            {
                                PaymentId = webhook.Payment.Id,
                                Status = paymentStatus
                            });
                        }
                        break;
                        // Outros eventos...
                }

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar webhook do Asaas");
                return StatusCode(500, "Erro ao processar webhook");
            }
        }
    }
}