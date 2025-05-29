using System.Text.Json.Serialization;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Exceptions;

namespace MyChurch.Application.Event.Queries.GetEventById
{
    public class GetEventByIdQuery : JwtMemberDto, IRequest<EventDto>
    {
        [JsonIgnore]
        public int Id { get; set; }
    }

    public class GetEventByIdQueryHandler : IRequestHandler<GetEventByIdQuery, EventDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetEventByIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<EventDto> Handle(GetEventByIdQuery request, CancellationToken cancellationToken)
        {
            var loggedMember = await _unitOfWork.Members.Query()
                .FirstOrDefaultAsync(m => m.Id == request.UserId, cancellationToken);

            if (loggedMember == null)
                ValidationException.ThrowException("Member", "This Member does not exist.");

            int churchId = loggedMember.ChurchId;

            var ev = await _unitOfWork.Events.Query()
                .Include(e => e.Church)
                .Include(e => e.Participants)
                .Include(e => e.Notifications)
                .Include(e => e.Recurrence)
                .FirstOrDefaultAsync(e => e.Id == request.Id && e.ChurchId == churchId, cancellationToken);

            if (ev == null)
                ValidationException.ThrowException("Event", "Event not found or does not belong to your church.");

            return EventDto.New(ev);
        }
    }
}