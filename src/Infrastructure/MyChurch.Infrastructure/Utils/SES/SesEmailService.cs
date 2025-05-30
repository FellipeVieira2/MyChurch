
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
    }
}
