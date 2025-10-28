namespace MyChurch.Application.Reports.Dtos
{
    /// <summary>
    /// Análise de visitantes e conversão
    /// </summary>
    public class VisitorAnalyticsDto
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        // ?? VISÃO GERAL
        public int TotalVisitors { get; set; }
        public int NewVisitors { get; set; }
        public int ReturningVisitors { get; set; }
        public int VisitorsNeedingFollowUp { get; set; }

        // ?? CONVERSÃO
        public int VisitorsConverted { get; set; } // Tornaram-se membros
        public decimal ConversionRate { get; set; }
        public decimal AverageDaysToConvert { get; set; }

        // ?? STATUS
        public List<VisitorsByStatusDto> VisitorsByStatus { get; set; } = new();

        // ?? TENDÊNCIA
        public List<VisitorTrendDto> VisitorTrend { get; set; } = new();

        // ?? ENGAGEMENT SCORE
        public decimal AverageVisitorScore { get; set; }
        public List<VisitorScoreDistributionDto> ScoreDistribution { get; set; } = new();

        // ?? VISITANTES ATIVOS
        public List<VisitorDetailDto> MostEngagedVisitors { get; set; } = new();

        // ?? VISITANTES EM RISCO
        public List<VisitorDetailDto> VisitorsAtRisk { get; set; } = new();

        // ?? MÉTRICAS DE FOLLOW-UP
        public int FollowUpEmailsSent { get; set; }
        public int FollowUpResponsesReceived { get; set; }
        public decimal FollowUpResponseRate { get; set; }
    }

    public class VisitorsByStatusDto
    {
        public string StatusName { get; set; }
        public int Count { get; set; }
        public decimal Percentage { get; set; }
    }

    public class VisitorTrendDto
    {
        public DateTime Date { get; set; }
        public int NewVisitors { get; set; }
        public int ReturningVisitors { get; set; }
        public int Converted { get; set; }
    }

    public class VisitorScoreDistributionDto
    {
        public string ScoreRange { get; set; } // "0-20", "21-40", etc.
        public int Count { get; set; }
        public decimal Percentage { get; set; }
    }

    public class VisitorDetailDto
    {
        public int VisitorId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Status { get; set; }
        public int Score { get; set; }
        public DateTime? LastVisitAt { get; set; }
        public int TotalVisits { get; set; }
        public int DaysSinceLastVisit { get; set; }
        public bool NeedsFollowUp { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
