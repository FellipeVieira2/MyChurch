using Microsoft.EntityFrameworkCore;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;
using MyChurch.Domain.Enum;

namespace MyChurch.Infrastructure.Repositories
{
    public class MemberCustomPermissionRepository : GenericRepository<MemberCustomPermission>, IMemberCustomPermissionRepository
    {
        public MemberCustomPermissionRepository(MyChurchDbContext context) : base(context)
        {
        }

        public async Task<List<MemberCustomPermission>> GetByMemberIdAsync(int memberId)
        {
            return await _context.Set<MemberCustomPermission>()
                .Where(mcp => mcp.MemberId == memberId)
                .Where(mcp => !mcp.ExpiresAt.HasValue || mcp.ExpiresAt.Value > DateTime.UtcNow)
                .ToListAsync();
        }

        public async Task<MemberCustomPermission?> GetByMemberAndPermissionAsync(int memberId, Permission permission)
        {
            return await _context.Set<MemberCustomPermission>()
                .Where(mcp => mcp.MemberId == memberId && mcp.Permission == permission)
                .Where(mcp => !mcp.ExpiresAt.HasValue || mcp.ExpiresAt.Value > DateTime.UtcNow)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> HasPermissionAsync(int memberId, Permission permission)
        {
            var customPermission = await GetByMemberAndPermissionAsync(memberId, permission);
            return customPermission?.IsGranted ?? false;
        }
    }
}
