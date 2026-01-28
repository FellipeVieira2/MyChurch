using Microsoft.Extensions.Configuration;
using MyChurch.Infrastructure.Utils.SES;
using PostmarkDotNet;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyChurch.Infrastructure.Utils.Postmark
{
    public class PostmarkEmailService : MyChurch.Infrastructure.Utils.SES.IEmailService
    {
        private readonly string _remetente;
        private readonly string _postmarkServerToken;
        private readonly PostmarkClient _client;

        public PostmarkEmailService(IConfiguration configuration)
        {
            _remetente = configuration["Email:Remetente"] ?? "comercial@mychurchlab.net";
            _postmarkServerToken = configuration["Postmark:ServerToken"] ?? throw new ArgumentNullException("Postmark:ServerToken configuration is missing");
            _client = new PostmarkClient(_postmarkServerToken);
        }

        public async Task EnviarEmailAsync(string destinatario, string assunto, string corpoHtml, string corpoTexto = null)
        {
            try
            {
                var message = new PostmarkMessage
                {
                    From = _remetente,
                    To = destinatario,
                    Subject = assunto,
                    HtmlBody = corpoHtml,
                    TextBody = corpoTexto ?? "Este email requer um leitor de HTML."
                };

                var response = await _client.SendMessageAsync(message);
                
                if (response.Status != PostmarkStatus.Success)
                {
                    throw new Exception($"Failed to send email through Postmark: {response.Message}");
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error sending email with Postmark: {ex.Message}");
                throw;
            }
        }

        // ? Implementações dos novos métodos (podem ser sobrescritas ou usar EnviarEmailAsync)
        public Task SendWelcomeEmailAsync(string email, string name, string churchName)
        {
            var html = $"<h1>Bem-vindo à {churchName}!</h1><p>Olá {name}, seja bem-vindo!</p>";
            return EnviarEmailAsync(email, $"Bem-vindo à {churchName}", html);
        }

        public Task SendDonationConfirmationAsync(string email, string name, decimal amount, string paymentMethod, string receiptUrl = null)
        {
            var html = $"<h1>Doação Confirmada</h1><p>Olá {name}, sua doação de R$ {amount:N2} foi confirmada!</p>";
            return EnviarEmailAsync(email, "Confirmação de Doação", html);
        }

        public Task SendEventReminderAsync(string email, string name, string eventName, DateTime eventDate, string location)
        {
            var html = $"<h1>Lembrete de Evento</h1><p>Olá {name}, não esqueça: {eventName} em {eventDate:dd/MM/yyyy} em {location}</p>";
            return EnviarEmailAsync(email, $"Lembrete: {eventName}", html);
        }

        public Task SendVisitorFollowUpAsync(string email, string visitorName, string churchName = null)
        {
            var html = $"<h1>Sentimos sua falta!</h1><p>Olá {visitorName}, gostaríamos de vê-lo novamente!</p>";
            return EnviarEmailAsync(email, "Sentimos sua falta!", html);
        }

        public Task SendBirthdayEmailAsync(string email, string name)
        {
            var html = $"<h1>Feliz Aniversário!</h1><p>Parabéns, {name}! Que Deus abençoe este novo ano de vida!</p>";
            return EnviarEmailAsync(email, "?? Feliz Aniversário!", html);
        }

        public Task SendEmailAsync(string to, string subject, string htmlContent, string plainTextContent = null)
        {
            return EnviarEmailAsync(to, subject, htmlContent, plainTextContent);
        }

        public Task SendPrayerRequestConfirmationAsync(string email, string name, string request)
        {
            var html = $"<h1>Pedido de Oração Recebido</h1><p>Olá {name}, recebemos seu pedido de oração e estaremos intercedendo por você.</p>";
            return EnviarEmailAsync(email, "Pedido de Oração Recebido", html);
        }

        public Task SendInactiveMemberEmailAsync(string email, string name, string churchName, int daysInactive)
        {
            var html = $"<h1>Sentimos sua falta!</h1><p>Olá {name}, percebemos que você não participa há {daysInactive} dias. Estamos aqui se precisar!</p>";
            return EnviarEmailAsync(email, "Sentimos sua falta!", html);
        }
    }
}