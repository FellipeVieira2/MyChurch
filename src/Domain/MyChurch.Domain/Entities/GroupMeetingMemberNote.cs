using System;

namespace MyChurch.Domain.Entities
{
    public class GroupMeetingMemberNote
    {
        public int Id { get; private set; }
        public int GroupMeetingId { get; private set; }
        public GroupMeeting GroupMeeting { get; private set; }
        public int MemberId { get; private set; }
        public int LeaderId { get; private set; }
        public string Note { get; private set; }
        public DateTime CreatedAt { get; private set; }

        protected GroupMeetingMemberNote() { }

        public GroupMeetingMemberNote(int groupMeetingId, int memberId, int leaderId, string note)
        {
            GroupMeetingId = groupMeetingId;
            MemberId = memberId;
            LeaderId = leaderId;
            Note = note;
            CreatedAt = DateTime.UtcNow;
        }
    }
}
