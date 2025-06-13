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
        IDonationWorshipServiceRepository DonationWorshipServices { get; }
        IPrayerRequestRepository PrayerRequests { get; }
        Task<bool> CommitAsync();
    }
}
