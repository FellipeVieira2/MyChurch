namespace MyChurch.Application.Dtos
{
    /// <summary>
    /// Estatísticas de membros por faixa etária
    /// </summary>
    public class MembersByAgeGroupDto
    {
        public string AgeGroup { get; set; } = null!;
        public int Count { get; set; }
        public decimal Percentage { get; set; }
    }
    
    /// <summary>
    /// Estatísticas de membros por gênero
    /// </summary>
    public class MembersByGenderDto
    {
        public string Gender { get; set; } = null!;
        public int Count { get; set; }
        public decimal Percentage { get; set; }
    }
    
    /// <summary>
    /// Estatísticas de membros por ministério
    /// </summary>
    public class MembersByMinistryDto
    {
        public string MinistryName { get; set; } = null!;
        public int MemberCount { get; set; }
        public decimal Percentage { get; set; }
    }
    
    /// <summary>
    /// Evolução mensal de membros
    /// </summary>
    public class MonthlyMemberGrowthDto
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public string MonthName { get; set; } = null!;
        public int NewMembers { get; set; }
        public int TotalMembers { get; set; }
        public int InactiveMembers { get; set; }
    }
    
    /// <summary>
    /// Evolução mensal financeira
    /// </summary>
    public class MonthlyFinancialDto
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public string MonthName { get; set; } = null!;
        public decimal Revenue { get; set; }
        public decimal Expenses { get; set; }
        public decimal Balance { get; set; }
    }
    
    /// <summary>
    /// Estatísticas de presença por evento
    /// </summary>
    public class AttendanceByEventDto
    {
        public int EventId { get; set; }
        public string EventName { get; set; } = null!;
        public DateTime EventDate { get; set; }
        public int TotalPresences { get; set; }
        public decimal AttendancePercentage { get; set; }
    }
    
    /// <summary>
    /// Top membros mais engajados
    /// </summary>
    public class TopEngagedMembersDto
    {
        public int MemberId { get; set; }
        public string MemberName { get; set; } = null!;
        public string? MemberPhoto { get; set; }
        public int EngagementScore { get; set; }
        public int AttendanceCount { get; set; }
        public int DonationCount { get; set; }
        public int PrayerRequestCount { get; set; }
    }
}
