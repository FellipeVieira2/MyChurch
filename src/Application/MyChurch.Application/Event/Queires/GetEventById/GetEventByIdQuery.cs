using System.Text.Json.Serialization;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Enum;
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
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == request.UserId, cancellationToken);

            if (loggedMember == null)
                ValidationException.ThrowException("Member", "This Member does not exist.");

            int churchId = loggedMember.ChurchId;

            var evQuery = _unitOfWork.Events.Query()
                .AsNoTracking()
                .Include(e => e.Department)
                .Include(e => e.Church)
                .Include(e => e.Participants)
                .Include(e => e.DiaconateScaleMembers)
                    .ThenInclude(x => x.Member)
                .Include(e => e.KidsScaleMembers)
                    .ThenInclude(x => x.Member)
                .Include(e => e.Notifications)
                .Include(e => e.Recurrence)
                .Where(e => e.Id == request.Id && e.ChurchId == churchId);

            if (loggedMember.Role != UserRole.Admin)
            {
                var allowedDepartmentIds = await _unitOfWork.DepartmentMembers.Query()
                    .AsNoTracking()
                    .Where(dm => dm.MemberId == loggedMember.Id && dm.IsActive)
                    .Select(dm => dm.DepartmentId)
                    .Distinct()
                    .ToListAsync(cancellationToken);

                evQuery = evQuery.Where(e => e.DepartmentId == null || (e.DepartmentId.HasValue && allowedDepartmentIds.Contains(e.DepartmentId.Value)));
            }

            var ev = await evQuery.FirstOrDefaultAsync(cancellationToken);

            if (ev == null)
                ValidationException.ThrowException("Event", "Event not found or does not belong to your church.");

            return EventDto.New(ev);
        }
    }
}