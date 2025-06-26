using System;
using System.Collections.Generic;
using MediatR;
using MyChurch.Domain.Entities;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace MyChurch.Application.Group.Queries
{
    public record GetGroupMeetingsQuery(int GroupId) : IRequest<List<GroupMeeting>>;
    public class GetGroupMeetingsQueryHandler : IRequestHandler<GetGroupMeetingsQuery, List<GroupMeeting>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public GetGroupMeetingsQueryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }
        public async Task<List<GroupMeeting>> Handle(GetGroupMeetingsQuery request, CancellationToken cancellationToken)
        {
            var meetings = _unitOfWork.GroupMeetings.Query().Where(x => x.GroupId == request.GroupId).ToList();
            return meetings;
        }
    }

    public record GetGroupMeetingDetailsQuery(int MeetingId) : IRequest<GroupMeeting>;
    public class GetGroupMeetingDetailsQueryHandler : IRequestHandler<GetGroupMeetingDetailsQuery, GroupMeeting>
    {
        private readonly IUnitOfWork _unitOfWork;
        public GetGroupMeetingDetailsQueryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }
        public async Task<GroupMeeting> Handle(GetGroupMeetingDetailsQuery request, CancellationToken cancellationToken)
        {
            var meeting = await _unitOfWork.GroupMeetings.Query().FirstOrDefaultAsync(x => x.Id == request.MeetingId, cancellationToken);
            return meeting;
        }
    }
}
