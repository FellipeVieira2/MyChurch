using Microsoft.EntityFrameworkCore.Storage;
using MyChurch.Domain.Contracts;

namespace MyChurch.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly MyChurchDbContext _context;
        private IDbContextTransaction? _currentTransaction;

        public IChurchRepository Churchs { get; }
        public IDonationRepository Donations { get; }
        public IEventRepository Events { get; }
        public IDiaconateScaleMemberRepository DiaconateScaleMembers { get; }
        public IKidsScaleMemberRepository KidsScaleMembers { get; }
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
        public IWorshipActivityBibleRepository WorshipActivityBibles { get; }
        public IWorshipActivityHymnRepository WorshipActivityHymns { get; }
        public IWorshipActivityRepository WorshipActivities { get; }
        public IWorshipServiceRepository WorshipServices { get; }
        public IWorshipPresenceRepository WorshipPresences { get; }
        public IWorshipScheduleRepository WorshipSchedules { get; }
        public IWorshipScaleMemberRepository WorshipScaleMembers { get; }
        public IDonationWorshipServiceRepository DonationWorshipServices { get; }
        public IPrayerRequestRepository PrayerRequests { get; }
        public IFeedPostImageRepository FeedPostImages { get; }
        public IAdminNoticeRepository AdminNotices { get; }
        public IHymnVerseRepository HymnVerses { get; }
        public ICampaignRepository Campaigns { get; }
        public IFamilyRepository Families { get; }
        public IFamilyInvitationRepository FamilyInvitations { get; }
        public IChildRepository Children { get; }
        public IChildPickupAuthorizationRepository ChildPickupAuthorizations { get; }
        public IKidsCheckInRepository KidsCheckIns { get; }
        public IGroupRepository Groups { get; }
        public IChildGroupAssignmentRepository ChildGroupAssignments { get; }
        public IGroupMemberRepository GroupMembers { get; }
        public IGroupResourceRepository GroupResources { get; }
        public IJourneyRepository Journeys { get; }
        public IJourneyStageRepository JourneyStages { get; }
        public IMemberJourneyProgressRepository MemberJourneyProgresses { get; }
        public IMemberJourneyAssignmentRepository MemberJourneyAssignments { get; }
        public IAchievementRepository Achievements { get; }
        public IMemberAchievementRepository MemberAchievements { get; }
        public IDailyChallengeRepository DailyChallenges { get; }
        public IPastoralAlertRepository PastoralAlerts { get; }
        public IFaithLevelRepository FaithLevels { get; }
        public IMemberFavoriteVerseRepository MemberFavoriteVerses { get; }
        public IMemberConfigurationRepository MemberConfigurations { get; }
        public IPreLaunchInterestRepository PreLaunchInterests { get; }
        public IBibleReadingPlanRepository BibleReadingPlans { get; }
        public IBibleReadingPlanStageRepository BibleReadingPlanStages { get; }
        public IMemberBibleReadingProgressRepository MemberBibleReadingProgresses { get; }
        public IMemberBibleReadingAssignmentRepository MemberBibleReadingAssignments { get; }
        public IGroupMeetingRepository GroupMeetings { get; }
        public IGroupMeetingAttendanceRepository GroupMeetingAttendances { get; }
        public IGroupMeetingMemberNoteRepository GroupMeetingMemberNotes { get; }
        public IUserActionHistoryRepository UserActionHistories { get; }
        public IPresentationRepository Presentations { get; }
        public ISlideRepository Slides { get; }
        public IImportedHymnRepository ImportedHymns { get; }
        public IVisitorRepository Visitors { get; }
        public IVisitorStatusHistoryRepository VisitorStatusHistories { get; }
        public IReviewRepository Reviews { get; }
        public IReviewVoteRepository ReviewVotes { get; }
        public IReviewPhotoRepository ReviewPhotos { get; }
        public IReviewResponseRepository ReviewResponses { get; }

        public IChurchPhotoRepository ChurchPhotos { get; }
        public IChurchPhotoLikeRepository ChurchPhotoLikes { get; }

        public IChurchPromotionRepository ChurchPromotions { get; }
        public IEventPromotionRepository EventPromotions { get; }
        public IEngagementEventRepository EngagementEvents { get; }

        public IChurchScheduleRepository ChurchSchedules { get; }

        public IRolePermissionRepository RolePermissions { get; }
        public IMemberCustomPermissionRepository MemberCustomPermissions { get; }

        public IDepartmentRepository Departments { get; }
        public IDepartmentMemberRepository DepartmentMembers { get; }
        public IDepartmentGeneralLeaderScopeRepository DepartmentGeneralLeaderScopes { get; }

        public IPlatformUserRepository PlatformUsers { get; }

        public UnitOfWork(MyChurchDbContext context)
        {
            _context = context;

            Churchs = new ChurchRepository(_context);
            Donations = new DonationRepository(_context);
            Events = new EventRepository(_context);
            DiaconateScaleMembers = new DiaconateScaleMemberRepository(_context);
            KidsScaleMembers = new KidsScaleMemberRepository(_context);
            Members = new MemberRepository(_context);
            Payments = new PaymentRepository(_context);
            Plans = new PlanRepository(_context);
            Subscriptions = new SubscriptionRepository(_context);
            Assets = new AssetRepository(_context);
            EventNotifications = new EventNotificationRepository(_context);
            EventRecurrences = new EventRecurrenceRepository(_context);
            FeedPosts = new FeedPostRepository(_context);
            FeedLikes = new FeedLikeRepository(_context);
            CashFlowEntries = new CashFlowEntryRepository(_context);
            CashFlowCategories = new CashFlowCategoryRepository(_context);
            Verses = new VerseRepository(_context);
            Versions = new VersionRepository(_context);
            Books = new BookRepository(_context);
            Chapters = new ChapterRepository(_context);
            Hymns = new HymnRepository(_context);
            MemberDocuments = new MemberDocumentRepository(_context);
            CreditCardInfos = new CreditCardInfoRepository(_context);
            BankingInfos = new BankingInfoRepository(_context);
            TransferHistories = new TransferHistoryRepository(_context);
            VerseOfTheDays = new VerseOfTheDayRepository(_context);
            WorshipActivityBibles = new WorshipActivityBibleRepository(_context);
            WorshipActivityHymns = new WorshipActivityHymnRepository(_context);
            WorshipActivities = new WorshipActivityRepository(_context);
            WorshipServices = new WorshipServiceRepository(_context);
            WorshipPresences = new WorshipPresenceRepository(_context);
            WorshipSchedules = new WorshipScheduleRepository(_context);
            WorshipScaleMembers = new WorshipScaleMemberRepository(_context);
            DonationWorshipServices = new DonationWorshipServiceRepository(_context);
            PrayerRequests = new PrayerRequestRepository(_context);
            FeedPostImages = new FeedPostImageRepository(_context);
            AdminNotices = new AdminNoticeRepository(_context);
            HymnVerses = new HymnVerseRepository(_context);
            Campaigns = new CampaignRepository(_context);
            Families = new FamilyRepository(_context);
            FamilyInvitations = new FamilyInvitationRepository(_context);
            Children = new ChildRepository(_context);
            ChildPickupAuthorizations = new ChildPickupAuthorizationRepository(_context);
            KidsCheckIns = new KidsCheckInRepository(_context);
            Groups = new GroupRepository(_context);
            ChildGroupAssignments = new ChildGroupAssignmentRepository(_context);
            GroupMembers = new GroupMemberRepository(_context);
            GroupResources = new GroupResourceRepository(_context);
            Journeys = new JourneyRepository(_context);
            JourneyStages = new JourneyStageRepository(_context);
            MemberJourneyProgresses = new MemberJourneyProgressRepository(_context);
            MemberJourneyAssignments = new MemberJourneyAssignmentRepository(_context);
            Achievements = new AchievementRepository(_context);
            MemberAchievements = new MemberAchievementRepository(_context);
            DailyChallenges = new DailyChallengeRepository(_context);
            PastoralAlerts = new PastoralAlertRepository(_context);
            FaithLevels = new FaithLevelRepository(_context);
            MemberFavoriteVerses = new MemberFavoriteVerseRepository(_context);
            MemberConfigurations = new MemberConfigurationRepository(_context);
            PreLaunchInterests = new PreLaunchInterestRepository(_context);
            BibleReadingPlans = new BibleReadingPlanRepository(_context);
            BibleReadingPlanStages = new BibleReadingPlanStageRepository(_context);
            MemberBibleReadingProgresses = new MemberBibleReadingProgressRepository(_context);
            MemberBibleReadingAssignments = new MemberBibleReadingAssignmentRepository(_context);
            GroupMeetings = new GroupMeetingRepository(_context);
            GroupMeetingAttendances = new GroupMeetingAttendanceRepository(_context);
            GroupMeetingMemberNotes = new GroupMeetingMemberNoteRepository(_context);
            UserActionHistories = new UserActionHistoryRepository(_context);
            Presentations = new PresentationRepository(_context);
            Slides = new SlideRepository(_context);
            ImportedHymns = new ImportedHymnRepository(_context);
            Visitors = new VisitorRepository(_context);
            VisitorStatusHistories = new VisitorStatusHistoryRepository(_context);
            Reviews = new ReviewRepository(_context);
            ReviewVotes = new ReviewVoteRepository(_context);
            ReviewPhotos = new ReviewPhotoRepository(_context);
            ReviewResponses = new ReviewResponseRepository(_context);

            ChurchPhotos = new ChurchPhotoRepository(_context);
            ChurchPhotoLikes = new ChurchPhotoLikeRepository(_context);

            ChurchPromotions = new ChurchPromotionRepository(_context);
            EventPromotions = new EventPromotionRepository(_context);
            EngagementEvents = new EngagementEventRepository(_context);

            ChurchSchedules = new ChurchScheduleRepository(_context);

            RolePermissions = new RolePermissionRepository(_context);
            MemberCustomPermissions = new MemberCustomPermissionRepository(_context);

            Departments = new DepartmentRepository(_context);
            DepartmentMembers = new DepartmentMemberRepository(_context);
            DepartmentGeneralLeaderScopes = new DepartmentGeneralLeaderScopeRepository(_context);
            PlatformUsers = new PlatformUserRepository(_context);
        }

        public async Task<IDisposable> BeginTransactionAsync()
        {
            _currentTransaction = await _context.Database.BeginTransactionAsync();
            return _currentTransaction;
        }

        public async Task CommitTransactionAsync()
        {
            if (_currentTransaction != null)
            {
                await _currentTransaction.CommitAsync();
                await _currentTransaction.DisposeAsync();
                _currentTransaction = null;
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_currentTransaction != null)
            {
                await _currentTransaction.RollbackAsync();
                await _currentTransaction.DisposeAsync();
                _currentTransaction = null;
            }
        }

        public async Task<bool> CommitAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public void Dispose()
        {
            _context.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
