using System;
using System.Collections.Generic;

namespace MyChurch.Domain.Entities
{
    public class GroupMeeting
    {
        public int Id { get; private set; }
        public int GroupId { get; private set; }
        public Group Group { get; private set; }
        public DateTime MeetingDate { get; private set; }
        public string Topic { get; private set; }
        public string? SummaryNotes { get; private set; }
        public ICollection<GroupMeetingAttendance> Attendances { get; private set; } = new List<GroupMeetingAttendance>();
        public ICollection<GroupMeetingMemberNote> MemberNotes { get; private set; } = new List<GroupMeetingMemberNote>();

        protected GroupMeeting() { }

        public GroupMeeting(int groupId, DateTime meetingDate, string topic, string? summaryNotes = null)
        {
            GroupId = groupId;
            MeetingDate = meetingDate;
            Topic = topic;
            SummaryNotes = summaryNotes;
        }

        public void UpdateSummary(string? summaryNotes)
        {
            SummaryNotes = summaryNotes;
        }
    }
}
