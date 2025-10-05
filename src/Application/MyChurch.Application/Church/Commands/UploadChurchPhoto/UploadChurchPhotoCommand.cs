using MediatR;
using Microsoft.Extensions.Logging;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;
using MyChurch.Domain.Exceptions;
using MyChurch.Infrastructure.Utils.S3;

namespace MyChurch.Application.Church.Commands.UploadChurchPhoto
{
    /// <summary>
    /// Comando para fazer upload de uma foto da igreja
    /// </summary>
    public class UploadChurchPhotoCommand : JwtMemberDto, IRequest<ChurchPhotoDto>
    {
        /// <summary>ID da igreja</summary>
        public int ChurchId { get; set; }
        
        /// <summary>Foto em Base64</summary>
        /// <example>data:image/jpeg;base64,/9j/4AAQSkZJRg...</example>
        public string PhotoBase64 { get; set; }
        
        /// <summary>Legenda/descrição</summary>
        /// <example>Interior do templo durante culto</example>
        public string? Caption { get; set; }
        
        /// <summary>Categoria da foto (1-10)</summary>
        /// <example>2</example>
        public PhotoCategory Category { get; set; }
        
        /// <summary>Nome original do arquivo</summary>
        public string? OriginalFileName { get; set; }
    }

    public class UploadChurchPhotoCommandHandler : IRequestHandler<UploadChurchPhotoCommand, ChurchPhotoDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IS3Helper _s3Helper;
        private readonly ILogger<UploadChurchPhotoCommandHandler> _logger;

        public UploadChurchPhotoCommandHandler(
            IUnitOfWork unitOfWork,
            IS3Helper s3Helper,
            ILogger<UploadChurchPhotoCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _s3Helper = s3Helper;
            _logger = logger;
        }

        public async Task<ChurchPhotoDto> Handle(UploadChurchPhotoCommand request, CancellationToken cancellationToken)
        {
            // Verifica se o usuário pertence à igreja
            var member = _unitOfWork.Members.Query()
                .FirstOrDefault(m => m.Id == request.UserId && m.ChurchId == request.ChurchId);

            if (member == null)
            {
                _logger.LogWarning("Membro {UserId} tentou enviar foto para igreja {ChurchId} sem permissão", 
                    request.UserId, request.ChurchId);
                ValidationException.ThrowException("UploadPhoto", "Você não pertence a esta igreja.");
            }

            // Verifica se a igreja existe
            var church = _unitOfWork.Churchs.Query()
                .FirstOrDefault(c => c.Id == request.ChurchId);

            if (church == null)
            {
                ValidationException.ThrowException("UploadPhoto", "Igreja não encontrada.");
            }

            // Upload da foto para S3
            var photoUrl = await UploadPhotoToS3(request.PhotoBase64, request.ChurchId, cancellationToken);

            // Cria a entidade
            var photo = new Domain.Entities.ChurchPhoto
            {
                ChurchId = request.ChurchId,
                PhotoUrl = photoUrl,
                Caption = request.Caption,
                Category = request.Category,
                OriginalFileName = request.OriginalFileName,
                UploadedByMemberId = request.UserId,
                UploadedAt = DateTime.UtcNow,
                // Fotos de membros são automaticamente aprovadas
                // Fotos de visitantes precisam de aprovação
                IsApproved = true,
                IsRejected = false
            };

            _unitOfWork.ChurchPhotos.Create(photo);
            await _unitOfWork.CommitAsync();

            _logger.LogInformation("Foto {PhotoId} enviada para igreja {ChurchId} por membro {MemberId}", 
                photo.Id, request.ChurchId, request.UserId);

            return ChurchPhotoDto.New(photo);
        }

        private async Task<string> UploadPhotoToS3(string photoBase64, int churchId, CancellationToken cancellationToken)
        {
            if (photoBase64.Contains(','))
                photoBase64 = photoBase64.Split(',')[1];

            var photoBytes = Convert.FromBase64String(photoBase64);
            using var photoStream = new MemoryStream(photoBytes);
            
            var fileName = $"churches/{churchId}/photos/{Guid.NewGuid()}.jpg";
            var photoUrl = await _s3Helper.UploadFileAsync(photoStream, fileName, "image/jpeg", cancellationToken);

            return photoUrl;
        }
    }
}
