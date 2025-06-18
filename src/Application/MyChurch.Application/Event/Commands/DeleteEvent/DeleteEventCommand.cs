using MediatR;
using Microsoft.EntityFrameworkCore;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Exceptions;
using System.Text.Json.Serialization;

namespace MyChurch.Application.Event.Commands.DeleteEvent
{
    public class DeleteEventCommand : JwtMemberDto, IRequest<Unit>
    {
        [JsonIgnore]
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

            // Busca o evento e inclui os cultos relacionados
            var ev = await _unitOfWork.Events.Query()
                .Include(e => e.WorshipServices)
                .FirstOrDefaultAsync(e => e.Id == request.Id && e.ChurchId == churchId, cancellationToken);

            if (ev is null)
                ValidationException.ThrowException("Event", "Event not found or does not belong to your church.");

            // Remove todos os WorshipServices relacionados ao evento
            if (ev.WorshipServices != null && ev.WorshipServices.Any())
            {
                foreach (var worshipService in ev.WorshipServices.ToList())
                {
                    _unitOfWork.WorshipServices.Delete(worshipService);
                }
            }

            _unitOfWork.Events.Delete(ev);
            await _unitOfWork.CommitAsync();

            return Unit.Value;
        }
    }
}