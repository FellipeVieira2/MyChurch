using System;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;
using MyChurch.Domain.Services;
using Microsoft.Extensions.Logging;
using MyChurch.Infrastructure.Utils.S3;

namespace MyChurch.Application.Reviews.Commands.SubmitReview
{
    public class SubmitReviewCommand : IRequest<SubmitReviewResult>
    {
        public int EntityId { get; set; }
        public string EntityType { get; set; } = string.Empty;
        public int ReviewerId { get; set; }
        public int Score { get; set; }
        public string Comment { get; set; } = string.Empty;
        public bool SkipVerification { get; set; } = false;
        
        /// <summary>
        /// Lista de fotos em base64 para upload
        /// </summary>
        public List<ReviewPhotoDto>? Photos { get; set; }
    }

    public class ReviewPhotoDto
    {
        public string PhotoBase64 { get; set; } = string.Empty;
        public string? Caption { get; set; }
        public string? OriginalFileName { get; set; }
    }

    public class SubmitReviewResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int? ReviewId { get; set; }
        public bool IsVerified { get; set; } = false;
        public int PhotosUploaded { get; set; } = 0;
        public System.Collections.Generic.List<string> InappropriateWordsFound { get; set; } = new();
    }

    public class SubmitReviewCommandHandler : IRequestHandler<SubmitReviewCommand, SubmitReviewResult>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IReviewVerificationService _verificationService;
        private readonly ILogger<SubmitReviewCommandHandler> _logger;
        private readonly IS3Helper _s3Helper;

        public SubmitReviewCommandHandler(
            IUnitOfWork unitOfWork, 
            IReviewVerificationService verificationService,
            ILogger<SubmitReviewCommandHandler> logger,
            IS3Helper s3Helper)
        {
            _unitOfWork = unitOfWork;
            _verificationService = verificationService;
            _logger = logger;
            _s3Helper = s3Helper;
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

            // Upload de fotos
            int photosUploaded = 0;
            if (request.Photos != null && request.Photos.Any())
            {
                foreach (var photoDto in request.Photos.Take(5)) // Limita a 5 fotos por review
                {
                    try
                    {
                        var photoUrl = await UploadPhotoAsync(photoDto.PhotoBase64, cancellationToken);
                        
                        var reviewPhoto = new ReviewPhoto
                        {
                            ReviewId = review.Id,
                            PhotoUrl = photoUrl,
                            Caption = photoDto.Caption,
                            OriginalFileName = photoDto.OriginalFileName,
                            DisplayOrder = photosUploaded,
                            UploadedAt = DateTime.UtcNow
                        };

                        _unitOfWork.ReviewPhotos.Create(reviewPhoto);
                        photosUploaded++;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error uploading review photo for review {ReviewId}", review.Id);
                    }
                }

                await _unitOfWork.CommitAsync();
            }

            _logger.LogInformation("Review created for {EntityType} {EntityId} by member {ReviewerId}. Verified: {IsVerified}, Photos: {PhotosCount}", 
                request.EntityType, request.EntityId, request.ReviewerId, isVerified, photosUploaded);

            return new SubmitReviewResult 
            { 
                Success = true, 
                Message = isVerified 
                    ? "Avaliação verificada enviada com sucesso!" 
                    : "Avaliação enviada com sucesso", 
                ReviewId = review.Id,
                IsVerified = isVerified,
                PhotosUploaded = photosUploaded
            };
        }

        private async Task<string> UploadPhotoAsync(string photoBase64, CancellationToken cancellationToken)
        {
            if (photoBase64.Contains(','))
                photoBase64 = photoBase64.Split(',')[1];

            var photoBytes = Convert.FromBase64String(photoBase64);
            using var photoStream = new MemoryStream(photoBytes);
            
            var fileName = $"reviews/{Guid.NewGuid()}.jpg";
            var photoUrl = await _s3Helper.UploadFileAsync(photoStream, fileName, "image/jpeg", cancellationToken);

            return photoUrl;
        }
    }
}
