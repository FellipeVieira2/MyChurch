namespace MyChurch.Application.Reports.Dtos
{
    /// <summary>
    /// Relatório financeiro detalhado
    /// </summary>
    public class FinancialReportDto
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        // ?? RECEITAS
        public decimal TotalDonations { get; set; }
        public decimal TotalTithes { get; set; }
        public decimal TotalOfferings { get; set; }
        public decimal OtherIncome { get; set; }
        public decimal TotalIncome { get; set; }

        // ?? DESPESAS
        public decimal TotalExpenses { get; set; }
        public List<ExpenseByCategoryDto> ExpensesByCategory { get; set; } = new();

        // ?? BALANÇO
        public decimal NetBalance { get; set; }

        // ?? DETALHES POR MÊS
        public List<MonthlyFinancialSummaryDto> MonthlySummary { get; set; } = new();

        // ?? TOP DOADORES
        public List<TopDonorDetailDto> TopDonors { get; set; } = new();

        // ?? GRÁFICOS
        public List<DonationTrendDto> DonationTrend { get; set; } = new();
    }

    public class ExpenseByCategoryDto
    {
        public string CategoryName { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal Percentage { get; set; }
    }

    public class MonthlyFinancialSummaryDto
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public string MonthName { get; set; }
        public decimal TotalIncome { get; set; }
        public decimal TotalExpenses { get; set; }
        public decimal NetBalance { get; set; }
    }

    public class TopDonorDetailDto
    {
        public int MemberId { get; set; }
        public string MemberName { get; set; }
        public string Email { get; set; }
        public decimal TotalDonated { get; set; }
        public int DonationCount { get; set; }
        public decimal AverageDonation { get; set; }
    }

    public class DonationTrendDto
    {
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
    }
}
