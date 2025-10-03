using Microsoft.EntityFrameworkCore;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;

namespace MyChurch.Infrastructure.Repositories
{
    public class ReviewVoteRepository : GenericRepository<ReviewVote>, IReviewVoteRepository
    {
        public ReviewVoteRepository(MyChurchDbContext context) : base(context)
        {
        }

        public async Task<ReviewVote?> GetVoteByMemberAndReviewAsync(int memberId, int reviewId, CancellationToken cancellationToken = default)
        {
            return await _context.Set<ReviewVote>()
                .FirstOrDefaultAsync(v => v.MemberId == memberId && v.ReviewId == reviewId, cancellationToken);
        }

        public async Task<int> CountHelpfulVotesAsync(int reviewId, CancellationToken cancellationToken = default)
        {
            return await _context.Set<ReviewVote>()
                .CountAsync(v => v.ReviewId == reviewId && v.IsHelpful, cancellationToken);
        }

        public async Task<int> CountNotHelpfulVotesAsync(int reviewId, CancellationToken cancellationToken = default)
        {
            return await _context.Set<ReviewVote>()
                .CountAsync(v => v.ReviewId == reviewId && !v.IsHelpful, cancellationToken);
        }

        public async Task<bool> HasMemberVotedAsync(int memberId, int reviewId, CancellationToken cancellationToken = default)
        {
            return await _context.Set<ReviewVote>()
                .AnyAsync(v => v.MemberId == memberId && v.ReviewId == reviewId, cancellationToken);
        }
    }
}
