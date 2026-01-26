using Microsoft.EntityFrameworkCore;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Exceptions;
using MyChurch.Domain.Enum;

namespace MyChurch.Application.Plans.Services
{
    public class PlanLimitService : IPlanLimitService
    {
        private readonly IUnitOfWork _uow;

        public PlanLimitService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<Domain.Entities.Plan?> GetActivePlanForChurchAsync(int churchId, CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow;

            // Preferir assinatura ativa
            var subscription = await _uow.Subscriptions.Query()
                .AsNoTracking()
                .Include(s => s.Plan)
                .Where(s => s.ChurchId == churchId)
                .Where(s => s.StartDate <= now && s.EndDate > now)
                .OrderByDescending(s => s.EndDate)
                .FirstOrDefaultAsync(cancellationToken);

            if (subscription?.Plan != null)
                return subscription.Plan;

            // Fallback: plano Free
            var freePlan = await _uow.Plans.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Tier == PlanTier.Free, cancellationToken);

            if (freePlan != null)
                return freePlan;

            // Último fallback: plano com menor preço
            return await _uow.Plans.Query()
                .AsNoTracking()
                .OrderBy(p => p.Price)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task EnsureMaxMembersAllowedAsync(int churchId, int additionalMembersToAdd, CancellationToken cancellationToken)
        {
            if (additionalMembersToAdd <= 0) return;

            var plan = await GetActivePlanForChurchAsync(churchId, cancellationToken);
            if (plan == null) return;

            var currentCount = await _uow.Members.Query()
                .AsNoTracking()
                .CountAsync(m => m.ChurchId == churchId, cancellationToken);

            if (currentCount + additionalMembersToAdd > plan.MaxMembers)
                ValidationException.ThrowException("plan_limit", $"Limite de membros atingido para o plano atual (MaxMembers={plan.MaxMembers}).");
        }

        public async Task EnsureMaxEventsAllowedAsync(int churchId, int additionalEventsToAdd, CancellationToken cancellationToken)
        {
            if (additionalEventsToAdd <= 0) return;

            var plan = await GetActivePlanForChurchAsync(churchId, cancellationToken);
            if (plan == null) return;

            var currentCount = await _uow.Events.Query()
                .AsNoTracking()
                .CountAsync(e => e.ChurchId == churchId, cancellationToken);

            if (currentCount + additionalEventsToAdd > plan.MaxEvents)
                ValidationException.ThrowException("plan_limit", $"Limite de eventos atingido para o plano atual (MaxEvents={plan.MaxEvents}).");
        }

        public async Task EnsureMaxDonationsPerMonthAllowedAsync(int churchId, int additionalDonationsToAdd, CancellationToken cancellationToken)
        {
            if (additionalDonationsToAdd <= 0) return;

            var plan = await GetActivePlanForChurchAsync(churchId, cancellationToken);
            if (plan == null) return;

            var now = DateTime.UtcNow;
            var monthStart = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
            var nextMonthStart = monthStart.AddMonths(1);

            var currentCount = await _uow.Donations.Query()
                .AsNoTracking()
                .CountAsync(d => d.Date >= monthStart && d.Date < nextMonthStart &&
                                 _uow.Members.Query().Any(m => m.Id == d.MemberId && m.ChurchId == churchId),
                    cancellationToken);

            if (currentCount + additionalDonationsToAdd > plan.MaxDonationsPerMonth)
                ValidationException.ThrowException("plan_limit", $"Limite de doações/mês atingido para o plano atual (MaxDonationsPerMonth={plan.MaxDonationsPerMonth}).");
        }

        public async Task EnsureExportAllowedAsync(int churchId, ExportType exportType, CancellationToken cancellationToken)
        {
            var plan = await GetActivePlanForChurchAsync(churchId, cancellationToken);
            if (plan == null) return;

            var allowed = exportType switch
            {
                ExportType.Csv => plan.CanExportCsv,
                ExportType.Pdf => plan.CanExportPdf,
                _ => false
            };

            if (!allowed)
                ValidationException.ThrowException("plan_limit", "Seu plano atual não permite exportação neste formato.");
        }

        public async Task EnsureDepartmentReportsAllowedAsync(int churchId, CancellationToken cancellationToken)
        {
            var plan = await GetActivePlanForChurchAsync(churchId, cancellationToken);
            if (plan == null) return;

            if (!plan.HasDepartmentReports)
                ValidationException.ThrowException("plan_limit", "Seu plano atual não permite relatórios por departamento.");
        }

        public async Task EnsureAdvancedPermissionsAllowedAsync(int churchId, CancellationToken cancellationToken)
        {
            var plan = await GetActivePlanForChurchAsync(churchId, cancellationToken);
            if (plan == null) return;

            if (!plan.HasAdvancedPermissions)
                ValidationException.ThrowException("plan_limit", "Seu plano atual não permite permissões avançadas.");
        }

        public async Task EnsurePromotionAllowedAsync(int churchId, PlanPromotionType promotionType, CancellationToken cancellationToken)
        {
            var plan = await GetActivePlanForChurchAsync(churchId, cancellationToken);
            if (plan == null) return;

            var allowed = promotionType switch
            {
                PlanPromotionType.Church => plan.CanPromoteChurch,
                PlanPromotionType.Event => plan.CanPromoteEvents,
                _ => false
            };

            if (!allowed)
                ValidationException.ThrowException("plan_limit", "Seu plano atual não permite promoções.");
        }

        public async Task EnsureMaxBranchesAllowedAsync(int parentChurchId, int additionalBranchesToAdd, CancellationToken cancellationToken)
        {
            if (additionalBranchesToAdd <= 0) return;

            var plan = await GetActivePlanForChurchAsync(parentChurchId, cancellationToken);
            if (plan == null) return;

            var currentBranches = await _uow.Churchs.Query()
                .AsNoTracking()
                .CountAsync(c => c.ParentChurchId == parentChurchId, cancellationToken);

            if (currentBranches + additionalBranchesToAdd > plan.Branches)
                ValidationException.ThrowException("plan_limit", $"Limite de filiais atingido para o plano atual (Branches={plan.Branches}).");
        }
    }
}
