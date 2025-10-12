using Microsoft.EntityFrameworkCore;
using MyChurch.Domain.Entities;
using MyChurch.Domain.Enum;

namespace MyChurch.Infrastructure.Data.Seeders
{
    /// <summary>
    /// Seeder para popular permissões padrão no banco de dados
    /// </summary>
    public static class RolePermissionSeeder
    {
        public static async Task SeedDefaultPermissionsAsync(MyChurchDbContext context)
        {
            var defaultPermissions = DefaultRolePermissions.GetDefaultPermissions();

            foreach (var rolePermissions in defaultPermissions)
            {
                var role = rolePermissions.Key;
                var permissions = rolePermissions.Value;

                foreach (var permission in permissions)
                {
                    // Verificar se já existe (permissão global, churchId = null)
                    var exists = await context.RolePermissions
                        .AnyAsync(rp => 
                            rp.Role == role && 
                            rp.Permission == permission && 
                            rp.ChurchId == null);

                    if (!exists)
                    {
                        var rolePermission = new RolePermission
                        {
                            Role = role,
                            Permission = permission,
                            IsActive = true,
                            ChurchId = null, // Permissão global
                            Created = DateTime.UtcNow
                        };

                        context.RolePermissions.Add(rolePermission);
                    }
                }
            }

            await context.SaveChangesAsync();
        }

        /// <summary>
        /// Seed permissões específicas para uma igreja
        /// Útil quando uma igreja quer customizar as permissões padrão
        /// </summary>
        public static async Task SeedChurchPermissionsAsync(MyChurchDbContext context, int churchId)
        {
            var defaultPermissions = DefaultRolePermissions.GetDefaultPermissions();

            foreach (var rolePermissions in defaultPermissions)
            {
                var role = rolePermissions.Key;
                var permissions = rolePermissions.Value;

                foreach (var permission in permissions)
                {
                    var exists = await context.RolePermissions
                        .AnyAsync(rp => 
                            rp.Role == role && 
                            rp.Permission == permission && 
                            rp.ChurchId == churchId);

                    if (!exists)
                    {
                        var rolePermission = new RolePermission
                        {
                            Role = role,
                            Permission = permission,
                            IsActive = true,
                            ChurchId = churchId,
                            Created = DateTime.UtcNow
                        };

                        context.RolePermissions.Add(rolePermission);
                    }
                }
            }

            await context.SaveChangesAsync();
        }
    }
}
