using System;
using System.Collections.Generic;

namespace Mychurch.Common.Services
{
 public class FinancialReportDto
 {
 public DateTime StartDate { get; set; }
 public DateTime EndDate { get; set; }
 public decimal TotalDonations { get; set; }
 public decimal TotalTithes { get; set; }
 public decimal TotalOfferings { get; set; }
 public decimal OtherIncome { get; set; }
 public decimal TotalIncome { get; set; }
 public decimal TotalExpenses { get; set; }
 public List<ExpenseByCategoryDto> ExpensesByCategory { get; set; } = new();
 public decimal NetBalance { get; set; }
 public List<MonthlyFinancialSummaryDto> MonthlySummary { get; set; } = new();
 public List<TopDonorDetailDto> TopDonors { get; set; } = new();
 public List<DonationTrendDto> DonationTrend { get; set; } = new();
 }

 public class ExpenseByCategoryDto { public string CategoryName { get; set; } = ""; public decimal TotalAmount { get; set; } public decimal Percentage { get; set; } }
 public class MonthlyFinancialSummaryDto { }
 public class TopDonorDetailDto { public string MemberName { get; set; } = ""; public decimal TotalDonated { get; set; } public int DonationCount { get; set; } public decimal AverageDonation { get; set; } }
 public class DonationTrendDto { }

 public class EngagementReportDto { public DateTime StartDate { get; set; } public DateTime EndDate { get; set; } public List<MembersByFaithLevelDto> MembersByFaithLevel { get; set; } = new(); public List<MemberEngagementDetailDto> MembersEngagement { get; set; } = new(); public List<PastoralAlertDto> PastoralAlerts { get; set; } = new(); }
 public class MembersByFaithLevelDto { }
 public class MemberEngagementDetailDto { }
 public class PastoralAlertDto { public int Id { get; set; } public int MemberId { get; set; } public string Message { get; set; } = ""; public string Source { get; set; } = ""; public bool IsRead { get; set; } public DateTime CreatedAt { get; set; } }

 public class VisitorAnalyticsDto { public DateTime StartDate { get; set; } public DateTime EndDate { get; set; } public List<VisitorsByStatusDto> VisitorsByStatus { get; set; } = new(); public List<VisitorTrendDto> VisitorTrend { get; set; } = new(); public List<VisitorScoreDistributionDto> ScoreDistribution { get; set; } = new(); public List<VisitorDetailDto> MostEngagedVisitors { get; set; } = new(); public List<VisitorDetailDto> VisitorsAtRisk { get; set; } = new(); }
 public class VisitorsByStatusDto { }
 public class VisitorTrendDto { }
 public class VisitorScoreDistributionDto { }
 public class VisitorDetailDto { }
}