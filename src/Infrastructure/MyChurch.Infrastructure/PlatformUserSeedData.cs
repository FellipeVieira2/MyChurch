using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using MyChurch.Domain.Entities;
using MyChurch.Domain.Enum;

namespace MyChurch.Infrastructure
{
    public static class PlatformUserSeedData
    {
        public static void SeedPlatformUsers(MyChurchDbContext context)
        {
            const string adminEmail = "platformadmin@test.com";

            var existing = context.Set<PlatformUser>()
                .FirstOrDefault(u => u.Email.ToLower() == adminEmail.ToLower());

            if (existing != null)
            {
                // Mantém idempotente: atualiza campos essenciais (sem recriar)
                existing.Name = string.IsNullOrWhiteSpace(existing.Name) ? "Platform Admin" : existing.Name;
                existing.IsActive = true;
                existing.Role = UserRole.PlatformAdmin;
                existing.Updated = DateTime.UtcNow;

                if (string.IsNullOrWhiteSpace(existing.PasswordHash))
                    existing.PasswordHash = BCrypt.Net.BCrypt.HashPassword("Test@123456", workFactor: 12);

                context.Update(existing);
                context.SaveChanges();
                return;
            }

            var platformAdmin = new PlatformUser
            {
                Name = "Platform Admin",
                Email = adminEmail,
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
