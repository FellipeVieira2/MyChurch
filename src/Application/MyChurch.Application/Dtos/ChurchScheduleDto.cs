namespace MyChurch.Application.Dtos
{
    public class ChurchScheduleDto
    {
        public int Id { get; set; }
        public int ChurchId { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public string DayOfWeekName { get; set; }
        public TimeSpan StartTime { get; set; }
        public string StartTimeFormatted { get; set; }
        public TimeSpan? EndTime { get; set; }
        public string? EndTimeFormatted { get; set; }
        public string ServiceType { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }

        public static ChurchScheduleDto New(Domain.Entities.ChurchSchedule schedule)
        {
            return new ChurchScheduleDto
            {
                Id = schedule.Id,
                ChurchId = schedule.ChurchId,
                DayOfWeek = schedule.DayOfWeek,
                DayOfWeekName = GetDayOfWeekNamePtBr(schedule.DayOfWeek),
                StartTime = schedule.StartTime,
                StartTimeFormatted = schedule.StartTime.ToString(@"hh\:mm"),
                EndTime = schedule.EndTime,
                EndTimeFormatted = schedule.EndTime?.ToString(@"hh\:mm"),
                ServiceType = schedule.ServiceType,
                Description = schedule.Description,
                IsActive = schedule.IsActive
            };
        }

        private static string GetDayOfWeekNamePtBr(DayOfWeek day)
        {
            return day switch
            {
                DayOfWeek.Sunday => "Domingo",
                DayOfWeek.Monday => "Segunda-feira",
                DayOfWeek.Tuesday => "Terça-feira",
                DayOfWeek.Wednesday => "Quarta-feira",
                DayOfWeek.Thursday => "Quinta-feira",
                DayOfWeek.Friday => "Sexta-feira",
                DayOfWeek.Saturday => "Sábado",
                _ => day.ToString()
            };
        }
    }
}
