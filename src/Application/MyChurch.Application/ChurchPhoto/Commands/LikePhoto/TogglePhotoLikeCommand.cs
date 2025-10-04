using MediatR;
using Microsoft.EntityFrameworkCore;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Exceptions;

namespace MyChurch.Application.ChurchPhoto.Commands.LikePhoto
{
    /// <summary>
    /// Comando para curtir/descurtir foto
    /// </summary>
    public class TogglePhotoLikeCommand : JwtMemberDto, IRequest<PhotoLikeResultDto>
    {
        public int PhotoId { get; set; }
        
        // Para visitantes (não autenticados)
        public int? VisitorId { get; set; }
    }
    
    public class PhotoLikeResultDto
    {
        public bool IsLiked { get; set; }
        public int TotalLikes { get; set; }
    }
    
    public class TogglePhotoLikeCommandHandler : IRequestHandler<TogglePhotoLikeCommand, PhotoLikeResultDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public TogglePhotoLikeCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PhotoLikeResultDto> Handle(TogglePhotoLikeCommand request, CancellationToken cancellationToken)
        {
            // Buscar foto
            var photo = await _unitOfWork.ChurchPhotos.GetPhotoWithDetailsAsync(request.PhotoId, cancellationToken);
            
            if (photo == null)
                ValidationException.ThrowException("Photo", "Foto não encontrada.");

            // Verificar se usuário já curtiu
            var memberId = request.UserId > 0 ? request.UserId : (int?)null;
            var visitorId = request.VisitorId;

            var existingLike = await _unitOfWork.ChurchPhotoLikes.GetUserLikeAsync(
                request.PhotoId, 
                memberId, 
                visitorId, 
                cancellationToken);

            bool isLiked;

            if (existingLike != null)
            {
                // Descurtir
                _unitOfWork.ChurchPhotoLikes.Delete(existingLike);
                photo.DecrementLikes();
                isLiked = false;
            }
            else
            {
                // Curtir
                var like = new Domain.Entities.ChurchPhotoLike
                {
                    ChurchPhotoId = request.PhotoId,
                    MemberId = memberId,
                    VisitorId = visitorId,
                    LikedAt = DateTime.UtcNow
                };
                
                _unitOfWork.ChurchPhotoLikes.Create(like);
                photo.IncrementLikes();
                isLiked = true;
            }

            _unitOfWork.ChurchPhotos.Update(photo);
            await _unitOfWork.CommitAsync();

            return new PhotoLikeResultDto
            {
                IsLiked = isLiked,
                TotalLikes = photo.Likes
            };
        }
    }
}
