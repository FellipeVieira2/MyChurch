using MediatR;
using Microsoft.Extensions.Logging;
using MyChurch.Infrastructure.Utils.SES;
using MyChurch.Application.ChurchPromotion.Events;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MyChurch.Application.ChurchPromotion.EventHandlers
{
    /// <summary>
    /// Handler que envia email quando promoção é ativada
    /// </summary>
    public class PromotionActivatedEmailHandler : INotificationHandler<PromotionActivatedEvent>
    {
        private readonly IEmailService _emailService;
        private readonly ILogger<PromotionActivatedEmailHandler> _logger;

        public PromotionActivatedEmailHandler(
            IEmailService emailService,
            ILogger<PromotionActivatedEmailHandler> logger)
        {
            _emailService = emailService;
            _logger = logger;
        }

        public async Task Handle(PromotionActivatedEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                var subject = "?? Sua Promoção foi Ativada - MyChurch";
                var body = BuildEmailBody(notification);

                await _emailService.EnviarEmailAsync(
                    notification.AdminEmail,
                    subject,
                    body,
                    null
                );

                _logger.LogInformation(
                    "? Email de ativação de promoção enviado para {Email} (PromotionId: {PromotionId})",
                    notification.AdminEmail,
                    notification.PromotionId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, 
                    "? Erro ao enviar email de ativação de promoção {PromotionId}",
                    notification.PromotionId);
            }
        }

        private string BuildEmailBody(PromotionActivatedEvent evt)
        {
            var duration = (evt.EndDate - evt.StartDate).Days;
            
            return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; padding: 30px; text-align: center; border-radius: 10px 10px 0 0; }}
        .content {{ background: #f9f9f9; padding: 30px; border-radius: 0 0 10px 10px; }}
        .promotion-box {{ background: white; padding: 20px; margin: 20px 0; border-radius: 8px; border-left: 4px solid #667eea; }}
        .stats {{ display: flex; justify-content: space-around; margin: 20px 0; }}
        .stat {{ text-align: center; }}
        .stat-value {{ font-size: 24px; font-weight: bold; color: #667eea; }}
        .stat-label {{ font-size: 12px; color: #666; }}
        .button {{ display: inline-block; padding: 12px 30px; background: #667eea; color: white; text-decoration: none; border-radius: 5px; margin: 10px 0; }}
        .footer {{ text-align: center; color: #666; font-size: 12px; margin-top: 20px; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>?? Promoção Ativada!</h1>
            <p>Sua igreja já está em destaque</p>
        </div>
        
        <div class='content'>
            <p>Olá <strong>{evt.AdminName}</strong>,</p>
            
            <p>Ótimas notícias! Seu pagamento foi confirmado e sua promoção já está <strong>ATIVA</strong> no MyChurch! ??</p>
            
            <div class='promotion-box'>
                <h3 style='margin-top: 0;'>?? Detalhes da Promoção</h3>
                <p><strong>Igreja:</strong> {evt.ChurchName}</p>
                <p><strong>Tipo:</strong> {GetPromotionTypeName(evt.PromotionType)}</p>
                <p><strong>Período:</strong> {evt.StartDate:dd/MM/yyyy} até {evt.EndDate:dd/MM/yyyy} ({duration} dias)</p>
                <p><strong>Investimento:</strong> R$ {evt.AmountPaid:N2}</p>
            </div>
            
            <div class='stats'>
                <div class='stat'>
                    <div class='stat-value'>{evt.EstimatedViews:N0}</div>
                    <div class='stat-label'>Visualizações Estimadas</div>
                </div>
                <div class='stat'>
                    <div class='stat-value'>{(evt.EstimatedViews * 0.03):N0}</div>
                    <div class='stat-label'>Cliques Estimados</div>
                </div>
                <div class='stat'>
                    <div class='stat-value'>{duration}</div>
                    <div class='stat-label'>Dias Ativos</div>
                </div>
            </div>
            
            <h3>? Próximos Passos:</h3>
            <ul>
                <li>Sua igreja já aparece em destaque na busca</li>
                <li>Acompanhe as estatísticas em tempo real no painel</li>
                <li>Prepare-se para receber mais visitantes!</li>
            </ul>
            
            <center>
                <a href='https://app.mychurch.com.br/promotions/{evt.PromotionId}' class='button'>
                    ?? Ver Estatísticas da Promoção
                </a>
            </center>
            
            <p style='margin-top: 30px; color: #666; font-size: 14px;'>
                ?? <strong>Dica:</strong> Compartilhe nas redes sociais que sua igreja está em destaque para atrair ainda mais visitantes!
            </p>
        </div>
        
        <div class='footer'>
            <p>Este é um email automático. Não responda a esta mensagem.</p>
            <p>© {DateTime.UtcNow.Year} MyChurch - Sistema de Gestão de Igrejas</p>
        </div>
    </div>
</body>
</html>";
        }

        private string GetPromotionTypeName(string type)
        {
            return type switch
            {
                "FeaturedInSearch" => "?? Destaque na Busca",
                "CarouselHome" => "?? Carousel da Home",
                "TopSearch" => "?? Topo dos Resultados",
                "SidebarBanner" => "?? Banner Lateral",
                "RegionalPromotion" => "??? Promoção Regional",
                _ => type
            };
        }
    }
}
