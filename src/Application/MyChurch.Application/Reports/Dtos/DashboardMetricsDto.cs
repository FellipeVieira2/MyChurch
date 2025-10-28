namespace MyChurch.Application.Reports.Dtos
{
    /// <summary>
    /// Métricas gerais para o dashboard da igreja
    /// </summary>
    public class DashboardMetricsDto
    {
        // ?? MEMBROS
        public int TotalMembers { get; set; }
        public int ActiveMembers { get; set; }
        public int InactiveMembers { get; set; }
        public int NewMembersThisMonth { get; set; }
        public int NewMembersThisYear { get; set; }
        public decimal MemberGrowthPercentage { get; set; } // Crescimento vs mês anterior

        // ?? FINANÇAS
        public decimal TotalDonationsThisMonth { get; set; }
        public decimal TotalDonationsThisYear { get; set; }
        public decimal AverageDonationAmount { get; set; }
        public int TotalDonorsThisMonth { get; set; }
        public decimal DonationGrowthPercentage { get; set; } // Crescimento vs mês anterior

        // ?? EVENTOS
        public int TotalEventsThisMonth { get; set; }
        public int UpcomingEvents { get; set; }
        public decimal AverageEventAttendance { get; set; }

        // ?? CULTOS
        public int TotalWorshipServicesThisMonth { get; set; }
        public decimal AverageWorshipAttendance { get; set; }

        // ?? VISITANTES
        public int TotalVisitors { get; set; }
        public int NewVisitorsThisMonth { get; set; }
        public int VisitorsConverted { get; set; } // Visitantes que se tornaram membros
        public decimal VisitorConversionRate { get; set; }

        // ?? ENGAJAMENTO
        public int ActiveBibleReadingMembers { get; set; }
        public int TotalPrayerRequests { get; set; }
        public int ActiveGroupMembers { get; set; }

        // ?? TOP PERFORMERS
        public List<TopDonorDto> TopDonors { get; set; } = new();
        public List<MostEngagedMemberDto> MostEngagedMembers { get; set; } = new();
    }

    public class TopDonorDto
    {
        public int MemberId { get; set; }
        public string MemberName { get; set; }
        public decimal TotalDonated { get; set; }
    }

    public class MostEngagedMemberDto
    {
        public int MemberId { get; set; }
        public string MemberName { get; set; }
        public int EngagementScore { get; set; }
    }
}
