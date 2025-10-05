using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;
using MyChurch.Domain.Exceptions;

namespace MyChurch.Application.Church.Commands.LikeChurchPhoto
{
    /// <summary>
    /// Comando para curtir/descurtir uma foto
    /// </summary>
    public class LikeChurchPhotoCommand : JwtMemberDto, IRequest<Unit>
    {
        public int PhotoId { get; set; }
        public bool Like { get; set; } = true; // true = curtir, false = descurtir
    }

    public class LikeChurchPhotoCommandHandler : IRequestHandler<LikeChurchPhotoCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<LikeChurchPhotoCommandHandler> _logger;

        public LikeChurchPhotoCommandHandler(
            IUnitOfWork unitOfWork,
            ILogger<LikeChurchPhotoCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Unit> Handle(LikeChurchPhotoCommand request, CancellationToken cancellationToken)
        {
            var photo = await _unitOfWork.ChurchPhotos.Query()
                .FirstOrDefaultAsync(p => p.Id == request.PhotoId, cancellationToken);

            if (photo == null)
            {
                ValidationException.ThrowException("LikePhoto", "Foto não encontrada.");
            }

            var existingLike = await _unitOfWork.ChurchPhotoLikes.Query()
                .FirstOrDefaultAsync(l => l.ChurchPhotoId == request.PhotoId && 
                                         l.MemberId == request.UserId, 
                                    cancellationToken);

            if (request.Like)
            {
                // Curtir
                if (existingLike == null)
                {
                    var like = new ChurchPhotoLike
                    {
                        ChurchPhotoId = request.PhotoId,
                        MemberId = request.UserId,
                        LikedAt = DateTime.UtcNow
                    };

                    _unitOfWork.ChurchPhotoLikes.Create(like);
                    photo.IncrementLikes();
                    _unitOfWork.ChurchPhotos.Update(photo);
                    
                    await _unitOfWork.CommitAsync();
                    
                    _logger.LogInformation("Membro {MemberId} curtiu foto {PhotoId}", request.UserId, request.PhotoId);
                }
            }
            else
            {
                // Descurtir
                if (existingLike != null)
                {
                    _unitOfWork.ChurchPhotoLikes.Delete(existingLike);
                    photo.DecrementLikes();
                    _unitOfWork.ChurchPhotos.Update(photo);
                    
                    await _unitOfWork.CommitAsync();
                    
                    _logger.LogInformation("Membro {MemberId} descurtiu foto {PhotoId}", request.UserId, request.PhotoId);
                }
            }

            return Unit.Value;
        }
    }
}
