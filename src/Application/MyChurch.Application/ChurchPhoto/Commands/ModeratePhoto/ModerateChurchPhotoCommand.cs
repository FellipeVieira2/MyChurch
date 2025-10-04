using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Enum;
using MyChurch.Domain.Exceptions;

namespace MyChurch.Application.ChurchPhoto.Commands.ModeratePhoto
{
    /// <summary>
    /// Comando para aprovar foto (apenas Admin)
    /// </summary>
    public class ApproveChurchPhotoCommand : JwtMemberDto, IRequest<ChurchPhotoDto>
    {
        public int PhotoId { get; set; }
        public bool SetAsFeatured { get; set; } = false;
        public int DisplayOrder { get; set; } = 0;
    }
    
    public class ApproveChurchPhotoCommandHandler : IRequestHandler<ApproveChurchPhotoCommand, ChurchPhotoDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ApproveChurchPhotoCommandHandler> _logger;

        public ApproveChurchPhotoCommandHandler(
            IUnitOfWork unitOfWork,
            ILogger<ApproveChurchPhotoCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<ChurchPhotoDto> Handle(ApproveChurchPhotoCommand request, CancellationToken cancellationToken)
        {
            // Verificar se é admin
            var admin = await _unitOfWork.Members.Query()
                .FirstOrDefaultAsync(m => m.Id == request.UserId, cancellationToken);
            
            if (admin == null || admin.Role != UserRole.Admin)
            {
                ValidationException.ThrowException("Authorization", "Apenas administradores podem aprovar fotos.");
            }

            // Buscar foto
            var photo = await _unitOfWork.ChurchPhotos.GetPhotoWithDetailsAsync(request.PhotoId, cancellationToken);
            
            if (photo == null)
                ValidationException.ThrowException("Photo", "Foto não encontrada.");

            // Verificar se admin pertence à mesma igreja
            if (photo.ChurchId != admin.ChurchId)
            {
                ValidationException.ThrowException("Authorization", "Você só pode aprovar fotos da sua igreja.");
            }

            // Aprovar
            photo.Approve(request.UserId);
            
            // Marcar como destaque se solicitado
            if (request.SetAsFeatured)
            {
                photo.SetAsFeatured(request.DisplayOrder);
            }

            _unitOfWork.ChurchPhotos.Update(photo);
            await _unitOfWork.CommitAsync();

            _logger.LogInformation(
                "Photo {PhotoId} approved by admin {AdminId} for church {ChurchId}", 
                request.PhotoId, 
                request.UserId, 
                photo.ChurchId);

            return ChurchPhotoDto.New(photo);
        }
    }
    
    /// <summary>
    /// Comando para rejeitar foto (apenas Admin)
    /// </summary>
    public class RejectChurchPhotoCommand : JwtMemberDto, IRequest<Unit>
    {
        public int PhotoId { get; set; }
        public string Reason { get; set; } = "Conteúdo inadequado";
    }
    
    public class RejectChurchPhotoCommandHandler : IRequestHandler<RejectChurchPhotoCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<RejectChurchPhotoCommandHandler> _logger;

        public RejectChurchPhotoCommandHandler(
            IUnitOfWork unitOfWork,
            ILogger<RejectChurchPhotoCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Unit> Handle(RejectChurchPhotoCommand request, CancellationToken cancellationToken)
        {
            // Verificar se é admin
            var admin = await _unitOfWork.Members.Query()
                .FirstOrDefaultAsync(m => m.Id == request.UserId, cancellationToken);
            
            if (admin == null || admin.Role != UserRole.Admin)
            {
                ValidationException.ThrowException("Authorization", "Apenas administradores podem rejeitar fotos.");
            }

            // Buscar foto
            var photo = await _unitOfWork.ChurchPhotos.GetPhotoWithDetailsAsync(request.PhotoId, cancellationToken);
            
            if (photo == null)
                ValidationException.ThrowException("Photo", "Foto não encontrada.");

            // Verificar se admin pertence à mesma igreja
            if (photo.ChurchId != admin.ChurchId)
            {
                ValidationException.ThrowException("Authorization", "Você só pode rejeitar fotos da sua igreja.");
            }

            // Rejeitar
            photo.Reject(request.UserId, request.Reason);

            _unitOfWork.ChurchPhotos.Update(photo);
            await _unitOfWork.CommitAsync();

            _logger.LogWarning(
                "Photo {PhotoId} rejected by admin {AdminId} for church {ChurchId}. Reason: {Reason}", 
                request.PhotoId, 
                request.UserId, 
                photo.ChurchId,
                request.Reason);

            return Unit.Value;
        }
    }
}
