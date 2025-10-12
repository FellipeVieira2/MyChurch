using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;
using MyChurch.Domain.Enum;

namespace MyChurch.Application.CashFlow.Services
{
    public interface ICashFlowAutomationService
    {
        /// <summary>
        /// Cria lançamento automático quando doação é confirmada
        /// </summary>
        Task CreateEntryFromDonationAsync(int donationId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Busca ou cria categoria padrão para doações
        /// </summary>
        Task<CashFlowCategory> GetOrCreateDonationCategoryAsync(int churchId, CancellationToken cancellationToken = default);
    }

    public class CashFlowAutomationService : ICashFlowAutomationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CashFlowAutomationService> _logger;

        public CashFlowAutomationService(
            IUnitOfWork unitOfWork,
            ILogger<CashFlowAutomationService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task CreateEntryFromDonationAsync(int donationId, CancellationToken cancellationToken = default)
        {
            var donation = await _unitOfWork.Donations.Query()
                .Include(d => d.Member)
                .Include(d => d.Payments)
                .FirstOrDefaultAsync(d => d.Id == donationId, cancellationToken);

            if (donation == null)
            {
                _logger.LogWarning("Doação {DonationId} não encontrada", donationId);
                return;
            }

            // Valida se doação foi confirmada
            var isPaid = donation.Payments.Any(p =>
                p.PaymentStatus == PaymentStatus.Completed.ToString() ||
                p.PaymentStatus == PaymentStatus.Received.ToString() ||
                p.PaymentStatus == "RECEIVED" ||
                p.PaymentStatus == "CONFIRMED");

            if (!isPaid)
            {
                _logger.LogInformation("Doação {DonationId} ainda não foi confirmada", donationId);
                return;
            }

            var churchId = donation.Member.ChurchId;

            // Verifica se já existe lançamento para esta doação
            var existingEntry = await _unitOfWork.CashFlowEntries.Query()
                .FirstOrDefaultAsync(e => e.DonationId == donationId, cancellationToken);

            if (existingEntry != null)
            {
                _logger.LogInformation("Lançamento automático já existe para doação {DonationId}", donationId);
                return;
            }

            // Busca ou cria categoria de doações
            var category = await GetOrCreateDonationCategoryAsync(churchId, cancellationToken);

            // Pega a data de confirmação do pagamento (usa Date ao invés de PaymentDate)
            var paymentDate = donation.Payments
                .Where(p => p.PaymentStatus == PaymentStatus.Completed.ToString() ||
                           p.PaymentStatus == PaymentStatus.Received.ToString() ||
                           p.PaymentStatus == "RECEIVED" ||
                           p.PaymentStatus == "CONFIRMED")
                .OrderByDescending(p => p.Date) // ?? CORRIGIDO: Date ao invés de PaymentDate
                .Select(p => p.Date)
                .FirstOrDefault();

            if (paymentDate == default)
                paymentDate = DateTime.UtcNow;

            // Valor líquido (já vem deduzido após taxas no CreateDonationCommand)
            var netAmount = donation.Amount;

            // Cria lançamento automático
            var cashFlowEntry = new CashFlowEntry
            {
                Amount = netAmount,
                Date = paymentDate,
                Type = CashFlowType.Income,
                CategoryId = category.Id,
                ChurchId = churchId,
                MemberId = donation.MemberId,
                DonationId = donationId,
                IsAutomatic = true,
                Description = $"Doação #{donation.Id} - {donation.Member.Name}",
                Notes = $"Gerado automaticamente via confirmação de pagamento"
            };

            _unitOfWork.CashFlowEntries.Create(cashFlowEntry);
            await _unitOfWork.CommitAsync();

            _logger.LogInformation(
                "? Lançamento automático criado: Doação {DonationId} ? CashFlowEntry {EntryId} (R$ {Amount:F2})",
                donationId,
                cashFlowEntry.Id,
                netAmount);
        }

        public async Task<CashFlowCategory> GetOrCreateDonationCategoryAsync(int churchId, CancellationToken cancellationToken = default)
        {
            const string CATEGORY_NAME = "Doações";

            var category = await _unitOfWork.CashFlowCategories.Query()
                .FirstOrDefaultAsync(c => c.ChurchId == churchId && c.Name == CATEGORY_NAME, cancellationToken);

            if (category == null)
            {
                category = new CashFlowCategory
                {
                    Name = CATEGORY_NAME,
                    Description = "Categoria automática para registro de doações confirmadas via Asaas",
                    ChurchId = churchId
                    // ?? REMOVIDO: Created (não existe na entidade)
                };

                _unitOfWork.CashFlowCategories.Create(category);
                await _unitOfWork.CommitAsync();

                _logger.LogInformation("? Categoria '{CategoryName}' criada para igreja {ChurchId}", CATEGORY_NAME, churchId);
            }

            return category;
        }
    }
}
