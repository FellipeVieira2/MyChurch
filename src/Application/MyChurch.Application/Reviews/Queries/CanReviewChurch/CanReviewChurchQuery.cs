using MediatR;
using Microsoft.EntityFrameworkCore;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Services;

namespace MyChurch.Application.Reviews.Queries.CanReviewChurch
{
    /// <summary>
    /// Verifica se um membro pode avaliar uma igreja
    /// </summary>
    public class CanReviewChurchQuery : JwtMemberDto, IRequest<CanReviewChurchResult>
    {
        public int ChurchId { get; set; }
    }

    public class CanReviewChurchResult
    {
        public bool CanReview { get; set; }
        public string? Reason { get; set; }
        public bool HasVisited { get; set; }
        public DateTime? LastVisitDate { get; set; }
        public bool HasExistingReview { get; set; }
    }

    public class CanReviewChurchQueryHandler : IRequestHandler<CanReviewChurchQuery, CanReviewChurchResult>
    {
        private readonly IReviewVerificationService _verificationService;
        private readonly Domain.Contracts.IUnitOfWork _unitOfWork;

        public CanReviewChurchQueryHandler(
            IReviewVerificationService verificationService,
            Domain.Contracts.IUnitOfWork unitOfWork)
        {
            _verificationService = verificationService;
            _unitOfWork = unitOfWork;
        }

        public async Task<CanReviewChurchResult> Handle(CanReviewChurchQuery request, CancellationToken cancellationToken)
        {
            var (canReview, reason) = await _verificationService.CanMemberReviewChurchAsync(
                request.UserId, 
                request.ChurchId, 
                cancellationToken);

            var hasVisited = await _verificationService.HasMemberVisitedChurchAsync(
                request.UserId, 
                request.ChurchId, 
                cancellationToken);

            var latestPresence = await _unitOfWork.WorshipPresences.Query()
                .Where(wp => 
                    wp.MemberId == request.UserId &&
                    _unitOfWork.WorshipServices.Query()
                        .Any(ws => ws.Id == wp.WorshipServiceId && ws.ChurchId == request.ChurchId))
                .OrderByDescending(wp => wp.Timestamp)
                .Select(wp => (DateTime?)wp.Timestamp)
                .FirstOrDefaultAsync(cancellationToken);

            var hasExistingReview = await _unitOfWork.Reviews.Query()
                .AnyAsync(r => 
                    r.ReviewerId == request.UserId && 
                    r.EntityId == request.ChurchId && 
                    r.EntityType == "Church",
                    cancellationToken);

            return new CanReviewChurchResult
            {
                CanReview = canReview,
                Reason = reason,
                HasVisited = hasVisited,
                LastVisitDate = latestPresence,
                HasExistingReview = hasExistingReview
            };
        }
    }
}
