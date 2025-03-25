using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MyChurch.Domain.Contracts;
using MyChurch.Infrastructure.Repositories;

namespace MyChurch.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection InjectInfra(this IServiceCollection services, IConfiguration configuration, string connectionString = "MyChurchDb")
        {
            services.AddScoped<IChurchRepository, ChurchRepository>();
            services.AddScoped<IDonationRepository, DonationRepository>();
            services.AddScoped<IEventRepository, EventRepository>();
            services.AddScoped<IMemberRepository, MemberRepository>();
            services.AddScoped<IPaymentRepository, PaymentRepository>();
            services.AddScoped<IPlanRepository, PlanRepository>();
            services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddDbContext<MyChurchDbContext>(options =>
            {
                options.UseNpgsql(configuration.GetConnectionString(connectionString));
#if DEBUG
                options.LogTo(Console.WriteLine, LogLevel.Information);
#endif
            });
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

            return services;
        }
    }
}
