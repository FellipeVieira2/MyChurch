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
        public IFeedPostImageRepository FeedPostImages { get; }
        public IAdminNoticeRepository AdminNotices { get; }
        public IHymnVerseRepository HymnVerses { get; }
        public ICampaignRepository Campaigns { get; }
        public IFamilyRepository Families { get; }
        public IFamilyInvitationRepository FamilyInvitations { get; }
        public IChildRepository Children { get; }
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
        public IFaithLevelRepository FaithLevels { get; private set; }
        public IMemberFavoriteVerseRepository MemberFavoriteVerses { get; }
        public IMemberConfigurationRepository MemberConfigurations { get; }
        public IPreLaunchInterestRepository PreLaunchInterests { get; }
        public IBibleReadingPlanRepository BibleReadingPlans { get; }
        public IBibleReadingPlanStageRepository BibleReadingPlanStages { get; }
        public IMemberBibleReadingProgressRepository MemberBibleReadingProgresses { get; }
        public IGroupMeetingRepository GroupMeetings { get; }
        public IGroupMeetingAttendanceRepository GroupMeetingAttendances { get; }
        public IGroupMeetingMemberNoteRepository GroupMeetingMemberNotes { get; }
        public IMemberBibleReadingAssignmentRepository MemberBibleReadingAssignments { get; }
        public IUserActionHistoryRepository UserActionHistories { get; }
        public IPresentationRepository Presentations { get; }
        public ISlideRepository Slides { get; }
        public IImportedHymnRepository ImportedHymns { get; }

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
            IPrayerRequestRepository prayerRequests,
            IFeedPostImageRepository feedPostImages,
            IAdminNoticeRepository adminNotices,
            IHymnVerseRepository hymnVerses,
            ICampaignRepository campaigns,
            IFamilyRepository families,
            IFamilyInvitationRepository familyInvitations,
            IChildRepository children,
            IGroupRepository groups,
            IChildGroupAssignmentRepository childGroupAssignments,
            IGroupMemberRepository groupMembers,
            IGroupResourceRepository groupResources,
            IJourneyRepository journeys,
            IJourneyStageRepository journeyStages,
            IMemberJourneyProgressRepository memberJourneyProgresses,
            IMemberJourneyAssignmentRepository memberJourneyAssignments,
            IAchievementRepository achievements,
            IMemberAchievementRepository memberAchievements,
            IDailyChallengeRepository dailyChallenges,
            IPastoralAlertRepository pastoralAlerts,
            IFaithLevelRepository faithLevels,
            IMemberFavoriteVerseRepository memberFavoriteVerses,
            IMemberConfigurationRepository memberConfigurations,
            IPreLaunchInterestRepository preLaunchInterests,
            IBibleReadingPlanRepository bibleReadingPlans,
            IBibleReadingPlanStageRepository bibleReadingPlanStages,
            IMemberBibleReadingProgressRepository memberBibleReadingProgresses,
            IGroupMeetingRepository groupMeetings,
            IGroupMeetingAttendanceRepository groupMeetingAttendances,
            IGroupMeetingMemberNoteRepository groupMeetingMemberNotes,
            IMemberBibleReadingAssignmentRepository memberBibleReadingAssignments,
            IUserActionHistoryRepository userActionHistories,
            IPresentationRepository presentations,
            ISlideRepository slides,
            IImportedHymnRepository importedHymns)
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
            FeedPostImages = feedPostImages;
            AdminNotices = adminNotices;
            HymnVerses = hymnVerses;
            Campaigns = campaigns;
            Families = families;
            FamilyInvitations = familyInvitations;
            Children = children;
            Groups = groups;
            ChildGroupAssignments = childGroupAssignments;
            GroupMembers = groupMembers;
            GroupResources = groupResources;
            Journeys = journeys;
            JourneyStages = journeyStages;
            MemberJourneyProgresses = memberJourneyProgresses;
            MemberJourneyAssignments = memberJourneyAssignments;
            Achievements = achievements;
            MemberAchievements = memberAchievements;
            DailyChallenges = dailyChallenges;
            PastoralAlerts = pastoralAlerts;
            FaithLevels = faithLevels;
            MemberFavoriteVerses = memberFavoriteVerses;
            MemberConfigurations = memberConfigurations;
            PreLaunchInterests = preLaunchInterests;
            BibleReadingPlans = bibleReadingPlans;
            BibleReadingPlanStages = bibleReadingPlanStages;
            MemberBibleReadingProgresses = memberBibleReadingProgresses;
            GroupMeetings = groupMeetings;
            GroupMeetingAttendances = groupMeetingAttendances;
            GroupMeetingMemberNotes = groupMeetingMemberNotes;
            MemberBibleReadingAssignments = memberBibleReadingAssignments;
            UserActionHistories = userActionHistories;
            Presentations = presentations;
            Slides = slides;
            ImportedHymns = importedHymns;
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
