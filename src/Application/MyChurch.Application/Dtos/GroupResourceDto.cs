using System;

namespace MyChurch.Application.Dtos
{
    public class GroupResourceDto
    {
        public int Id { get; set; }
        public int GroupId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string FileUrl { get; set; }
        public int UploadedByMemberId { get; set; }
        public DateTime UploadedAt { get; set; }
    }
}
