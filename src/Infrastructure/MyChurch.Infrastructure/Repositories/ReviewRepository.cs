using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;

namespace MyChurch.Infrastructure.Repositories
{
    public class ReviewRepository : GenericRepository<Review>, IReviewRepository
    {
        private readonly MyChurchDbContext _context;
        public ReviewRepository(MyChurchDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<Review>> GetReviewsAsync(int entityId, string entityType, int page, int pageSize, CancellationToken cancellationToken = default)
        {
            return await _context.Reviews
                .Include(r => r.Reviewer)
                .Where(r => r.EntityId == entityId && r.EntityType == entityType)
                .OrderByDescending(r => r.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);
        }

        public async Task<double> GetAverageScoreAsync(int entityId, string entityType, CancellationToken cancellationToken = default)
        {
            return await _context.Reviews
                .Where(r => r.EntityId == entityId && r.EntityType == entityType)
                .Select(r => (double?)r.Score)
                .AverageAsync(cancellationToken) ?? 0;
        }

        public async Task<int> GetTotalReviewsAsync(int entityId, string entityType, CancellationToken cancellationToken = default)
        {
            return await _context.Reviews
                .CountAsync(r => r.EntityId == entityId && r.EntityType == entityType, cancellationToken);
        }
    }
}
