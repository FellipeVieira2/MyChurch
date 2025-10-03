using System;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;
using MyChurch.Domain.Services;
using Microsoft.Extensions.Logging;

namespace MyChurch.Application.Reviews.Commands.SubmitReview
{
    public class SubmitReviewCommand : IRequest<SubmitReviewResult>
    {
        public int EntityId { get; set; }
        public string EntityType { get; set; } = string.Empty;
        public int ReviewerId { get; set; }
        public int Score { get; set; }
        public string Comment { get; set; } = string.Empty;
        public bool SkipVerification { get; set; } = false; // Para testes ou admin bypass
    }

    public class SubmitReviewResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int? ReviewId { get; set; }
        public bool IsVerified { get; set; } = false;
        public System.Collections.Generic.List<string> InappropriateWordsFound { get; set; } = new();
    }

    public class SubmitReviewCommandHandler : IRequestHandler<SubmitReviewCommand, SubmitReviewResult>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IReviewVerificationService _verificationService;
        private readonly ILogger<SubmitReviewCommandHandler> _logger;

        public SubmitReviewCommandHandler(
            IUnitOfWork unitOfWork, 
            IReviewVerificationService verificationService,
            ILogger<SubmitReviewCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _verificationService = verificationService;
            _logger = logger;
        }

        public async Task<SubmitReviewResult> Handle(SubmitReviewCommand request, CancellationToken cancellationToken)
        {
            if (request.Score < 1 || request.Score > 5)
            {
                return new SubmitReviewResult { Success = false, Message = "A nota deve estar entre 1 e 5" };
            }

            bool isVerified = false;
            int? presenceId = null;

            // Verificação de presença (apenas para igrejas)
            if (request.EntityType == "Church" && !request.SkipVerification)
            {
                var (canReview, reason) = await _verificationService.CanMemberReviewChurchAsync(
                    request.ReviewerId, 
                    request.EntityId, 
                    cancellationToken);

                if (!canReview)
                {
                    _logger.LogWarning("Member {MemberId} cannot review church {ChurchId}: {Reason}", 
                        request.ReviewerId, request.EntityId, reason);
                    return new SubmitReviewResult 
                    { 
                        Success = false, 
                        Message = reason ?? "Você não pode avaliar esta igreja no momento." 
                    };
                }

                // Busca ID da presença para verificação
                presenceId = await _verificationService.GetLatestPresenceIdAsync(
                    request.ReviewerId, 
                    request.EntityId, 
                    cancellationToken);

                isVerified = presenceId.HasValue;
            }

            var review = new Review
            {
                EntityId = request.EntityId,
                EntityType = request.EntityType,
                ReviewerId = request.ReviewerId,
                Score = request.Score,
                Comment = request.Comment,
                CreatedAt = DateTime.UtcNow,
                IsVerified = isVerified,
                WorshipPresenceId = presenceId,
                VerifiedAt = isVerified ? DateTime.UtcNow : null
            };

            _unitOfWork.Reviews.Create(review);
            await _unitOfWork.CommitAsync();

            _logger.LogInformation("Review created for {EntityType} {EntityId} by member {ReviewerId}. Verified: {IsVerified}", 
                request.EntityType, request.EntityId, request.ReviewerId, isVerified);

            return new SubmitReviewResult 
            { 
                Success = true, 
                Message = isVerified 
                    ? "Avaliação verificada enviada com sucesso!" 
                    : "Avaliação enviada com sucesso", 
                ReviewId = review.Id,
                IsVerified = isVerified
            };
        }
    }
}
