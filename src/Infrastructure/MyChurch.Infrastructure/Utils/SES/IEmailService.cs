using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyChurch.Infrastructure.Utils.SES
{
    public interface IEmailService
    {
        Task EnviarEmailAsync(string destinatario, string assunto, string corpoHtml, string corpoTexto = null);
    }
}
