using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyChurch.Infrastructure.Utils.SES
{
    /// <summary>
    /// Serviço de envio de emails transacionais
    /// </summary>
    public interface IEmailService
    {
        /// <summary>
        /// Método legado - manter para compatibilidade
        /// </summary>
        Task EnviarEmailAsync(string destinatario, string assunto, string corpoHtml, string corpoTexto = null);

        /// <summary>
        /// Envia email de boas-vindas para novo membro
        /// </summary>
        Task SendWelcomeEmailAsync(string email, string name, string churchName);

        /// <summary>
        /// Envia confirmação de doação
        /// </summary>
        Task SendDonationConfirmationAsync(string email, string name, decimal amount, string paymentMethod, string receiptUrl = null);

        /// <summary>
        /// Envia lembrete de evento
        /// </summary>
        Task SendEventReminderAsync(string email, string name, string eventName, DateTime eventDate, string location);

        /// <summary>
        /// Envia follow-up para visitante
        /// </summary>
        Task SendVisitorFollowUpAsync(string email, string visitorName, string churchName = null);

        /// <summary>
        /// Envia email de aniversário
        /// </summary>
        Task SendBirthdayEmailAsync(string email, string name);

        /// <summary>
        /// Envia email genérico
        /// </summary>
        Task SendEmailAsync(string to, string subject, string htmlContent, string plainTextContent = null);

        /// <summary>
        /// Envia email de confirmação de pedido de oração
        /// </summary>
        Task SendPrayerRequestConfirmationAsync(string email, string name, string request);

        /// <summary>
        /// Envia email para membro inativo
        /// </summary>
        Task SendInactiveMemberEmailAsync(string email, string name, string churchName, int daysInactive);
    }
}
