using System;
using System.Threading.Tasks;

namespace MyChurch.Domain.Contracts
{
    /// <summary>
    /// Abstração de envio de emails (camada de domínio/aplicação).
    /// Implementações ficam na infraestrutura (ex.: SES).
    /// </summary>
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string htmlContent, string? plainTextContent = null);

        Task SendWelcomeEmailAsync(string email, string name, string churchName);
        Task SendDonationConfirmationAsync(string email, string name, decimal amount, string paymentMethod, string? receiptUrl = null);
        Task SendEventReminderAsync(string email, string name, string eventName, DateTime eventDate, string location);
        Task SendVisitorFollowUpAsync(string email, string visitorName, string? churchName = null);
        Task SendBirthdayEmailAsync(string email, string name);
        Task SendPrayerRequestConfirmationAsync(string email, string name, string request);
        Task SendInactiveMemberEmailAsync(string email, string name, string churchName, int daysInactive);

        // Legado (pt-BR)
        Task EnviarEmailAsync(string destinatario, string assunto, string corpoHtml, string? corpoTexto = null);
    }
}
