using MyChurch.Domain.Entities;
using MyChurch.Domain.Enum;

namespace MyChurch.Domain.Contracts
{
    public interface IMemberCustomPermissionRepository : IGenericRepository<MemberCustomPermission>
    {
        Task<List<MemberCustomPermission>> GetByMemberIdAsync(int memberId);
        Task<MemberCustomPermission?> GetByMemberAndPermissionAsync(int memberId, Permission permission);
        Task<bool> HasPermissionAsync(int memberId, Permission permission);
    }
}
