using MediatR;
using Microsoft.EntityFrameworkCore;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Exceptions;

namespace MyChurch.Application.WorshipService.Commands.ManageScale
{
    public class RemoveWorshipScaleMemberCommand : JwtMemberDto, IRequest<bool>
    {
        public int Id { get; set; }
        public int WorshipServiceId { get; set; }
    }

    public class RemoveWorshipScaleMemberCommandHandler : IRequestHandler<RemoveWorshipScaleMemberCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public RemoveWorshipScaleMemberCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(RemoveWorshipScaleMemberCommand request, CancellationToken cancellationToken)
        {
            var loggedMember = await _unitOfWork.Members.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == request.UserId, cancellationToken);

            if (loggedMember == null)
                throw new UnauthorizedAccessException("Usuário não encontrado.");

            var worshipService = await _unitOfWork.WorshipServices.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(ws => ws.Id == request.WorshipServiceId && ws.ChurchId == loggedMember.ChurchId, cancellationToken);

            if (worshipService == null)
                ValidationException.ThrowException("WorshipService", "Culto não encontrado ou não pertence à sua igreja.");

            var item = await _unitOfWork.WorshipScaleMembers.Query()
                .FirstOrDefaultAsync(x => x.Id == request.Id && x.WorshipServiceId == request.WorshipServiceId, cancellationToken);

            if (item == null)
                ValidationException.ThrowException("WorshipScale", "Item de escala não encontrado.");

            _unitOfWork.WorshipScaleMembers.Delete(item);
            await _unitOfWork.CommitAsync();

            return true;
        }
    }
}
