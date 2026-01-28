using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyChurch.Domain.Contracts;

namespace MyChurch.Infrastructure.Utils.SES
{
    /// <summary>
    /// Serviço de envio de emails transacionais
    /// </summary>
    public interface IEmailService : MyChurch.Domain.Contracts.IEmailService
    {
    }
}
