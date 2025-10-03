using MediatR;
using Microsoft.EntityFrameworkCore;
using MyChurch.Domain.Contracts;

namespace MyChurch.Application.Reviews.Queries.GetChurchPhotoGallery
{
    /// <summary>
    /// Busca galeria de fotos de reviews de uma igreja
    /// </summary>
    public class GetChurchPhotoGalleryQuery : IRequest<ChurchPhotoGalleryResult>
    {
        public int ChurchId { get; set; }
        public int Limit { get; set; } = 50;
    }

    public class ChurchPhotoGalleryResult
    {
        public int ChurchId { get; set; }
        public int TotalPhotos { get; set; }
        public List<ChurchPhotoDto> Photos { get; set; } = new();
    }

    public class ChurchPhotoDto
    {
        public int PhotoId { get; set; }
        public int ReviewId { get; set; }
        public string PhotoUrl { get; set; } = string.Empty;
        public string? Caption { get; set; }
        public DateTime UploadedAt { get; set; }
        
        // Info do revisor
        public int ReviewerId { get; set; }
        public string ReviewerName { get; set; } = string.Empty;
        public string? ReviewerPhoto { get; set; }
        public int ReviewScore { get; set; }
        public bool IsVerified { get; set; }
    }

    public class GetChurchPhotoGalleryQueryHandler : IRequestHandler<GetChurchPhotoGalleryQuery, ChurchPhotoGalleryResult>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetChurchPhotoGalleryQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ChurchPhotoGalleryResult> Handle(GetChurchPhotoGalleryQuery request, CancellationToken cancellationToken)
        {
            var photos = await _unitOfWork.ReviewPhotos.Query()
                .Include(p => p.Review)
                    .ThenInclude(r => r.Reviewer)
                .Where(p => p.Review.EntityId == request.ChurchId && p.Review.EntityType == "Church")
                .OrderByDescending(p => p.UploadedAt)
                .Take(request.Limit)
                .Select(p => new ChurchPhotoDto
                {
                    PhotoId = p.Id,
                    ReviewId = p.ReviewId,
                    PhotoUrl = p.PhotoUrl,
                    Caption = p.Caption,
                    UploadedAt = p.UploadedAt,
                    ReviewerId = p.Review.ReviewerId,
                    ReviewerName = p.Review.Reviewer != null ? p.Review.Reviewer.Name : "Anônimo",
                    ReviewerPhoto = p.Review.Reviewer != null ? p.Review.Reviewer.Photo : null,
                    ReviewScore = p.Review.Score,
                    IsVerified = p.Review.IsVerified
                })
                .ToListAsync(cancellationToken);

            var totalPhotos = await _unitOfWork.ReviewPhotos.Query()
                .CountAsync(p => p.Review.EntityId == request.ChurchId && p.Review.EntityType == "Church", 
                    cancellationToken);

            return new ChurchPhotoGalleryResult
            {
                ChurchId = request.ChurchId,
                TotalPhotos = totalPhotos,
                Photos = photos
            };
        }
    }
}
