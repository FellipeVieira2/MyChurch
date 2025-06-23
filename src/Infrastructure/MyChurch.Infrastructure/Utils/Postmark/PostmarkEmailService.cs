using Microsoft.Extensions.Configuration;
using MyChurch.Infrastructure.Utils.SES;
using PostmarkDotNet;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyChurch.Infrastructure.Utils.Postmark
{
    public class PostmarkEmailService : IEmailService
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
    }
}