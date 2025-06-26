using MediatR;
using MyChurch.Domain.Entities;
using MyChurch.Domain.Contracts;
using Microsoft.EntityFrameworkCore;
using MyChurch.Domain.Exceptions;

namespace MyChurch.Application.Group.Commands
{
    public class CreateGroupMeetingCommand : IRequest<int>
    {
        public int GroupId { get; set; }
        public DateTime MeetingDate { get; set; }
        public string Topic { get; set; }
        public string? SummaryNotes { get; set; }
    }

    public class CreateGroupMeetingCommandHandler : IRequestHandler<CreateGroupMeetingCommand, int>
    {
        private readonly IUnitOfWork _unitOfWork;
        public CreateGroupMeetingCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<int> Handle(CreateGroupMeetingCommand request, CancellationToken cancellationToken)
        {
            var group = await _unitOfWork.Groups.Query().FirstOrDefaultAsync(x => x.Id == request.GroupId, cancellationToken);
            if (group == null)
                ValidationException.ThrowException("Group", "This Group does not exist.");
            var meeting = new GroupMeeting(request.GroupId, request.MeetingDate, request.Topic, request.SummaryNotes);
            _unitOfWork.GroupMeetings.Create(meeting);
            await _unitOfWork.CommitAsync();
            return meeting.Id;
        }
    }
}
