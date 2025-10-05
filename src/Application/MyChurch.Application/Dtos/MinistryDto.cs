using MyChurch.Domain.Entities;

namespace MyChurch.Application.Dtos
{
    public class MinistryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public int? LeaderId { get; set; }
        public string? LeaderName { get; set; }
        public int ChurchId { get; set; }
        public string? Photo { get; set; }
        public string? Color { get; set; }
        public string? MeetingDay { get; set; }
        public TimeSpan? MeetingTime { get; set; }
        public string? MeetingLocation { get; set; }
        public bool IsActive { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
        public int MemberCount { get; set; }
        
        public static MinistryDto FromEntity(Domain.Entities.Ministry ministry)
        {
            return new MinistryDto
            {
                Id = ministry.Id,
                Name = ministry.Name,
                Description = ministry.Description,
                LeaderId = ministry.LeaderId,
                LeaderName = ministry.Leader?.Name,
                ChurchId = ministry.ChurchId,
                Photo = ministry.Photo,
                Color = ministry.Color,
                MeetingDay = ministry.MeetingDay,
                MeetingTime = ministry.MeetingTime,
                MeetingLocation = ministry.MeetingLocation,
                IsActive = ministry.IsActive,
                Created = ministry.Created,
                Updated = ministry.Updated,
                MemberCount = ministry.MinistryMembers?.Count(m => m.IsActive) ?? 0
            };
        }
    }
}
