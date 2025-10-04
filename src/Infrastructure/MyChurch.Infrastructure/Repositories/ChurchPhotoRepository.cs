using Microsoft.EntityFrameworkCore;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;

namespace MyChurch.Infrastructure.Repositories
{
    public class ChurchPhotoRepository : GenericRepository<ChurchPhoto>, IChurchPhotoRepository
    {
        public ChurchPhotoRepository(MyChurchDbContext context) : base(context)
        {
        }

        public async Task<List<ChurchPhoto>> GetChurchPhotosAsync(
            int churchId, 
            PhotoCategory? category = null, 
            bool onlyApproved = true, 
            bool onlyFeatured = false, 
            int page = 1, 
            int pageSize = 20, 
            CancellationToken cancellationToken = default)
        {
            var query = _context.Set<ChurchPhoto>()
                .Include(p => p.UploadedBy)
                .Include(p => p.UploadedByVisitor)
                .Where(p => p.ChurchId == churchId);

            if (onlyApproved)
                query = query.Where(p => p.IsApproved && !p.IsRejected);

            if (onlyFeatured)
                query = query.Where(p => p.IsFeatured);

            if (category.HasValue)
                query = query.Where(p => p.Category == category.Value);

            return await query
                .OrderByDescending(p => p.IsFeatured)
                .ThenBy(p => p.DisplayOrder)
                .ThenByDescending(p => p.Likes)
                .ThenByDescending(p => p.UploadedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<ChurchPhoto>> GetPendingPhotosAsync(
            int churchId, 
            CancellationToken cancellationToken = default)
        {
            return await _context.Set<ChurchPhoto>()
                .Include(p => p.UploadedBy)
                .Include(p => p.UploadedByVisitor)
                .Where(p => p.ChurchId == churchId && !p.IsApproved && !p.IsRejected)
                .OrderBy(p => p.UploadedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<int> GetTotalPhotosCountAsync(
            int churchId, 
            PhotoCategory? category = null, 
            bool onlyApproved = true, 
            CancellationToken cancellationToken = default)
        {
            var query = _context.Set<ChurchPhoto>()
                .Where(p => p.ChurchId == churchId);

            if (onlyApproved)
                query = query.Where(p => p.IsApproved && !p.IsRejected);

            if (category.HasValue)
                query = query.Where(p => p.Category == category.Value);

            return await query.CountAsync(cancellationToken);
        }

        public async Task<ChurchPhoto?> GetPhotoWithDetailsAsync(
            int photoId, 
            CancellationToken cancellationToken = default)
        {
            return await _context.Set<ChurchPhoto>()
                .Include(p => p.Church)
                .Include(p => p.UploadedBy)
                .Include(p => p.UploadedByVisitor)
                .Include(p => p.ApprovedByAdmin)
                .Include(p => p.PhotoLikes)
                .FirstOrDefaultAsync(p => p.Id == photoId, cancellationToken);
        }
    }
    
    public class ChurchPhotoLikeRepository : GenericRepository<ChurchPhotoLike>, IChurchPhotoLikeRepository
    {
        public ChurchPhotoLikeRepository(MyChurchDbContext context) : base(context)
        {
        }

        public async Task<bool> HasUserLikedPhotoAsync(
            int photoId, 
            int? memberId, 
            int? visitorId, 
            CancellationToken cancellationToken = default)
        {
            return await _context.Set<ChurchPhotoLike>()
                .AnyAsync(l => 
                    l.ChurchPhotoId == photoId && 
                    ((memberId.HasValue && l.MemberId == memberId.Value) ||
                     (visitorId.HasValue && l.VisitorId == visitorId.Value)),
                    cancellationToken);
        }

        public async Task<ChurchPhotoLike?> GetUserLikeAsync(
            int photoId, 
            int? memberId, 
            int? visitorId, 
            CancellationToken cancellationToken = default)
        {
            return await _context.Set<ChurchPhotoLike>()
                .FirstOrDefaultAsync(l => 
                    l.ChurchPhotoId == photoId && 
                    ((memberId.HasValue && l.MemberId == memberId.Value) ||
                     (visitorId.HasValue && l.VisitorId == visitorId.Value)),
                    cancellationToken);
        }
    }
}
