using System.Threading.Tasks;

namespace Mychurch.Common.Services
{
 public interface IReportGeneratorService
 {
 Task<byte[]> GenerateFinancialReportPdfAsync(FinancialReportDto report, string churchName);
 Task<byte[]> GenerateFinancialReportExcelAsync(FinancialReportDto report, string churchName);
 Task<byte[]> GenerateEngagementReportPdfAsync(EngagementReportDto report, string churchName);
 Task<byte[]> GenerateEngagementReportExcelAsync(EngagementReportDto report, string churchName);
 Task<byte[]> GenerateVisitorAnalyticsPdfAsync(VisitorAnalyticsDto report, string churchName);
 Task<byte[]> GenerateVisitorAnalyticsExcelAsync(VisitorAnalyticsDto report, string churchName);
 }
}