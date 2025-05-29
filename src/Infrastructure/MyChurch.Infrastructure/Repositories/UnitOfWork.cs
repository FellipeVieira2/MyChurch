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
            ICashFlowCategoryRepository cashFlowCategories)
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
