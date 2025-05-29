using MediatR;
using Microsoft.EntityFrameworkCore;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Exceptions;

namespace MyChurch.Application.Event.Commands.DeleteEvent
{
    public class DeleteEventCommand : JwtMemberDto, IRequest<Unit>
    {
        public int Id { get; set; }
    }

    public class DeleteEventCommandHandler : IRequestHandler<DeleteEventCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteEventCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(DeleteEventCommand request, CancellationToken cancellationToken)
        {
            // Busca o membro logado para obter o ChurchId
            var loggedMember = await _unitOfWork.Members.Query()
                .FirstOrDefaultAsync(m => m.Id == request.UserId, cancellationToken);

            if (loggedMember is null)
                ValidationException.ThrowException("Member", "This Member does not exist.");

            int churchId = loggedMember.ChurchId;

            // Busca o evento
            var ev = await _unitOfWork.Events.Query()
                .FirstOrDefaultAsync(e => e.Id == request.Id && e.ChurchId == churchId, cancellationToken);

            if (ev is null)
                ValidationException.ThrowException("Event", "Event not found or does not belong to your church.");

            _unitOfWork.Events.Delete(ev);
            await _unitOfWork.CommitAsync();

            return Unit.Value;
        }
    }
}
