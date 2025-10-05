using MediatR;
using MyChurch.Application.Dtos;

namespace MyChurch.Application.Ministries.Commands.UpdateMinistry
{
    public class UpdateMinistryCommand : IRequest<MinistryDto>
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int? LeaderId { get; set; }
        public string? Photo { get; set; }
        public string? Color { get; set; }
        public string? MeetingDay { get; set; }
        public TimeSpan? MeetingTime { get; set; }
        public string? MeetingLocation { get; set; }
        public bool? IsActive { get; set; }
    }
}
