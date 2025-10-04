using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;
using MyChurch.Domain.Exceptions;
using MyChurch.Infrastructure.Utils.S3;

namespace MyChurch.Application.ChurchPhoto.Commands.UploadPhoto
{
    /// <summary>
    /// Comando para upload de foto da igreja
    /// </summary>
    public class UploadChurchPhotoCommand : JwtMemberDto, IRequest<ChurchPhotoDto>
    {
        public int ChurchId { get; set; }
        public string PhotoBase64 { get; set; } = string.Empty;
        public string? Caption { get; set; }
        public string Category { get; set; } = "Other";
        
        // Para visitantes (não autenticados)
        public int? VisitorId { get; set; }
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
            // Validar igreja existe
            var church = await _unitOfWork.Churchs.Query()
                .FirstOrDefaultAsync(c => c.Id == request.ChurchId, cancellationToken);
            
            if (church == null)
                ValidationException.ThrowException("Church", "Igreja não encontrada.");

            // Validar categoria
            if (!Enum.TryParse<PhotoCategory>(request.Category, out var category))
            {
                ValidationException.ThrowException("Category", "Categoria inválida.");
            }

            // Upload da foto para S3
            var photoUrl = await UploadPhotoToS3Async(request.PhotoBase64, cancellationToken);

            // Criar entidade
            var photo = new Domain.Entities.ChurchPhoto
            {
                ChurchId = request.ChurchId,
                UploadedByMemberId = request.UserId > 0 ? request.UserId : null,
                UploadedByVisitorId = request.VisitorId,
                PhotoUrl = photoUrl,
                Caption = request.Caption,
                Category = category,
                UploadedAt = DateTime.UtcNow,
                IsApproved = false, // Precisa aprovação
                IsRejected = false
            };

            _unitOfWork.ChurchPhotos.Create(photo);
            await _unitOfWork.CommitAsync();

            _logger.LogInformation(
                "Photo uploaded for church {ChurchId} by {UserType} {UserId}", 
                request.ChurchId,
                request.UserId > 0 ? "Member" : "Visitor",
                request.UserId > 0 ? request.UserId : request.VisitorId);

            // Retornar DTO
            photo.Church = church;
            return ChurchPhotoDto.New(photo);
        }

        private async Task<string> UploadPhotoToS3Async(string photoBase64, CancellationToken cancellationToken)
        {
            try
            {
                // Remover prefixo data:image se existir
                if (photoBase64.Contains(','))
                    photoBase64 = photoBase64.Split(',')[1];

                var photoBytes = Convert.FromBase64String(photoBase64);
                using var photoStream = new MemoryStream(photoBytes);

                var fileName = $"church-photos/{Guid.NewGuid()}.jpg";
                var photoUrl = await _s3Helper.UploadFileAsync(photoStream, fileName, "image/jpeg", cancellationToken);

                return photoUrl;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading photo to S3");
                throw new Exception("Erro ao fazer upload da foto. Tente novamente.", ex);
            }
        }
    }
}
