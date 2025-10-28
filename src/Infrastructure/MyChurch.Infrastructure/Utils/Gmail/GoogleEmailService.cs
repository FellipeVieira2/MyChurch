using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Services;
using Microsoft.Extensions.Configuration;
using MimeKit;
using MyChurch.Infrastructure.Utils.SES;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace MyChurch.Infrastructure.Utils.Gmail
{
    public class GoogleEmailService : IEmailService
    {
        private readonly string _remetente;
        private readonly string _googleCloudToken;
        private readonly IConfiguration _configuration;
        
        public GoogleEmailService(IConfiguration configuration)
        {
            _configuration = configuration;
            _remetente = configuration["Email:Remetente"] ?? "comercial@mychurchlab.net";
            _googleCloudToken = configuration["GoogleCloudApiKey"];
        }

        public async Task EnviarEmailAsync(string destinatario, string assunto, string corpoHtml, string corpoTexto = null)
        {
            try
            {
                // Create a new MimeMessage
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("MyChurch", _remetente));
                message.To.Add(new MailboxAddress("", destinatario));
                message.Subject = assunto;

                // Create the HTML body part
                var bodyBuilder = new BodyBuilder
                {
                    HtmlBody = corpoHtml,
                    TextBody = corpoTexto ?? "Este email requer um leitor de HTML."
                };

                message.Body = bodyBuilder.ToMessageBody();

                // Convert the MimeMessage to a Gmail API message
                using var memoryStream = new MemoryStream();
                await message.WriteToAsync(memoryStream);
                memoryStream.Position = 0;

                // Create Gmail API service
                var service = CreateGmailService();

                // Create the message
                var gmailMessage = new Message
                {
                    Raw = Convert.ToBase64String(memoryStream.ToArray())
                        .Replace('+', '-')
                        .Replace('/', '_')
                        .Replace("=", "")
                };

                // Send the message
                await service.Users.Messages.Send(gmailMessage, "me").ExecuteAsync();
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error sending email: {ex.Message}");
                throw;
            }
        }

        private GmailService CreateGmailService()
        {
            // Use the API token from configuration
            var credential = GoogleCredential.FromAccessToken(_googleCloudToken)
                .CreateScoped(GmailService.Scope.GmailSend);

            // Create Gmail API service
            return new GmailService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "MyChurch"
            });
        }

        // ? Stub implementations
        public Task SendWelcomeEmailAsync(string email, string name, string churchName) =>
            EnviarEmailAsync(email, $"Bem-vindo à {churchName}", $"<h1>Olá {name}!</h1>");

        public Task SendDonationConfirmationAsync(string email, string name, decimal amount, string paymentMethod, string receiptUrl = null) =>
            EnviarEmailAsync(email, "Doação Confirmada", $"<h1>R$ {amount:N2} confirmada!</h1>");

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