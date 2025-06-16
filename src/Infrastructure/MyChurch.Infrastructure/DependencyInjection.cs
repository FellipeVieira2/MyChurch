using Amazon.S3;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MyChurch.Domain.Contracts;
using MyChurch.Infrastructure.Repositories;
using MyChurch.Infrastructure.Utils.S3;

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
            services.AddScoped<IAssetRepository, AssetRepository>();
            services.AddScoped<IEventNotificationRepository, EventNotificationRepository>();
            services.AddScoped<IEventRecurrenceRepository, EventRecurrenceRepository>();
            services.AddScoped<IFeedPostRepository, FeedPostRepository>();
            services.AddScoped<IFeedLikeRepository, FeedLikeRepository>();
            services.AddScoped<ICashFlowEntryRepository, CashFlowEntryRepository>();
            services.AddScoped<ICashFlowCategoryRepository, CashFlowCategoryRepository>();
            services.AddScoped<IHymnRepository, HymnRepository>();
            services.AddScoped<IVersionRepository, VersionRepository>();
            services.AddScoped<IBookRepository, BookRepository>();
            services.AddScoped<IChapterRepository, ChapterRepository>();
            services.AddScoped<IVerseRepository, VerseRepository>();
            services.AddScoped<IMemberDocumetRepository, MemberDocumentRepository>();
            services.AddScoped<ICreditCardInfoRepository, CreditCardInfoRepository>();
            services.AddScoped<IBankingInfoRepository, BankingInfoRepository>();
            services.AddScoped<ITransferHistoryRepository, TransferHistoryRepository>();
            services.AddScoped<IVerseOfTheDayRepository, VerseOfTheDayRepository>();
            services.AddScoped<IWorshipActivityBibleRepository, WorshipActivityBibleRepository>();
            services.AddScoped<IWorshipActivityHymnRepository, WorshipActivityHymnRepository>();
            services.AddScoped<IWorshipActivityRepository, WorshipActivityRepository>();
            services.AddScoped<IWorshipPresenceRepository, WorshipPresenceRepository>();
            services.AddScoped<IWorshipServiceRepository, WorshipServiceRepository>();
            services.AddScoped<IWorshipScheduleRepository, WorshipScheduleRepository>();
            services.AddScoped<IDonationWorshipServiceRepository, DonationWorshipServiceRepository>();
            services.AddScoped<IFeedPostImageRepository, FeedPostImageRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IPrayerRequestRepository, PrayerRequestRepository>();
            services.AddScoped<IAdminNoticeRepository, AdminNoticeRepository>();
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

        public static IServiceCollection InjectS3(this IServiceCollection services, IConfiguration configuration)
        {
            S3Settings aWSS3Config = new S3Settings()
            {
                AccessKey = configuration["S3Settings:AccessKey"],
                BucketName = configuration["S3Settings:BucketName"],
                Region = configuration["S3Settings:Region"],
                SecretKey = configuration["S3Settings:SecretKey"]
            };
            services.AddSingleton(aWSS3Config);

            services.AddScoped<IS3Helper, S3Helper>();
            return services;
        }
    }
}
