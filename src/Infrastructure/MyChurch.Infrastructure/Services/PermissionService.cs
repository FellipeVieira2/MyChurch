using Microsoft.EntityFrameworkCore;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;
using MyChurch.Domain.Enum;
using MyChurch.Domain.Services;

namespace MyChurch.Infrastructure.Services
{
    public class PermissionService : IPermissionService
    {
        private readonly IUnitOfWork _unitOfWork;

        public PermissionService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> HasPermissionAsync(int memberId, Permission permission, CancellationToken cancellationToken = default)
        {
            // 1. Buscar o membro
            var member = await _unitOfWork.Members.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == memberId, cancellationToken);

            if (member == null)
                return false;

            // 2. Verificar se existe permissão customizada (tem prioridade)
            var customPermission = await _unitOfWork.MemberCustomPermissions
                .GetByMemberAndPermissionAsync(memberId, permission);

            if (customPermission != null && customPermission.IsActive())
            {
                return customPermission.IsGranted;
            }

            // 3. Verificar permissões da role
            var rolePermissions = await _unitOfWork.RolePermissions
                .GetByRoleAsync(member.Role, member.ChurchId);

            return rolePermissions.Any(rp => rp.Permission == permission && rp.IsActive);
        }

        public async Task<bool> HasAnyPermissionAsync(int memberId, Permission[] permissions, CancellationToken cancellationToken = default)
        {
            foreach (var permission in permissions)
            {
                if (await HasPermissionAsync(memberId, permission, cancellationToken))
                    return true;
            }
            return false;
        }

        public async Task<bool> HasAllPermissionsAsync(int memberId, Permission[] permissions, CancellationToken cancellationToken = default)
        {
            foreach (var permission in permissions)
            {
                if (!await HasPermissionAsync(memberId, permission, cancellationToken))
                    return false;
            }
            return true;
        }

        public async Task<List<Permission>> GetMemberPermissionsAsync(int memberId, CancellationToken cancellationToken = default)
        {
            var member = await _unitOfWork.Members.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == memberId, cancellationToken);

            if (member == null)
                return new List<Permission>();

            // 1. Obter permissões da role
            var rolePermissions = await _unitOfWork.RolePermissions
                .GetByRoleAsync(member.Role, member.ChurchId);

            var permissions = rolePermissions
                .Where(rp => rp.IsActive)
                .Select(rp => rp.Permission)
                .ToHashSet();

            // 2. Aplicar permissões customizadas
            var customPermissions = await _unitOfWork.MemberCustomPermissions
                .GetByMemberIdAsync(memberId);

            foreach (var custom in customPermissions.Where(cp => cp.IsActive()))
            {
                if (custom.IsGranted)
                {
                    permissions.Add(custom.Permission);
                }
                else
                {
                    permissions.Remove(custom.Permission);
                }
            }

            return permissions.ToList();
        }

        public async Task GrantPermissionAsync(int memberId, Permission permission, int grantedByMemberId, string? reason = null, DateTime? expiresAt = null, CancellationToken cancellationToken = default)
        {
            var existing = await _unitOfWork.MemberCustomPermissions
                .GetByMemberAndPermissionAsync(memberId, permission);

            if (existing != null)
            {
                existing.IsGranted = true;
                existing.GrantedByMemberId = grantedByMemberId;
                existing.Reason = reason;
                existing.ExpiresAt = expiresAt;
                existing.Updated = DateTime.UtcNow;
                _unitOfWork.MemberCustomPermissions.Update(existing);
            }
            else
            {
                var customPermission = new MemberCustomPermission
                {
                    MemberId = memberId,
                    Permission = permission,
                    IsGranted = true,
                    GrantedByMemberId = grantedByMemberId,
                    Reason = reason,
                    ExpiresAt = expiresAt,
                    Created = DateTime.UtcNow
                };
                _unitOfWork.MemberCustomPermissions.Create(customPermission);
            }

            await _unitOfWork.CommitAsync();
        }

        public async Task RevokePermissionAsync(int memberId, Permission permission, int revokedByMemberId, string? reason = null, CancellationToken cancellationToken = default)
        {
            var existing = await _unitOfWork.MemberCustomPermissions
                .GetByMemberAndPermissionAsync(memberId, permission);

            if (existing != null)
            {
                existing.IsGranted = false;
                existing.GrantedByMemberId = revokedByMemberId;
                existing.Reason = reason;
                existing.Updated = DateTime.UtcNow;
                _unitOfWork.MemberCustomPermissions.Update(existing);
            }
            else
            {
                var customPermission = new MemberCustomPermission
                {
                    MemberId = memberId,
                    Permission = permission,
                    IsGranted = false,
                    GrantedByMemberId = revokedByMemberId,
                    Reason = reason,
                    Created = DateTime.UtcNow
                };
                _unitOfWork.MemberCustomPermissions.Create(customPermission);
            }

            await _unitOfWork.CommitAsync();
        }

        public async Task<List<Permission>> GetRolePermissionsAsync(UserRole role, int? churchId = null, CancellationToken cancellationToken = default)
        {
            var rolePermissions = await _unitOfWork.RolePermissions
                .GetByRoleAsync(role, churchId);

            return rolePermissions
                .Where(rp => rp.IsActive)
                .Select(rp => rp.Permission)
                .ToList();
        }
    }
}
