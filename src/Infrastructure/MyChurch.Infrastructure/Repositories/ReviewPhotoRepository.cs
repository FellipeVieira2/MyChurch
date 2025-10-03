using Microsoft.EntityFrameworkCore;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;

namespace MyChurch.Infrastructure.Repositories
{
    public class ReviewPhotoRepository : GenericRepository<ReviewPhoto>, IReviewPhotoRepository
    {
        public ReviewPhotoRepository(MyChurchDbContext context) : base(context)
        {
        }

        public async Task<List<ReviewPhoto>> GetPhotosByReviewIdAsync(int reviewId, CancellationToken cancellationToken = default)
        {
            return await _context.Set<ReviewPhoto>()
                .Where(p => p.ReviewId == reviewId)
                .OrderBy(p => p.DisplayOrder)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<ReviewPhoto>> GetPhotosByEntityAsync(int entityId, string entityType, int limit = 100, CancellationToken cancellationToken = default)
        {
            return await _context.Set<ReviewPhoto>()
                .Include(p => p.Review)
                .Where(p => p.Review.EntityId == entityId && p.Review.EntityType == entityType)
                .OrderByDescending(p => p.UploadedAt)
                .Take(limit)
                .ToListAsync(cancellationToken);
        }

        public async Task DeletePhotosByReviewIdAsync(int reviewId, CancellationToken cancellationToken = default)
        {
            var photos = await _context.Set<ReviewPhoto>()
                .Where(p => p.ReviewId == reviewId)
                .ToListAsync(cancellationToken);

            _context.Set<ReviewPhoto>().RemoveRange(photos);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
