using MyChurch.Domain.Enum;

namespace MyChurch.Application.Plans.Services
{
    public interface IPlanLimitService
    {
        Task<Domain.Entities.Plan?> GetActivePlanForChurchAsync(int churchId, CancellationToken cancellationToken);
        Task EnsureMaxMembersAllowedAsync(int churchId, int additionalMembersToAdd, CancellationToken cancellationToken);
        Task EnsureMaxEventsAllowedAsync(int churchId, int additionalEventsToAdd, CancellationToken cancellationToken);
        Task EnsureMaxDonationsPerMonthAllowedAsync(int churchId, int additionalDonationsToAdd, CancellationToken cancellationToken);
        Task EnsureExportAllowedAsync(int churchId, ExportType exportType, CancellationToken cancellationToken);
        Task EnsureDepartmentReportsAllowedAsync(int churchId, CancellationToken cancellationToken);
        Task EnsureAdvancedPermissionsAllowedAsync(int churchId, CancellationToken cancellationToken);
        Task EnsurePromotionAllowedAsync(int churchId, PlanPromotionType promotionType, CancellationToken cancellationToken);
        Task EnsureMaxBranchesAllowedAsync(int parentChurchId, int additionalBranchesToAdd, CancellationToken cancellationToken);
    }

    public enum ExportType
    {
        Csv,
        Pdf
    }

    public enum PlanPromotionType
    {
        Church,
        Event
    }
}
