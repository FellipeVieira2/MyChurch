using System;
using MediatR;
using MyChurch.Domain.Entities;
using MyChurch.Domain.Contracts;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace MyChurch.Application.Group.Commands
{
    public class AddMeetingMemberNoteCommand : IRequest<int>
    {
        public int MeetingId { get; set; }
        public int MemberId { get; set; }
        public int LeaderId { get; set; }
        public string Note { get; set; }
    }

    public class AddMeetingMemberNoteCommandHandler : IRequestHandler<AddMeetingMemberNoteCommand, int>
    {
        private readonly IUnitOfWork _unitOfWork;
        public AddMeetingMemberNoteCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<int> Handle(AddMeetingMemberNoteCommand request, CancellationToken cancellationToken)
        {
            var meeting = await _unitOfWork.GroupMeetings.Query().FirstOrDefaultAsync(x => x.Id == request.MeetingId, cancellationToken);
            if (meeting == null)
                throw new Exception("Reunião não encontrada.");

            var note = new GroupMeetingMemberNote(request.MeetingId, request.MemberId, request.LeaderId, request.Note);
            _unitOfWork.GroupMeetingMemberNotes.Create(note);
            await _unitOfWork.CommitAsync();
            return note.Id;
        }
    }
}
