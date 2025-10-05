using MediatR;
using MyChurch.Application.Dtos;

namespace MyChurch.Application.Ministries.Commands.CreateMinistry
{
    public class CreateMinistryCommand : IRequest<MinistryDto>
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public int? LeaderId { get; set; }
        public int ChurchId { get; set; }
        public string? Photo { get; set; }
        public string? Color { get; set; }
        public string? MeetingDay { get; set; }
        public TimeSpan? MeetingTime { get; set; }
        public string? MeetingLocation { get; set; }
    }
}
