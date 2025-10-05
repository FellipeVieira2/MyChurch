using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Exceptions;

namespace MyChurch.Application.Church.Commands.DeleteChurchPhoto
{
    /// <summary>
    /// Comando para deletar uma foto
    /// </summary>
    public class DeleteChurchPhotoCommand : JwtMemberDto, IRequest<Unit>
    {
        public int PhotoId { get; set; }
        public int ChurchId { get; set; }
    }

    public class DeleteChurchPhotoCommandHandler : IRequestHandler<DeleteChurchPhotoCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DeleteChurchPhotoCommandHandler> _logger;

        public DeleteChurchPhotoCommandHandler(
            IUnitOfWork unitOfWork,
            ILogger<DeleteChurchPhotoCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Unit> Handle(DeleteChurchPhotoCommand request, CancellationToken cancellationToken)
        {
            // Verifica permissão
            var member = _unitOfWork.Members.Query()
                .FirstOrDefault(m => m.Id == request.UserId && m.ChurchId == request.ChurchId);

            if (member == null)
            {
                ValidationException.ThrowException("DeletePhoto", "Você não tem permissão.");
            }

            var photo = await _unitOfWork.ChurchPhotos.Query()
                .FirstOrDefaultAsync(p => p.Id == request.PhotoId && p.ChurchId == request.ChurchId, 
                                   cancellationToken);

            if (photo == null)
            {
                ValidationException.ThrowException("DeletePhoto", "Foto não encontrada.");
            }

            // Apenas admin ou quem enviou pode deletar
            var isAdmin = member.Role == Domain.Enum.UserRole.Admin;
            var isOwner = photo.UploadedByMemberId == request.UserId;

            if (!isAdmin && !isOwner)
            {
                ValidationException.ThrowException("DeletePhoto", "Apenas administradores ou o autor podem deletar.");
            }

            _unitOfWork.ChurchPhotos.Delete(photo);
            await _unitOfWork.CommitAsync();

            _logger.LogInformation("Foto {PhotoId} deletada por membro {MemberId}", request.PhotoId, request.UserId);

            return Unit.Value;
        }
    }
}
