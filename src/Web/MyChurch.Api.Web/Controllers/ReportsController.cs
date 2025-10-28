using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyChurch.Application.Reports.Queries.GetDashboardMetrics;

namespace MyChurch.Api.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin,Leader")]
    public class ReportsController : BaseController
    {
        /// <summary>
        /// ?? Dashboard com métricas gerais da igreja
        /// </summary>
        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboardMetrics([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            var query = AuthorizationRequestCreate<GetDashboardMetricsQuery>();
            query.StartDate = startDate;
            query.EndDate = endDate;

            var result = await Mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// ?? Relatório Financeiro Detalhado
        /// </summary>
        [HttpGet("financial")]
        public async Task<IActionResult> GetFinancialReport([FromQuery] DateTime startDate, [FromQuery] DateTime endDate, [FromQuery] string? format = "json")
        {
            // TODO: Implementar GetFinancialReportQuery
            return Ok(new { message = "Financial report endpoint - to be implemented" });
        }

        /// <summary>
        /// ?? Relatório de Engajamento dos Membros
        /// </summary>
        [HttpGet("engagement")]
        public async Task<IActionResult> GetEngagementReport([FromQuery] DateTime startDate, [FromQuery] DateTime endDate, [FromQuery] string? format = "json")
        {
            // TODO: Implementar GetEngagementReportQuery
            return Ok(new { message = "Engagement report endpoint - to be implemented" });
        }

        /// <summary>
        /// ?? Análise de Visitantes e Conversão
        /// </summary>
        [HttpGet("visitors")]
        public async Task<IActionResult> GetVisitorAnalytics([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            // TODO: Implementar GetVisitorAnalyticsQuery
            return Ok(new { message = "Visitor analytics endpoint - to be implemented" });
        }

        /// <summary>
        /// ?? Exportar Relatório Financeiro em PDF
        /// </summary>
        [HttpGet("financial/pdf")]
        public async Task<IActionResult> ExportFinancialReportPdf([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            // TODO: Implementar geração de PDF
            return Ok(new { message = "PDF export - to be implemented" });
        }

        /// <summary>
        /// ?? Exportar Relatório Financeiro em Excel
        /// </summary>
        [HttpGet("financial/excel")]
        public async Task<IActionResult> ExportFinancialReportExcel([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            // TODO: Implementar geração de Excel
            return Ok(new { message = "Excel export - to be implemented" });
        }
    }
}
