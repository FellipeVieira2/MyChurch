using MediatR;
using Microsoft.EntityFrameworkCore;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Exceptions;

namespace MyChurch.Application.WorshipService.Commands.ManageScale
{
    public class UpdateWorshipScaleMemberCommand : JwtMemberDto, IRequest<bool>
    {
        public int Id { get; set; }
        public int WorshipServiceId { get; set; }
        public int MemberId { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public int Order { get; set; }
        public string? Notes { get; set; }
    }

    public class UpdateWorshipScaleMemberCommandHandler : IRequestHandler<UpdateWorshipScaleMemberCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateWorshipScaleMemberCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(UpdateWorshipScaleMemberCommand request, CancellationToken cancellationToken)
        {
            var loggedMember = await _unitOfWork.Members.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == request.UserId, cancellationToken);

            if (loggedMember == null)
                throw new UnauthorizedAccessException("Usuário não encontrado.");

            var normalizedRoleName = request.RoleName?.Trim();

            if (string.IsNullOrWhiteSpace(normalizedRoleName))
                ValidationException.ThrowException("RoleName", "A função na escala é obrigatória.");

            var item = await _unitOfWork.WorshipScaleMembers.Query()
                .FirstOrDefaultAsync(x => x.Id == request.Id && x.WorshipServiceId == request.WorshipServiceId, cancellationToken);

            if (item == null)
                ValidationException.ThrowException("WorshipScale", "Item de escala não encontrado.");

            var worshipService = await _unitOfWork.WorshipServices.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(ws => ws.Id == request.WorshipServiceId && ws.ChurchId == loggedMember.ChurchId, cancellationToken);

            if (worshipService == null)
                ValidationException.ThrowException("WorshipService", "Culto não encontrado ou não pertence à sua igreja.");

            var targetMember = await _unitOfWork.Members.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == request.MemberId && m.ChurchId == loggedMember.ChurchId, cancellationToken);

            if (targetMember == null)
                ValidationException.ThrowException("Member", "Membro não encontrado para esta igreja.");

            var alreadyExists = await _unitOfWork.WorshipScaleMembers.Query()
                .AsNoTracking()
                .AnyAsync(x => x.Id != request.Id
                    && x.WorshipServiceId == request.WorshipServiceId
                    && x.MemberId == request.MemberId
                    && x.RoleName.ToLower() == normalizedRoleName.ToLower(), cancellationToken);

            if (alreadyExists)
                ValidationException.ThrowException("WorshipScale", "Este membro já está escalado com esta função neste culto.");

            item.MemberId = request.MemberId;
            item.RoleName = normalizedRoleName;
            item.Order = request.Order;
            item.Notes = string.IsNullOrWhiteSpace(request.Notes) ? null : request.Notes.Trim();

            _unitOfWork.WorshipScaleMembers.Update(item);
            await _unitOfWork.CommitAsync();

            return true;
        }
    }
}
