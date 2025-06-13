using MyChurch.Domain.Contracts;

namespace MyChurch.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly MyChurchDbContext _context;

        public IChurchRepository Churchs { get; }        

        public IDonationRepository Donations { get; }

        public IEventRepository Events { get; }

        public IMemberRepository Members { get; }

        public IPaymentRepository Payments { get; }

        public IPlanRepository Plans { get; }

        public ISubscriptionRepository Subscriptions { get; }

        public IAssetRepository Assets { get; }

        public IEventNotificationRepository EventNotifications { get; }
        public IEventRecurrenceRepository EventRecurrences { get; }
        public IFeedPostRepository FeedPosts { get; }
        public IFeedLikeRepository FeedLikes { get; }
        public ICashFlowEntryRepository CashFlowEntries { get; }
        public ICashFlowCategoryRepository CashFlowCategories { get; }
        public IVerseRepository Verses { get; }
        public IVersionRepository Versions { get; }
        public IBookRepository Books { get; }
        public IChapterRepository Chapters { get; }
        public IHymnRepository Hymns { get; }
        public IMemberDocumetRepository MemberDocuments { get; }
        public ICreditCardInfoRepository CreditCardInfos { get; }
        public IBankingInfoRepository BankingInfos { get; }
        public ITransferHistoryRepository TransferHistories { get; }
        public IVerseOfTheDayRepository VerseOfTheDays { get; }
        // Repositories for worship activities
        public IWorshipActivityBibleRepository WorshipActivityBibles { get; }
        public IWorshipActivityHymnRepository WorshipActivityHymns { get; }
        public IWorshipActivityRepository WorshipActivities { get; }
        public IWorshipServiceRepository WorshipServices { get; }
        public IWorshipPresenceRepository WorshipPresences { get; }
        public IWorshipScheduleRepository WorshipSchedules { get; }
        public IDonationWorshipServiceRepository DonationWorshipServices { get; }
        public IPrayerRequestRepository PrayerRequests { get; }
        public UnitOfWork(
            IChurchRepository churchs,
            IDonationRepository donations,
            IEventRepository events,
            IMemberRepository members,
            IPaymentRepository payments,
            IPlanRepository plans,
            ISubscriptionRepository subscriptions,
            MyChurchDbContext context,
            IAssetRepository assets,
            IEventNotificationRepository eventNotifications,
            IEventRecurrenceRepository eventRecurrences,
            IFeedPostRepository feedPosts,
            IFeedLikeRepository feedLikes,
            ICashFlowEntryRepository cashFlowEntries,
            ICashFlowCategoryRepository cashFlowCategories,
            IVerseRepository verses,
            IVersionRepository versions,
            IBookRepository books,
            IChapterRepository chapters,
            IHymnRepository hymns,
            IMemberDocumetRepository memberDocuments,
            ICreditCardInfoRepository creditCardInfos,
            IBankingInfoRepository bankingInfos,
            ITransferHistoryRepository transferHistories,
            IVerseOfTheDayRepository verseOfTheDays,
            IWorshipActivityBibleRepository worshipActivityBibles,
            IWorshipActivityHymnRepository worshipActivityHymns,
            IWorshipActivityRepository worshipActivities,
            IWorshipServiceRepository worshipServices,
            IWorshipPresenceRepository worshipPresences,
            IWorshipScheduleRepository worshipSchedules,
            IDonationWorshipServiceRepository donationWorshipServices,
            IPrayerRequestRepository prayerRequests)
        {
            Churchs = churchs;
            Donations = donations;
            Events = events;
            Members = members;
            Payments = payments;
            Plans = plans;
            Subscriptions = subscriptions;
            _context = context;
            Assets = assets;
            EventNotifications = eventNotifications;
            EventRecurrences = eventRecurrences;
            FeedPosts = feedPosts;
            FeedLikes = feedLikes;
            CashFlowEntries = cashFlowEntries;
            CashFlowCategories = cashFlowCategories;
            Verses = verses;
            Versions = versions;
            Books = books;
            Chapters = chapters;
            Hymns = hymns;
            MemberDocuments = memberDocuments;
            CreditCardInfos = creditCardInfos;
            BankingInfos = bankingInfos;
            TransferHistories = transferHistories;
            VerseOfTheDays = verseOfTheDays;
            WorshipActivityBibles = worshipActivityBibles;
            WorshipActivityHymns = worshipActivityHymns;
            WorshipActivities = worshipActivities;
            WorshipServices = worshipServices;
            WorshipPresences = worshipPresences;
            WorshipSchedules = worshipSchedules;
            DonationWorshipServices = donationWorshipServices;
            PrayerRequests = prayerRequests;
        }

        public async Task<bool> CommitAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context.Dispose();
            }
        }
    }
}
