using Microsoft.EntityFrameworkCore;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;
using MyChurch.Domain.Enum;

namespace MyChurch.Infrastructure.Repositories
{
    public class RolePermissionRepository : GenericRepository<RolePermission>, IRolePermissionRepository
    {
        public RolePermissionRepository(MyChurchDbContext context) : base(context)
        {
        }

        public async Task<List<RolePermission>> GetByRoleAsync(UserRole role, int? churchId = null)
        {
            var query = _context.Set<RolePermission>()
                .Where(rp => rp.Role == role && rp.IsActive);

            if (churchId.HasValue)
            {
                query = query.Where(rp => rp.ChurchId == churchId.Value || rp.ChurchId == null);
            }
            else
            {
                query = query.Where(rp => rp.ChurchId == null);
            }

            return await query.ToListAsync();
        }

        public async Task<List<RolePermission>> GetByChurchAsync(int churchId)
        {
            return await _context.Set<RolePermission>()
                .Where(rp => rp.ChurchId == churchId && rp.IsActive)
                .ToListAsync();
        }
    }
}
