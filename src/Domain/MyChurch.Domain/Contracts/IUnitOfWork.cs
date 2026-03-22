namespace MyChurch.Domain.Contracts
{
    public interface IUnitOfWork
    {
        IChurchRepository Churchs { get; }
        IDonationRepository Donations { get; }
        IEventRepository Events { get; }
        IMemberRepository Members { get; }
        IPaymentRepository Payments { get; }
        IPlanRepository Plans { get; }
        ISubscriptionRepository Subscriptions { get; }
        IAssetRepository Assets { get; }
        IEventNotificationRepository EventNotifications { get; }
        IEventRecurrenceRepository EventRecurrences { get; }
        IFeedPostRepository FeedPosts { get; }
        IFeedLikeRepository FeedLikes { get; }
        ICashFlowEntryRepository CashFlowEntries { get; }
        ICashFlowCategoryRepository CashFlowCategories { get; }
        IVersionRepository Versions { get; }
        IBookRepository Books { get; }
        IChapterRepository Chapters { get; }
        IVerseRepository Verses { get; }
        IHymnRepository Hymns { get; }
        IMemberDocumetRepository MemberDocuments { get; }
        ICreditCardInfoRepository CreditCardInfos { get; }
        IBankingInfoRepository BankingInfos { get; }
        ITransferHistoryRepository TransferHistories { get; }
        IVerseOfTheDayRepository VerseOfTheDays { get; }
        IWorshipActivityBibleRepository WorshipActivityBibles { get; }
        IWorshipActivityHymnRepository WorshipActivityHymns { get; }
        IWorshipActivityRepository WorshipActivities { get; }
        IWorshipServiceRepository WorshipServices { get; }
        IWorshipPresenceRepository WorshipPresences { get; }
        IWorshipScheduleRepository WorshipSchedules { get; }
        IWorshipScaleMemberRepository WorshipScaleMembers { get; }
        IDonationWorshipServiceRepository DonationWorshipServices { get; }
        IPrayerRequestRepository PrayerRequests { get; }
        IFeedPostImageRepository FeedPostImages { get; }
        IAdminNoticeRepository AdminNotices { get; }
        IHymnVerseRepository HymnVerses { get; }
        ICampaignRepository Campaigns { get; }
        IFamilyRepository Families { get; }
        IFamilyInvitationRepository FamilyInvitations { get; }
        IChildRepository Children { get; }
        IGroupRepository Groups { get; }
        IChildGroupAssignmentRepository ChildGroupAssignments { get; }
        IGroupMemberRepository GroupMembers { get; }
        IGroupResourceRepository GroupResources { get; }
        IJourneyRepository Journeys { get; }
        IJourneyStageRepository JourneyStages { get; }
        IMemberJourneyProgressRepository MemberJourneyProgresses { get; }
        IMemberJourneyAssignmentRepository MemberJourneyAssignments { get; }
        IAchievementRepository Achievements { get; }
        IMemberAchievementRepository MemberAchievements { get; }
        IDailyChallengeRepository DailyChallenges { get; }
        IPastoralAlertRepository PastoralAlerts { get; }
        IFaithLevelRepository FaithLevels { get; }
        IMemberFavoriteVerseRepository MemberFavoriteVerses { get; }
        IMemberConfigurationRepository MemberConfigurations { get; }
        IPreLaunchInterestRepository PreLaunchInterests { get; }
        IBibleReadingPlanRepository BibleReadingPlans { get; }
        IBibleReadingPlanStageRepository BibleReadingPlanStages { get; }
        IMemberBibleReadingProgressRepository MemberBibleReadingProgresses { get; }
        IMemberBibleReadingAssignmentRepository MemberBibleReadingAssignments { get; }
        IGroupMeetingRepository GroupMeetings { get; }
        IGroupMeetingAttendanceRepository GroupMeetingAttendances { get; }
        IGroupMeetingMemberNoteRepository GroupMeetingMemberNotes { get; }
        IUserActionHistoryRepository UserActionHistories { get; }
        ISlideRepository Slides { get; }
        IPresentationRepository Presentations { get; }
        IImportedHymnRepository ImportedHymns { get; }
        IVisitorRepository Visitors { get; }
        IVisitorStatusHistoryRepository VisitorStatusHistories { get; }
        IReviewRepository Reviews { get; }
        IReviewVoteRepository ReviewVotes { get; }
        IReviewPhotoRepository ReviewPhotos { get; }
        IReviewResponseRepository ReviewResponses { get; }
        
        // 📸 Galeria de Fotos da Igreja
        IChurchPhotoRepository ChurchPhotos { get; }
        IChurchPhotoLikeRepository ChurchPhotoLikes { get; }
        
        // 🎯 Sistema de Promoções
        IChurchPromotionRepository ChurchPromotions { get; }
        IEventPromotionRepository EventPromotions { get; }
        IEngagementEventRepository EngagementEvents { get; }
        
        // 🕐 HORÁRIOS DE CULTOS
        IChurchScheduleRepository ChurchSchedules { get; }
        
        // 🔐 SISTEMA DE PERMISSÕES GRANULARES
        IRolePermissionRepository RolePermissions { get; }
        IMemberCustomPermissionRepository MemberCustomPermissions { get; }

        IDepartmentRepository Departments { get; }
        IDepartmentMemberRepository DepartmentMembers { get; }
        IDepartmentGeneralLeaderScopeRepository DepartmentGeneralLeaderScopes { get; }
        IPlatformUserRepository PlatformUsers { get; }

        Task<bool> CommitAsync();
        Task<IDisposable> BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}
