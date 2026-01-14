using System;
using System.Linq;
using MyChurch.Domain.Entities;
using MyChurch.Domain.Enum;

namespace MyChurch.Infrastructure
{
    public static class PlatformUserSeedData
    {
        public static void SeedPlatformUsers(MyChurchDbContext context)
        {
            // Evita duplicar (idempotente)
            if (context.Set<PlatformUser>().Any())
                return;

            var platformAdmin = new PlatformUser
            {
                Name = "Platform Admin",
                Email = "platformadmin@test.com",
                Photo = null,
                IsActive = true,
                Role = UserRole.PlatformAdmin,
                Created = DateTime.UtcNow,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Test@123456", workFactor: 12)
            };

            context.Add(platformAdmin);
            context.SaveChanges();
        }
    }
}
