namespace MyChurch.Application.Dtos
{
    public class EventDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public DateTime FinishDate { get; set; }
        public string Location { get; set; }
        public int ChurchId { get; set; }
        public int? DepartmentId { get; set; }
        public string? DepartmentName { get; set; }
        public ChurchDto Church { get; set; }
        public bool RequiresParticipantList { get; set; }
        public int EventType { get; set; } // Assuming EventType is an integer enum
        public ICollection<MemberDto> Participants { get; set; } = new List<MemberDto>();
        public ICollection<DiaconateScaleMemberDto> DiaconateScaleMembers { get; set; } = new List<DiaconateScaleMemberDto>();
        public ICollection<KidsScaleMemberDto> KidsScaleMembers { get; set; } = new List<KidsScaleMemberDto>();

        public EventRecurrenceDto? Recurrence { get; set; }
        public ICollection<EventNotificationDto> Notifications { get; set; } = new List<EventNotificationDto>();

        public static EventDto New(Domain.Entities.Event ev)
        {
            return new EventDto
            {
                Id = ev.Id,
                Title = ev.Title,
                Description = ev.Description,
                Date = ev.Date,
                FinishDate = ev.FinishDate,
                Location = ev.Location,
                ChurchId = ev.ChurchId,
                DepartmentId = ev.DepartmentId,
                DepartmentName = ev.Department?.Name,
                RequiresParticipantList = ev.RequiresParticipantList,
                Participants = ev.Participants?.Select(MemberDto.New).ToList() ?? new List<MemberDto>(),
                DiaconateScaleMembers = ev.DiaconateScaleMembers?
                    .OrderBy(x => x.Order)
                    .ThenBy(x => x.Member != null ? x.Member.Name : string.Empty)
                    .Select(DiaconateScaleMemberDto.New)
                    .ToList() ?? new List<DiaconateScaleMemberDto>(),
                KidsScaleMembers = ev.KidsScaleMembers?
                    .OrderBy(x => x.Order)
                    .ThenBy(x => x.Member != null ? x.Member.Name : string.Empty)
                    .Select(KidsScaleMemberDto.New)
                    .ToList() ?? new List<KidsScaleMemberDto>(),
                Recurrence = ev.Recurrence != null ? EventRecurrenceDto.New(ev.Recurrence) : null,
                EventType = (int)ev.EventType, // Assuming EventType is an enum
                Notifications = ev.Notifications?.Select(EventNotificationDto.New).ToList() ?? new List<EventNotificationDto>()
            };
        }
    }
}
