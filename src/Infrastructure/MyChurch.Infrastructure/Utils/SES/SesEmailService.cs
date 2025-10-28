using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;

namespace MyChurch.Infrastructure.Utils.SES
{
    public class SesEmailService : IEmailService
    {
        private readonly IAmazonSimpleEmailService _sesClient;
        private readonly string _remetente;

        public SesEmailService(IAmazonSimpleEmailService sesClient, string remetente)
        {
            _sesClient = sesClient;
            _remetente = remetente ?? throw new ArgumentNullException(nameof(remetente));
        }

        public async Task EnviarEmailAsync(string destinatario, string assunto, string corpoHtml, string corpoTexto = null)
        {
            var sendRequest = new SendEmailRequest
            {
                Source = _remetente,
                Destination = new Destination
                {
                    ToAddresses = new List<string> { destinatario }
                },
                Message = new Message
                {
                    Subject = new Content(assunto),
                    Body = new Body
                    {
                        Html = new Content { Charset = "UTF-8", Data = corpoHtml },
                        Text = string.IsNullOrEmpty(corpoTexto) ? null : new Content { Charset = "UTF-8", Data = corpoTexto }
                    }
                }
            };

            await _sesClient.SendEmailAsync(sendRequest);
        }

        // ✅ Stub implementations - redirect to EnviarEmailAsync
        public Task SendWelcomeEmailAsync(string email, string name, string churchName) =>
            EnviarEmailAsync(email, $"Bem-vindo à {churchName}", $"<h1>Olá {name}!</h1>");

        public Task SendDonationConfirmationAsync(string email, string name, decimal amount, string paymentMethod, string receiptUrl = null) =>
            EnviarEmailAsync(email, "Doação Confirmada", $"<h1>Doação de R$ {amount:N2} confirmada!</h1>");

        public Task SendEventReminderAsync(string email, string name, string eventName, DateTime eventDate, string location) =>
            EnviarEmailAsync(email, $"Lembrete: {eventName}", $"<h1>{eventName}</h1>");

        public Task SendVisitorFollowUpAsync(string email, string visitorName, string churchName = null) =>
            EnviarEmailAsync(email, "Sentimos sua falta!", $"<h1>Olá {visitorName}!</h1>");

        public Task SendBirthdayEmailAsync(string email, string name) =>
            EnviarEmailAsync(email, "Feliz Aniversário!", $"<h1>Parabéns {name}!</h1>");

        public Task SendEmailAsync(string to, string subject, string htmlContent, string plainTextContent = null) =>
            EnviarEmailAsync(to, subject, htmlContent, plainTextContent);

        public Task SendPrayerRequestConfirmationAsync(string email, string name, string request) =>
            EnviarEmailAsync(email, "Pedido de Oração", $"<h1>Recebido!</h1>");

        public Task SendInactiveMemberEmailAsync(string email, string name, string churchName, int daysInactive) =>
            EnviarEmailAsync(email, "Sentimos sua falta!", $"<h1>Olá {name}!</h1>");
    }
}
