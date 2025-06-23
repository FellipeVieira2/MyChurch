using Microsoft.EntityFrameworkCore;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;

namespace MyChurch.Infrastructure.Repositories
{
    public class PreLaunchInterestRepository : GenericRepository<PreLaunchInterest>, IPreLaunchInterestRepository
    {
        public PreLaunchInterestRepository(MyChurchDbContext context) : base(context)
        {
        }

        public async Task<bool> IsEmailRegisteredAsync(string email, CancellationToken cancellationToken = default)
        {
            return await _context.Set<PreLaunchInterest>()
                .AnyAsync(p => p.Email.ToLower() == email.ToLower(), cancellationToken);
        }

        public async Task<PreLaunchInterest> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            return await _context.Set<PreLaunchInterest>()
                .FirstOrDefaultAsync(p => p.Email.ToLower() == email.ToLower(), cancellationToken);
        }

        public async Task<PreLaunchInterest> GetByConfirmationTokenAsync(string token, CancellationToken cancellationToken = default)
        {
            return await _context.Set<PreLaunchInterest>()
                .FirstOrDefaultAsync(p => p.ConfirmationToken == token, cancellationToken);
        }
    }
}