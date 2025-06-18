using System;

namespace MyChurch.Domain.Entities
{
    public class GroupResource
    {
        public int Id { get; private set; }
        public int GroupId { get; private set; }
        public string Title { get; private set; }
        public string Description { get; private set; }
        public string FileUrl { get; private set; }
        public int UploadedByMemberId { get; private set; }
        public DateTime UploadedAt { get; private set; }

        private GroupResource() { }
        public GroupResource(int groupId, string title, string description, string fileUrl, int uploadedByMemberId)
        {
            GroupId = groupId;
            Title = title;
            Description = description;
            FileUrl = fileUrl;
            UploadedByMemberId = uploadedByMemberId;
            UploadedAt = DateTime.UtcNow;
        }
    }
}
