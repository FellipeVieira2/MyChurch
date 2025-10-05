using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyChurch.Application.Dtos;
using MyChurch.Application.Webhook.Commands;
using MyChurch.Application.ChurchPromotion.Commands.ProcessPromotionPayment;
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

                _logger.LogInformation("📨 Webhook recebido do Asaas: {Body}", body);

                var webhook = JsonSerializer.Deserialize<AsaasWebhookEventDto>(body);

                if (webhook == null || webhook.Payment == null)
                {
                    _logger.LogWarning("Webhook inválido ou sem informações de pagamento");
                    return BadRequest("Webhook inválido");
                }

                // Processa webhook de acordo com o evento
                switch (webhook.Event)
                {
                    case "PAYMENT_CONFIRMED":
                    case "PAYMENT_RECEIVED":
                    case "PAYMENT_RECEIVED_IN_CASH":
                        _logger.LogInformation("✅ Pagamento confirmado: {PaymentId}", webhook.Payment.Id);
                        
                        // 1. Confirmar pagamento (doações, assinaturas)
                        if (Enum.TryParse<PaymentStatus>(webhook.Payment.Status, true, out var paymentStatus))
                        {
                            await Mediator.Send(new ConfirmPaymentCommand
                            {
                                PaymentId = webhook.Payment.Id,
                                Status = paymentStatus
                            });
                        }

                        // 2. Processar pagamento de promoções (ativa automaticamente)
                        await Mediator.Send(new ProcessPromotionPaymentCommand
                        {
                            TransactionId = webhook.Payment.Id,
                            PaymentStatus = webhook.Payment.Status
                        });
                        break;

                    case "PAYMENT_OVERDUE":
                        _logger.LogWarning("⚠️ Pagamento vencido: {PaymentId}", webhook.Payment.Id);
                        break;

                    case "PAYMENT_DELETED":
                    case "PAYMENT_REFUNDED":
                        _logger.LogWarning("❌ Pagamento cancelado/reembolsado: {PaymentId}", webhook.Payment.Id);
                        
                        // Rejeitar promoção se pagamento foi cancelado
                        await Mediator.Send(new ProcessPromotionPaymentCommand
                        {
                            TransactionId = webhook.Payment.Id,
                            PaymentStatus = "CANCELLED"
                        });
                        break;

                    default:
                        _logger.LogInformation("Evento não processado: {Event}", webhook.Event);
                        break;
                }

                return Ok(new { message = "Webhook processado com sucesso" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar webhook do Asaas");
                return StatusCode(500, "Erro ao processar webhook");
            }
        }
    }
}