using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MyChurch.Domain.Contracts;
using Mychurch.Common.Utils.Objects;

namespace MyChurch.Application.Reviews.Queries.GetReviews
{
    public class GetReviewsQuery : IRequest<ReviewsResultDto>
    {
        public int EntityId { get; set; }
        public string EntityType { get; set; } = string.Empty;
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class ReviewsResultDto
    {
        public double AverageScore { get; set; }
        public int TotalReviews { get; set; }
        public PagedResultDto<ReviewDto> Reviews { get; set; } = new();
    }

    public class ReviewDto
    {
        public int Id { get; set; }
        public int ReviewerId { get; set; }
        public string ReviewerName { get; set; } = string.Empty;
        public string? ReviewerPhoto { get; set; }
        public int Score { get; set; }
        public string Comment { get; set; } = string.Empty;
        public System.DateTime CreatedAt { get; set; }
        public int HelpfulVotes { get; set; }
        public int NotHelpfulVotes { get; set; }
        public int HelpfulnessScore { get; set; }
    }

    public class GetReviewsQueryHandler : IRequestHandler<GetReviewsQuery, ReviewsResultDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        public GetReviewsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<ReviewsResultDto> Handle(GetReviewsQuery request, CancellationToken cancellationToken)
        {
            var query = _unitOfWork.Reviews.Query()
                .Where(r => r.EntityId == request.EntityId && r.EntityType == request.EntityType)
                .Include(r => r.Reviewer)
                .Include(r => r.Votes)
                .AsNoTracking();
            var totalReviews = await query.CountAsync(cancellationToken);
            var averageScore = totalReviews > 0 ? await query.AverageAsync(r => r.Score, cancellationToken) : 0;
            var reviews = await query
                .OrderByDescending(r => r.CreatedAt)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(r => new ReviewDto
                {
                    Id = r.Id,
                    ReviewerId = r.ReviewerId,
                    ReviewerName = r.Reviewer != null ? r.Reviewer.Name : string.Empty,
                    ReviewerPhoto = r.Reviewer != null ? r.Reviewer.Photo : null,
                    Score = r.Score,
                    Comment = r.Comment,
                    CreatedAt = r.CreatedAt,
                    HelpfulVotes = r.Votes.Count(v => v.IsHelpful),
                    NotHelpfulVotes = r.Votes.Count(v => !v.IsHelpful),
                    HelpfulnessScore = r.Votes.Count(v => v.IsHelpful) - r.Votes.Count(v => !v.IsHelpful)
                })
                .ToListAsync(cancellationToken);
            return new ReviewsResultDto
            {
                AverageScore = System.Math.Round(averageScore, 1),
                TotalReviews = totalReviews,
                Reviews = new PagedResultDto<ReviewDto>(reviews, request.Page, request.PageSize, totalReviews)
            };
        }
    }
}
