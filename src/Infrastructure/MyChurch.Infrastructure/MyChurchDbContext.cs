using Microsoft.EntityFrameworkCore;
using MyChurch.Domain.Entities;

namespace MyChurch.Infrastructure
{
    public class MyChurchDbContext : DbContext
    {
        public MyChurchDbContext(DbContextOptions<MyChurchDbContext> options) : base(options) { }

        public DbSet<DonationWorshipService> DonationWorshipServices { get; set; }
        public DbSet<PrayerRequest> PrayerRequests { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasPostgresExtension("unaccent");
            modelBuilder.HasPostgresExtension("uuid-ossp");
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(MyChurchDbContext).Assembly);
        }
        [DbFunction("unaccent")]
        public string Unaccent()
        {
            throw new NotSupportedException();
        }
    }
}
