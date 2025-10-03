using System;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;

namespace MyChurch.Application.Reviews.Commands.SubmitReview
{
    public class SubmitReviewCommand : IRequest<SubmitReviewResult>
    {
        public int EntityId { get; set; }
        public string EntityType { get; set; } = string.Empty;
        public int ReviewerId { get; set; }
        public int Score { get; set; }
        public string Comment { get; set; } = string.Empty;
    }

    public class SubmitReviewResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int? ReviewId { get; set; }
        public System.Collections.Generic.List<string> InappropriateWordsFound { get; set; } = new();
    }

    public class SubmitReviewCommandHandler : IRequestHandler<SubmitReviewCommand, SubmitReviewResult>
    {
        private readonly IUnitOfWork _unitOfWork;
        public SubmitReviewCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<SubmitReviewResult> Handle(SubmitReviewCommand request, CancellationToken cancellationToken)
        {
            if (request.Score < 1 || request.Score > 5)
            {
                return new SubmitReviewResult { Success = false, Message = "A nota deve estar entre 1 e 5" };
            }
            var review = new Review
            {
                EntityId = request.EntityId,
                EntityType = request.EntityType,
                ReviewerId = request.ReviewerId,
                Score = request.Score,
                Comment = request.Comment,
                CreatedAt = DateTime.UtcNow
            };
            _unitOfWork.Reviews.Create(review);
            await _unitOfWork.CommitAsync();
            return new SubmitReviewResult { Success = true, Message = "Avaliação enviada com sucesso", ReviewId = review.Id };
        }
    }
}
