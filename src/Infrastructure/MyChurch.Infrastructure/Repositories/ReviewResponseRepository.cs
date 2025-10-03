using Microsoft.EntityFrameworkCore;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;

namespace MyChurch.Infrastructure.Repositories
{
    public class ReviewResponseRepository : GenericRepository<ReviewResponse>, IReviewResponseRepository
    {
        public ReviewResponseRepository(MyChurchDbContext context) : base(context)
        {
        }

        public async Task<ReviewResponse?> GetByReviewIdAsync(int reviewId, CancellationToken cancellationToken = default)
        {
            return await _context.Set<ReviewResponse>()
                .Include(rr => rr.Responder)
                .FirstOrDefaultAsync(rr => rr.ReviewId == reviewId, cancellationToken);
        }

        public async Task<bool> HasResponseAsync(int reviewId, CancellationToken cancellationToken = default)
        {
            return await _context.Set<ReviewResponse>()
                .AnyAsync(rr => rr.ReviewId == reviewId, cancellationToken);
        }

        public async Task<List<ReviewResponse>> GetChurchResponsesAsync(int churchId, CancellationToken cancellationToken = default)
        {
            return await _context.Set<ReviewResponse>()
                .Include(rr => rr.Review)
                .Include(rr => rr.Responder)
                .Where(rr => rr.Review.EntityId == churchId && rr.Review.EntityType == "Church")
                .OrderByDescending(rr => rr.RespondedAt)
                .ToListAsync(cancellationToken);
        }
    }
}
