using MediatR;
using Microsoft.EntityFrameworkCore;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Exceptions;

namespace MyChurch.Application.Asset.Commands.DeleteAsset
{
    public class DeleteAssetCommand : JwtMemberDto, IRequest<Unit>
    {
        public int Id { get; set; }
    }
    public class DeleteAssetCommandHandler : IRequestHandler<DeleteAssetCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteAssetCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(DeleteAssetCommand request, CancellationToken cancellationToken)
        {
            // Busca o membro logado para obter o ChurchId
            var loggedMember = await _unitOfWork.Members.Query()
                .FirstOrDefaultAsync(m => m.Id == request.UserId, cancellationToken);

            if (loggedMember is null)
                ValidationException.ThrowException("Member", "This Member does not exist.");

            int churchId = loggedMember.ChurchId;

            // Busca o ativo
            var asset = await _unitOfWork.Assets.Query()
                .FirstOrDefaultAsync(a => a.Id == request.Id && a.ChurchId == churchId, cancellationToken);

            if (asset is null)
                ValidationException.ThrowException("Asset", "Asset not found or does not belong to your church.");

            _unitOfWork.Assets.Delete(asset);
            await _unitOfWork.CommitAsync();

            return Unit.Value;
        }
    }
}
