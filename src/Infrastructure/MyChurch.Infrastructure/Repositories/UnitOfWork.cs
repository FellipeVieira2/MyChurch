using MyChurch.Domain.Contracts;
using MyChurch.Infrastructure.Context;

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

        public UnitOfWork(IChurchRepository churchs, IDonationRepository donations, IEventRepository events, IMemberRepository members, IPaymentRepository payments, IPlanRepository plans, ISubscriptionRepository subscriptions)
        {
            Churchs = churchs;
            Donations = donations;
            Events = events;
            Members = members;
            Payments = payments;
            Plans = plans;
            Subscriptions = subscriptions;
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
