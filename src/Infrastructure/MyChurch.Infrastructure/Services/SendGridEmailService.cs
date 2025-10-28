using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MyChurch.Infrastructure.Utils.SES;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace MyChurch.Infrastructure.Services
{
    /// <summary>
    /// Implementação do serviço de email usando SendGrid
    /// </summary>
    public class SendGridEmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<SendGridEmailService> _logger;
        private readonly string _apiKey;
        private readonly string _fromEmail;
        private readonly string _fromName;

        public SendGridEmailService(IConfiguration configuration, ILogger<SendGridEmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
            _apiKey = _configuration["SendGrid:ApiKey"];
            _fromEmail = _configuration["SendGrid:FromEmail"] ?? "noreply@mychurchlab.net";
            _fromName = _configuration["SendGrid:FromName"] ?? "MyChurch";
        }

        /// <summary>
        /// Método legado - redireciona para SendEmailAsync
        /// </summary>
        public async Task EnviarEmailAsync(string destinatario, string assunto, string corpoHtml, string corpoTexto = null)
        {
            await SendEmailAsync(destinatario, assunto, corpoHtml, corpoTexto);
        }

        public async Task SendWelcomeEmailAsync(string email, string name, string churchName)
        {
            var subject = $"Bem-vindo à {churchName}! ??";
            var htmlContent = $@"
                <!DOCTYPE html>
                <html>
                <head>
                    <meta charset='UTF-8'>
                    <style>
                        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
                        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                        .header {{ background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; padding: 30px; text-align: center; border-radius: 10px 10px 0 0; }}
                        .content {{ background: #f9f9f9; padding: 30px; border-radius: 0 0 10px 10px; }}
                        .button {{ display: inline-block; background: #667eea; color: white; padding: 12px 30px; text-decoration: none; border-radius: 5px; margin: 20px 0; }}
                        .footer {{ text-align: center; margin-top: 30px; color: #666; font-size: 12px; }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <h1>Bem-vindo à {churchName}!</h1>
                        </div>
                        <div class='content'>
                            <p>Olá <strong>{name}</strong>,</p>
                            <p>Ficamos muito felizes em ter você conosco! ??</p>
                            <p>Aqui estão algumas coisas que você pode fazer:</p>
                            <ul>
                                <li>? Participar de grupos pequenos</li>
                                <li>?? Consultar a agenda de eventos</li>
                                <li>?? Fazer doações online</li>
                                <li>?? Acompanhar planos de leitura bíblica</li>
                                <li>?? Enviar pedidos de oração</li>
                            </ul>
                            <p>Qualquer dúvida, estamos à disposição!</p>
                            <p><strong>Deus abençoe,</strong><br>Equipe {churchName}</p>
                        </div>
                        <div class='footer'>
                            <p>Você recebeu este email porque criou uma conta em {churchName}</p>
                        </div>
                    </div>
                </body>
                </html>
            ";

            var plainText = $"Olá {name},\n\nBem-vindo à {churchName}! Ficamos felizes em ter você conosco.\n\nDeus abençoe,\nEquipe {churchName}";

            await SendEmailAsync(email, subject, htmlContent, plainText);
        }

        public async Task SendDonationConfirmationAsync(string email, string name, decimal amount, string paymentMethod, string receiptUrl = null)
        {
            var subject = $"Confirmação de Doação - R$ {amount:N2}";
            var htmlContent = $@"
                <!DOCTYPE html>
                <html>
                <head>
                    <meta charset='UTF-8'>
                    <style>
                        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
                        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                        .header {{ background: #10b981; color: white; padding: 30px; text-align: center; border-radius: 10px 10px 0 0; }}
                        .content {{ background: #f9f9f9; padding: 30px; border-radius: 0 0 10px 10px; }}
                        .receipt-box {{ background: white; padding: 20px; border-left: 4px solid #10b981; margin: 20px 0; }}
                        .button {{ display: inline-block; background: #10b981; color: white; padding: 12px 30px; text-decoration: none; border-radius: 5px; margin: 20px 0; }}
                        .footer {{ text-align: center; margin-top: 30px; color: #666; font-size: 12px; }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <h1>? Doação Confirmada!</h1>
                        </div>
                        <div class='content'>
                            <p>Olá <strong>{name}</strong>,</p>
                            <p>Sua doação foi recebida com sucesso!</p>
                            <div class='receipt-box'>
                                <h3>Detalhes da Doação:</h3>
                                <p><strong>Valor:</strong> R$ {amount:N2}</p>
                                <p><strong>Data:</strong> {DateTime.UtcNow:dd/MM/yyyy HH:mm}</p>
                                <p><strong>Método:</strong> {paymentMethod}</p>
                            </div>
                            {(string.IsNullOrEmpty(receiptUrl) ? "" : $"<p><a href='{receiptUrl}' class='button'>?? Baixar Recibo</a></p>")}
                            <p>Muito obrigado pela sua contribuição! Sua doação faz diferença. ??</p>
                            <p><strong>Deus abençoe,</strong><br>Equipe MyChurch</p>
                        </div>
                        <div class='footer'>
                            <p>Este é um email automático de confirmação de doação</p>
                        </div>
                    </div>
                </body>
                </html>
            ";

            var plainText = $"Olá {name},\n\nSua doação de R$ {amount:N2} foi confirmada!\n\nMétodo: {paymentMethod}\nData: {DateTime.UtcNow:dd/MM/yyyy HH:mm}\n\nMuito obrigado!";

            await SendEmailAsync(email, subject, htmlContent, plainText);
        }

        public async Task SendEventReminderAsync(string email, string name, string eventName, DateTime eventDate, string location)
        {
            var subject = $"Lembrete: {eventName} - {eventDate:dd/MM/yyyy}";
            var htmlContent = $@"
                <!DOCTYPE html>
                <html>
                <head>
                    <meta charset='UTF-8'>
                    <style>
                        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
                        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                        .header {{ background: #f59e0b; color: white; padding: 30px; text-align: center; border-radius: 10px 10px 0 0; }}
                        .content {{ background: #f9f9f9; padding: 30px; border-radius: 0 0 10px 10px; }}
                        .event-box {{ background: white; padding: 20px; border-left: 4px solid #f59e0b; margin: 20px 0; }}
                        .footer {{ text-align: center; margin-top: 30px; color: #666; font-size: 12px; }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <h1>?? Lembrete de Evento</h1>
                        </div>
                        <div class='content'>
                            <p>Olá <strong>{name}</strong>,</p>
                            <p>Não se esqueça! Em breve temos:</p>
                            <div class='event-box'>
                                <h3>{eventName}</h3>
                                <p><strong>?? Data:</strong> {eventDate:dd/MM/yyyy}</p>
                                <p><strong>?? Horário:</strong> {eventDate:HH:mm}</p>
                                <p><strong>?? Local:</strong> {location}</p>
                            </div>
                            <p>Esperamos por você! ??</p>
                            <p><strong>Deus abençoe,</strong><br>Equipe MyChurch</p>
                        </div>
                        <div class='footer'>
                            <p>Lembrete automático de evento</p>
                        </div>
                    </div>
                </body>
                </html>
            ";

            var plainText = $"Olá {name},\n\nLembrete: {eventName}\nData: {eventDate:dd/MM/yyyy HH:mm}\nLocal: {location}\n\nEsperamos você!";

            await SendEmailAsync(email, subject, htmlContent, plainText);
        }

        public async Task SendVisitorFollowUpAsync(string email, string visitorName, string churchName = null)
        {
            churchName ??= "nossa igreja";
            var subject = "Sentimos sua falta! ??";
            var htmlContent = $@"
                <!DOCTYPE html>
                <html>
                <head>
                    <meta charset='UTF-8'>
                    <style>
                        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
                        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                        .header {{ background: #3b82f6; color: white; padding: 30px; text-align: center; border-radius: 10px 10px 0 0; }}
                        .content {{ background: #f9f9f9; padding: 30px; border-radius: 0 0 10px 10px; }}
                        .button {{ display: inline-block; background: #3b82f6; color: white; padding: 12px 30px; text-decoration: none; border-radius: 5px; margin: 20px 0; }}
                        .footer {{ text-align: center; margin-top: 30px; color: #666; font-size: 12px; }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <h1>Sentimos sua falta! ??</h1>
                        </div>
                        <div class='content'>
                            <p>Olá <strong>{visitorName}</strong>,</p>
                            <p>Vimos que você visitou {churchName} recentemente! ??</p>
                            <p>Gostaríamos muito de conhecer você melhor.</p>
                            <p>Você pode:</p>
                            <ul>
                                <li>?? Conhecer nossos grupos pequenos</li>
                                <li>?? Agendar uma conversa com nossos líderes</li>
                                <li>? Tirar dúvidas sobre a igreja</li>
                            </ul>
                            <p>Ficaremos muito felizes em recebê-lo novamente!</p>
                            <p><strong>Deus abençoe,</strong><br>Equipe {churchName}</p>
                        </div>
                        <div class='footer'>
                            <p>Você recebeu este email porque visitou {churchName}</p>
                        </div>
                    </div>
                </body>
                </html>
            ";

            var plainText = $"Olá {visitorName},\n\nSentimos sua falta! Vimos que você visitou {churchName} recentemente.\n\nGostaríamos de conhecer você melhor. Entre em contato!\n\nDeus abençoe!";

            await SendEmailAsync(email, subject, htmlContent, plainText);
        }

        public async Task SendBirthdayEmailAsync(string email, string name)
        {
            var subject = "?? Feliz Aniversário!";
            var htmlContent = $@"
                <!DOCTYPE html>
                <html>
                <head>
                    <meta charset='UTF-8'>
                    <style>
                        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
                        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                        .header {{ background: linear-gradient(135deg, #f093fb 0%, #f5576c 100%); color: white; padding: 30px; text-align: center; border-radius: 10px 10px 0 0; }}
                        .content {{ background: #f9f9f9; padding: 30px; border-radius: 0 0 10px 10px; text-align: center; }}
                        .footer {{ text-align: center; margin-top: 30px; color: #666; font-size: 12px; }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <h1>???? Feliz Aniversário! ????</h1>
                        </div>
                        <div class='content'>
                            <h2>Parabéns, {name}!</h2>
                            <p>Que este novo ano de vida seja repleto de:</p>
                            <p>? Bênçãos ? Saúde ? Alegria ?</p>
                            <p>Que Deus continue te guiando e abençoando em todos os seus caminhos!</p>
                            <p><strong>Com carinho,</strong><br>Sua família em Cristo ??</p>
                        </div>
                        <div class='footer'>
                            <p>Mensagem automática de aniversário</p>
                        </div>
                    </div>
                </body>
                </html>
            ";

            var plainText = $"Feliz Aniversário, {name}!\n\nQue Deus abençoe este novo ano de vida!\n\nCom carinho, Sua família em Cristo";

            await SendEmailAsync(email, subject, htmlContent, plainText);
        }

        public async Task SendPrayerRequestConfirmationAsync(string email, string name, string request)
        {
            var subject = "?? Seu pedido de oração foi recebido";
            var htmlContent = $@"
                <!DOCTYPE html>
                <html>
                <head>
                    <meta charset='UTF-8'>
                    <style>
                        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
                        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                        .header {{ background: #8b5cf6; color: white; padding: 30px; text-align: center; border-radius: 10px 10px 0 0; }}
                        .content {{ background: #f9f9f9; padding: 30px; border-radius: 0 0 10px 10px; }}
                        .prayer-box {{ background: white; padding: 20px; border-left: 4px solid #8b5cf6; margin: 20px 0; font-style: italic; }}
                        .footer {{ text-align: center; margin-top: 30px; color: #666; font-size: 12px; }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <h1>?? Pedido de Oração Recebido</h1>
                        </div>
                        <div class='content'>
                            <p>Olá <strong>{name}</strong>,</p>
                            <p>Recebemos seu pedido de oração:</p>
                            <div class='prayer-box'>
                                <p>{request}</p>
                            </div>
                            <p>Nossa equipe de intercessão estará orando por você.</p>
                            <p><em>""Orai uns pelos outros"" - Tiago 5:16</em></p>
                            <p><strong>Deus abençoe,</strong><br>Equipe de Intercessão</p>
                        </div>
                        <div class='footer'>
                            <p>Confirmação de pedido de oração</p>
                        </div>
                    </div>
                </body>
                </html>
            ";

            var plainText = $"Olá {name},\n\nRecebemos seu pedido de oração:\n\n\"{request}\"\n\nEstaremos orando por você!\n\nDeus abençoe";

            await SendEmailAsync(email, subject, htmlContent, plainText);
        }

        public async Task SendInactiveMemberEmailAsync(string email, string name, string churchName, int daysInactive)
        {
            var subject = $"Sentimos sua falta em {churchName}!";
            var htmlContent = $@"
                <!DOCTYPE html>
                <html>
                <head>
                    <meta charset='UTF-8'>
                    <style>
                        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
                        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                        .header {{ background: #ef4444; color: white; padding: 30px; text-align: center; border-radius: 10px 10px 0 0; }}
                        .content {{ background: #f9f9f9; padding: 30px; border-radius: 0 0 10px 10px; }}
                        .footer {{ text-align: center; margin-top: 30px; color: #666; font-size: 12px; }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <h1>?? Sentimos sua falta!</h1>
                        </div>
                        <div class='content'>
                            <p>Olá <strong>{name}</strong>,</p>
                            <p>Percebemos que você não participa de nossas atividades há {daysInactive} dias.</p>
                            <p>Gostaríamos muito de saber como você está!</p>
                            <p>Se precisar de algo, estamos aqui para ajudar. ??</p>
                            <p>Volte quando puder, sua presença faz diferença!</p>
                            <p><strong>Com carinho,</strong><br>Equipe {churchName}</p>
                        </div>
                        <div class='footer'>
                            <p>Mensagem de cuidado pastoral</p>
                        </div>
                    </div>
                </body>
                </html>
            ";

            var plainText = $"Olá {name},\n\nSentimos sua falta! Você não participa há {daysInactive} dias.\n\nEstamos aqui se precisar de algo.\n\nCom carinho, {churchName}";

            await SendEmailAsync(email, subject, htmlContent, plainText);
        }

        public async Task SendEmailAsync(string to, string subject, string htmlContent, string plainTextContent = null)
        {
            if (string.IsNullOrEmpty(_apiKey))
            {
                _logger.LogWarning("SendGrid API Key não configurada. Email não será enviado.");
                return;
            }

            try
            {
                var client = new SendGridClient(_apiKey);
                var from = new EmailAddress(_fromEmail, _fromName);
                var toAddress = new EmailAddress(to);
                var msg = MailHelper.CreateSingleEmail(from, toAddress, subject, plainTextContent ?? subject, htmlContent);
                
                var response = await client.SendEmailAsync(msg);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation($"Email enviado com sucesso para {to}");
                }
                else
                {
                    var body = await response.Body.ReadAsStringAsync();
                    _logger.LogError($"Erro ao enviar email para {to}: {response.StatusCode} - {body}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exceção ao enviar email para {to}");
            }
        }
    }
}
