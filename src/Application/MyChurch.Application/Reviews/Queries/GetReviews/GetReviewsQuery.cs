using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyChurch.Application.Common.Extensions;
using MyChurch.Application.Common.Models;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Enum;

namespace MyChurch.Application.Reviews.Queries.GetReviews
{
    public class GetReviewsQuery : IRequest<PaginatedList<ReviewWithVoteDto>>
    {
        public int EntityId { get; set; }
        public string EntityType { get; set; } = "Church";
        public int? MinScore { get; set; }
        public bool? IsVerified { get; set; }
        public bool? HasResponse { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SortBy { get; set; } = "CreatedAt";
        public string SortDirection { get; set; } = "desc";
        public int? CurrentMemberId { get; set; }
    }

    public class GetReviewsQueryHandler : IRequestHandler<GetReviewsQuery, PaginatedList<ReviewWithVoteDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetReviewsQueryHandler> _logger;

        public GetReviewsQueryHandler(IUnitOfWork unitOfWork, ILogger<GetReviewsQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<PaginatedList<ReviewWithVoteDto>> Handle(GetReviewsQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Getting reviews for {EntityType} {EntityId} - Page: {PageNumber}, SortBy: {SortBy}",
                request.EntityType,
                request.EntityId,
                request.PageNumber,
                request.SortBy);

            var query = _unitOfWork.Reviews.Query()
                .AsNoTracking()
                .Include(r => r.Reviewer)
                .Include(r => r.Photos)
                .Include(r => r.OfficialResponse)
                .Include(r => r.Votes)
                .Where(r => r.EntityId == request.EntityId && r.EntityType == request.EntityType);

            query = ApplyFilters(query, request);

            Dictionary<int, bool>? userVotes = null;
            if (request.CurrentMemberId.HasValue)
            {
                var reviewIds = await query.Select(r => r.Id).ToListAsync(cancellationToken);
                userVotes = await _unitOfWork.ReviewVotes.Query()
                    .Where(v => v.MemberId == request.CurrentMemberId.Value && reviewIds.Contains(v.ReviewId))
                    .ToDictionaryAsync(v => v.ReviewId, v => v.IsHelpful, cancellationToken);
            }

            query = ApplySorting(query, request);

            var totalCount = await query.CountAsync(cancellationToken);

            var reviews = await query
                .Skip((request.PageNumber - 1) * Math.Min(request.PageSize, 50))
                .Take(Math.Min(request.PageSize, 50))
                .ToListAsync(cancellationToken);

            var items = reviews.Select(r => new ReviewWithVoteDto
            {
                Id = r.Id,
                EntityId = r.EntityId,
                EntityType = r.EntityType,
                Score = r.Score,
                Comment = r.Comment,
                IsVerified = r.IsVerified,
                CreatedAt = r.CreatedAt,
                MemberName = r.Reviewer?.Name ?? "Anônimo",
                MemberId = r.ReviewerId,
                HelpfulCount = r.GetHelpfulVotesCount(),
                NotHelpfulCount = r.GetNotHelpfulVotesCount(),
                Photos = r.Photos.Select(p => new ReviewPhotoDto
                {
                    Id = p.Id,
                    ReviewId = p.ReviewId,
                    Url = p.PhotoUrl,
                    Caption = p.Caption,
                    DisplayOrder = p.DisplayOrder,
                    UploadedAt = p.UploadedAt
                }).ToList(),
                Response = r.OfficialResponse != null ? new ReviewResponseDto
                {
                    Id = r.OfficialResponse.Id,
                    ReviewId = r.OfficialResponse.ReviewId,
                    Response = r.OfficialResponse.Response,
                    ResponderId = r.OfficialResponse.ResponderId,
                    RespondedBy = r.OfficialResponse.Responder?.Name ?? "Igreja",
                    RespondedAt = r.OfficialResponse.RespondedAt
                } : null,
                HasCurrentMemberVoted = userVotes != null && userVotes.ContainsKey(r.Id),
                CurrentMemberVoteIsHelpful = userVotes != null && userVotes.ContainsKey(r.Id) 
                    ? userVotes[r.Id] 
                    : (bool?)null
            }).ToList();

            var result = new PaginatedList<ReviewWithVoteDto>(
                items,
                totalCount,
                request.PageNumber,
                Math.Min(request.PageSize, 50));

            _logger.LogInformation(
                "Retrieved {Count} reviews from {TotalCount} total",
                result.Items.Count,
                result.TotalCount);

            return result;
        }

        private static IQueryable<Domain.Entities.Review> ApplyFilters(
            IQueryable<Domain.Entities.Review> query,
            GetReviewsQuery request)
        {
            if (request.MinScore.HasValue)
                query = query.Where(r => r.Score >= request.MinScore.Value);

            if (request.IsVerified.HasValue)
                query = query.Where(r => r.IsVerified == request.IsVerified.Value);

            if (request.HasResponse.HasValue)
            {
                if (request.HasResponse.Value)
                    query = query.Where(r => r.OfficialResponse != null);
                else
                    query = query.Where(r => r.OfficialResponse == null);
            }

            return query;
        }

        private static IQueryable<Domain.Entities.Review> ApplySorting(
            IQueryable<Domain.Entities.Review> query,
            GetReviewsQuery request)
        {
            var isDescending = request.SortDirection?.ToLower() == "desc";

            return request.SortBy?.ToLower() switch
            {
                "score" => isDescending 
                    ? query.OrderByDescending(r => r.Score).ThenByDescending(r => r.CreatedAt)
                    : query.OrderBy(r => r.Score).ThenBy(r => r.CreatedAt),
                "helpful count" => isDescending 
                    ? query.OrderByDescending(r => r.Votes.Count(v => v.IsHelpful)).ThenByDescending(r => r.CreatedAt)
                    : query.OrderBy(r => r.Votes.Count(v => v.IsHelpful)).ThenBy(r => r.CreatedAt),
                _ => isDescending 
                    ? query.OrderByDescending(r => r.CreatedAt)
                    : query.OrderBy(r => r.CreatedAt)
            };
        }
    }
}
