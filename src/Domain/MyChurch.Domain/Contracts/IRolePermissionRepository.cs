using MyChurch.Domain.Entities;
using MyChurch.Domain.Enum;

namespace MyChurch.Domain.Contracts
{
    public interface IRolePermissionRepository : IGenericRepository<RolePermission>
    {
        Task<List<RolePermission>> GetByRoleAsync(UserRole role, int? churchId = null);
        Task<List<RolePermission>> GetByChurchAsync(int churchId);
    }
}
