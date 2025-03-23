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

        Task<bool> CommitAsync();

    }
}
