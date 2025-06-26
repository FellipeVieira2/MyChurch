using MediatR;
using Microsoft.EntityFrameworkCore;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;
using MyChurch.Domain.Exceptions;

namespace MyChurch.Application.Group.Commands
{
    public class RegisterMeetingAttendanceCommand : IRequest<bool>
    {
        public int MeetingId { get; set; }
        public List<AttendanceDto> Attendances { get; set; }
    }
    public class AttendanceDto { public int MemberId { get; set; } public bool IsPresent { get; set; } }

    public class RegisterMeetingAttendanceCommandHandler : IRequestHandler<RegisterMeetingAttendanceCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        public RegisterMeetingAttendanceCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<bool> Handle(RegisterMeetingAttendanceCommand request, CancellationToken cancellationToken)
        {
            var meeting = await _unitOfWork.GroupMeetings.Query().FirstOrDefaultAsync(x => x.Id == request.MeetingId, cancellationToken);
            if (meeting == null)
                ValidationException.ThrowException("Meeting", "Reunião não encontrada.");

            foreach (var att in request.Attendances)
            {
                var attendance = new GroupMeetingAttendance(request.MeetingId, att.MemberId, att.IsPresent);
                _unitOfWork.GroupMeetingAttendances.Create(attendance);
            }
            await _unitOfWork.CommitAsync();
            return true;
        }
    }
}
