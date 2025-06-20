using MyChurch.Domain.Contracts;
using MyChurch.Infrastructure.Repositories;
using System.Threading.Tasks;

namespace MyChurch.Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly MyChurchDbContext _context;

        public UnitOfWork(MyChurchDbContext context)
        {
            _context = context;
        }

        public IChurchRepository Churchs => new ChurchRepository(_context);
        public IDonationRepository Donations => new DonationRepository(_context);
        public IEventRepository Events => new EventRepository(_context);
        public IMemberRepository Members => new MemberRepository(_context);
        public IPaymentRepository Payments => new PaymentRepository(_context);
        public IPlanRepository Plans => new PlanRepository(_context);
        public ISubscriptionRepository Subscriptions => new SubscriptionRepository(_context);
        public IAssetRepository Assets => new AssetRepository(_context);
        public IEventNotificationRepository EventNotifications => new EventNotificationRepository(_context);
        public IEventRecurrenceRepository EventRecurrences => new EventRecurrenceRepository(_context);
        public IFeedPostRepository FeedPosts => new FeedPostRepository(_context);
        public IFeedLikeRepository FeedLikes => new FeedLikeRepository(_context);
        public ICashFlowEntryRepository CashFlowEntries => new CashFlowEntryRepository(_context);
        public ICashFlowCategoryRepository CashFlowCategories => new CashFlowCategoryRepository(_context);
        public IVersionRepository Versions => new VersionRepository(_context);
        public IBookRepository Books => new BookRepository(_context);
        public IChapterRepository Chapters => new ChapterRepository(_context);
        public IVerseRepository Verses => new VerseRepository(_context);
        public IHymnRepository Hymns => new HymnRepository(_context);
        public IMemberDocumetRepository MemberDocuments => new  MemberDocumentRepository(_context);
        public ICreditCardInfoRepository CreditCardInfos => new CreditCardInfoRepository(_context);
        public IBankingInfoRepository BankingInfos => new BankingInfoRepository(_context);
        public ITransferHistoryRepository TransferHistories => new TransferHistoryRepository(_context);
        public IVerseOfTheDayRepository VerseOfTheDays => new VerseOfTheDayRepository(_context);
        public IWorshipActivityBibleRepository WorshipActivityBibles => new WorshipActivityBibleRepository(_context);
        public IWorshipActivityHymnRepository WorshipActivityHymns => new WorshipActivityHymnRepository(_context);
        public IWorshipActivityRepository WorshipActivities => new WorshipActivityRepository(_context);
        public IWorshipServiceRepository WorshipServices => new WorshipServiceRepository(_context);
        public IWorshipPresenceRepository WorshipPresences => new WorshipPresenceRepository(_context);
        public IWorshipScheduleRepository WorshipSchedules => new WorshipScheduleRepository(_context);
        public IDonationWorshipServiceRepository DonationWorshipServices => new DonationWorshipServiceRepository(_context);
        public IPrayerRequestRepository PrayerRequests => new PrayerRequestRepository(_context);
        public IFeedPostImageRepository FeedPostImages => new FeedPostImageRepository(_context);
        public IAdminNoticeRepository AdminNotices => new AdminNoticeRepository(_context);
        public IHymnVerseRepository HymnVerses => new HymnVerseRepository(_context);
        public ICampaignRepository Campaigns => new CampaignRepository(_context);
        public IFamilyRepository Families => new FamilyRepository(_context);
        public IFamilyInvitationRepository FamilyInvitations => new FamilyInvitationRepository(_context);
        public IChildRepository Children => new ChildRepository(_context);
        public IGroupRepository Groups => new GroupRepository(_context);
        public IChildGroupAssignmentRepository ChildGroupAssignments => new ChildGroupAssignmentRepository(_context);
        public IGroupMemberRepository GroupMembers => new GroupMemberRepository(_context);
        public IGroupResourceRepository GroupResources => new GroupResourceRepository(_context);
        public IJourneyRepository Journeys => new JourneyRepository(_context);
        public IJourneyStageRepository JourneyStages => new JourneyStageRepository(_context);
        public IMemberJourneyProgressRepository MemberJourneyProgresses => new MemberJourneyProgressRepository(_context);
        public IMemberJourneyAssignmentRepository MemberJourneyAssignments => new MemberJourneyAssignmentRepository(_context);
        public IAchievementRepository Achievements => new AchievementRepository(_context);
        public IMemberAchievementRepository MemberAchievements => new MemberAchievementRepository(_context);
        public IDailyChallengeRepository DailyChallenges => new DailyChallengeRepository(_context);
        public IPastoralAlertRepository PastoralAlerts => new PastoralAlertRepository(_context);
        public IFaithLevelRepository FaithLevels => new FaithLevelRepository(_context);
        public async Task<bool> CommitAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
