namespace MyChurch.Application.Reports.Dtos
{
    /// <summary>
    /// Relatório de engajamento dos membros
    /// </summary>
    public class EngagementReportDto
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        // ?? VISÃO GERAL
        public int TotalMembers { get; set; }
        public int HighlyEngagedMembers { get; set; } // Score > 80
        public int ModeratelyEngagedMembers { get; set; } // Score 40-80
        public int LowEngagementMembers { get; set; } // Score < 40
        public decimal AverageEngagementScore { get; set; }

        // ?? LEITURA BÍBLICA
        public int MembersInBibleReadingPlans { get; set; }
        public decimal AverageBibleReadingCompletion { get; set; }
        public int CompletedPlans { get; set; }

        // ?? ORAÇÃO
        public int TotalPrayerRequests { get; set; }
        public int MembersWithPrayerRequests { get; set; }

        // ?? GRUPOS
        public int TotalGroups { get; set; }
        public int MembersInGroups { get; set; }
        public decimal AverageGroupAttendance { get; set; }

        // ?? CULTOS
        public int TotalWorshipServices { get; set; }
        public decimal AverageWorshipAttendance { get; set; }
        public int MembersWithPerfectAttendance { get; set; }

        // ?? DOAÇÕES
        public int RegularDonors { get; set; } // Doou nos últimos 3 meses
        public decimal DonorParticipationRate { get; set; }

        // ?? MEMBROS POR NÍVEL DE FÉ
        public List<MembersByFaithLevelDto> MembersByFaithLevel { get; set; } = new();

        // ?? LISTA DE MEMBROS
        public List<MemberEngagementDetailDto> MembersEngagement { get; set; } = new();

        // ?? ALERTAS PASTORAIS
        public List<PastoralAlertDto> PastoralAlerts { get; set; } = new();
    }

    public class MembersByFaithLevelDto
    {
        public string LevelName { get; set; }
        public int MemberCount { get; set; }
        public decimal Percentage { get; set; }
    }

    public class MemberEngagementDetailDto
    {
        public int MemberId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public int EngagementScore { get; set; }
        public string FaithLevel { get; set; }
        public int WorshipAttendances { get; set; }
        public int DonationsCount { get; set; }
        public bool IsInBiblePlan { get; set; }
        public bool IsInGroup { get; set; }
        public int DaysSinceLastActivity { get; set; }
        public string EngagementLevel { get; set; } // "High", "Moderate", "Low"
    }

    public class PastoralAlertDto
    {
        public int MemberId { get; set; }
        public string MemberName { get; set; }
        public string AlertType { get; set; }
        public string Message { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
