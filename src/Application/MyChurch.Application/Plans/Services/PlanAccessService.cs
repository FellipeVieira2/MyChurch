using Microsoft.EntityFrameworkCore;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Enum;
using MyChurch.Domain.Exceptions;

namespace MyChurch.Application.Plans.Services
{
    public class PlanAccessService : IPlanAccessService
    {
        private readonly IUnitOfWork _uow;
        private readonly IPlanLimitService _planLimits;

        public PlanAccessService(IUnitOfWork uow, IPlanLimitService planLimits)
        {
            _uow = uow;
            _planLimits = planLimits;
        }

        public async Task EnsureRoleChangeAllowedAsync(int churchId, UserRole newRole, CancellationToken cancellationToken)
        {
            // PlatformAdmin role é global; não depende de plano da igreja
            if (newRole == UserRole.PlatformAdmin)
                return;

            var plan = await _planLimits.GetActivePlanForChurchAsync(churchId, cancellationToken);
            if (plan == null)
                return;

            if (newRole == UserRole.Admin)
            {
                var currentAdmins = await _uow.Members.Query()
                    .AsNoTracking()
                    .CountAsync(m => m.ChurchId == churchId && m.Role == UserRole.Admin && m.IsActive, cancellationToken);

                if (currentAdmins >= plan.MaxAdmins)
                    ValidationException.ThrowException("plan_limit", $"Limite de administradores atingido para o plano atual (MaxAdmins={plan.MaxAdmins}).");
            }

            if (newRole == UserRole.Leader)
            {
                var currentLeaders = await _uow.Members.Query()
                    .AsNoTracking()
                    .CountAsync(m => m.ChurchId == churchId && m.Role == UserRole.Leader && m.IsActive, cancellationToken);

                if (currentLeaders >= plan.MaxLeaders)
                    ValidationException.ThrowException("plan_limit", $"Limite de líderes atingido para o plano atual (MaxLeaders={plan.MaxLeaders}).");
            }
        }
    }
}
