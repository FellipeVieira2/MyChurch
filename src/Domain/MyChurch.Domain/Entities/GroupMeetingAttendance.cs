using System;

namespace MyChurch.Domain.Entities
{
    public class GroupMeetingAttendance
    {
        public int Id { get; private set; }
        public int GroupMeetingId { get; private set; }
        public GroupMeeting GroupMeeting { get; private set; }
        public int MemberId { get; private set; }
        public bool IsPresent { get; private set; }

        protected GroupMeetingAttendance() { }

        public GroupMeetingAttendance(int groupMeetingId, int memberId, bool isPresent)
        {
            GroupMeetingId = groupMeetingId;
            MemberId = memberId;
            IsPresent = isPresent;
        }
    }
}
