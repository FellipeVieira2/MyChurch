using MyChurch.Domain.Entities;
using MyChurch.Domain.Enum;
using System.Collections.Generic;
using System.Linq;

namespace MyChurch.Application.Dtos
{
    public class WorshipServiceDto
    {
        public int Id { get; set; }
        public int ChurchId { get; set; }
        public int EventId { get; set; }
        public int? DepartmentId { get; set; }
        public string? DepartmentName { get; set; }
        public string Title { get; set; }
        public string? Theme { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string? Description { get; set; }
        public WorshipServiceStatus Status { get; set; }
        public List<WorshipActivityDto> Activities { get; set; } = new();
        public List<WorshipScheduleItemDto>? Schedule { get; set; }
        public List<WorshipScaleMemberDto>? ScaleMembers { get; set; }
        public int PresencesCount { get; set; }

        public static WorshipServiceDto New(Domain.Entities.WorshipService entity)
        {
            return new WorshipServiceDto
            {
                Id = entity.Id,
                ChurchId = entity.ChurchId,
                EventId = entity.EventId,
                DepartmentId = entity.DepartmentId,
                DepartmentName = entity.Department?.Name,
                Title = entity.Title,
                Theme = entity.Theme,
                StartTime = entity.StartTime,
                EndTime = entity.EndTime,
                Description = entity.Description,
                Status = entity.Status,
                Activities = entity.Activities?.Select(WorshipActivityDto.New).ToList() ?? new(),
                Schedule = entity.Schedule?.Select(WorshipScheduleItemDto.New).ToList() ?? new(),
                ScaleMembers = entity.ScaleMembers?
                    .OrderBy(x => x.Order)
                    .ThenBy(x => x.Member != null ? x.Member.Name : string.Empty)
                    .Select(WorshipScaleMemberDto.New)
                    .ToList() ?? new(),
                PresencesCount = entity.Presences?.Count ?? 0
            };
        }
    }
}
