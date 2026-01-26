using MyChurch.Domain.Enum;

namespace MyChurch.Application.Plans.Services
{
    public interface IPlanAccessService
    {
        Task EnsureRoleChangeAllowedAsync(int churchId, UserRole newRole, CancellationToken cancellationToken);
    }
}
