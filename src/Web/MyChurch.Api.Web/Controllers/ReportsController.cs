using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyChurch.Application.Plans.Services;
using MyChurch.Application.Reports.Queries.GetDashboardMetrics;
using MyChurch.Application.Reports.Queries.GetDepartmentFinancialReport;
using MyChurch.Application.Reports.Queries.GetFinancialReport;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Enum;
using Mychurch.Common.Services;

namespace MyChurch.Api.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ReportsController : BaseController
    {
        private readonly IReportGeneratorService _reportGenerator;
        private readonly IUnitOfWork _uow;

        public ReportsController(IReportGeneratorService reportGenerator, IUnitOfWork uow)
        {
            _reportGenerator = reportGenerator;
            _uow = uow;
        }

        /// <summary>
        /// ?? Dashboard com métricas gerais da igreja
        /// </summary>
        [HttpGet("dashboard")]
        [Authorize(Roles = "Admin,Leader")]
        public async Task<IActionResult> GetDashboardMetrics([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            var query = AuthorizationRequestCreate<GetDashboardMetricsQuery>();
            query.StartDate = startDate;
            query.EndDate = endDate;

            var result = await Mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Relatório financeiro (entrada/saída/saldo) agrupado por departamento.
        /// Admin vê todos. Membro vê apenas os departamentos que participa (e opcionalmente o geral).
        /// </summary>
        [HttpGet("department-financial")]
        public async Task<IActionResult> GetDepartmentFinancial([FromQuery] GetDepartmentFinancialReportQuery query)
        {
            var q = AuthorizationRequestCreate<GetDepartmentFinancialReportQuery>();
            q.DepartmentId = query.DepartmentId;
            q.StartDate = query.StartDate;
            q.EndDate = query.EndDate;
            q.IncludeGeneral = query.IncludeGeneral;

            var result = await Mediator.Send(q);
            return Ok(result);
        }

        /// <summary>
        /// ?? Relatório Financeiro (JSON)
        /// </summary>
        [HttpGet("financial")]
        [Authorize(Roles = UserRoleAccess.FinancialViewRoles)]
        public async Task<IActionResult> GetFinancialReport(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate,
            [FromQuery] int? departmentId = null)
        {
            var q = AuthorizationRequestCreate<GetFinancialReportQuery>();
            q.StartDate = startDate;
            q.EndDate = endDate;
            q.DepartmentId = departmentId;

            var result = await Mediator.Send(q);
            return Ok(result);
        }

        /// <summary>
        /// ?? Exportar Relatório Financeiro em PDF
        /// </summary>
        [HttpGet("financial/pdf")]
        [Authorize(Roles = UserRoleAccess.FinancialViewRoles)]
        public async Task<IActionResult> ExportFinancialReportPdf(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate,
            [FromQuery] int? departmentId = null,
            [FromServices] IPlanLimitService planLimits = null!)
        {
            var jwt = HttpContext?.Items["User"] as MyChurch.Application.Dtos.JwtMemberDto;
            if (jwt == null || jwt.UserId <= 0) return Unauthorized();

            var member = await _uow.Members.Query().AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == jwt.UserId, HttpContext.RequestAborted);

            if (member == null) return Unauthorized();

            await planLimits.EnsureExportAllowedAsync(member.ChurchId, ExportType.Pdf, HttpContext.RequestAborted);

            var q = AuthorizationRequestCreate<GetFinancialReportQuery>();
            q.StartDate = startDate;
            q.EndDate = endDate;
            q.DepartmentId = departmentId;

            var report = await Mediator.Send(q);

            var churchName = await _uow.Churchs.Query().AsNoTracking()
                .Where(c => c.Id == member.ChurchId)
                .Select(c => c.Name)
                .FirstOrDefaultAsync(HttpContext.RequestAborted) ?? "MyChurch";

            var bytes = await _reportGenerator.GenerateFinancialReportPdfAsync(report, churchName);
            var fileName = $"financial-report_{DateTime.UtcNow:yyyyMMddHHmmss}.pdf";
            return File(bytes, "application/pdf", fileName);
        }

        /// <summary>
        /// ?? Exportar Relatório Financeiro em Excel
        /// </summary>
        [HttpGet("financial/excel")]
        [Authorize(Roles = UserRoleAccess.FinancialViewRoles)]
        public async Task<IActionResult> ExportFinancialReportExcel(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate,
            [FromQuery] int? departmentId = null,
            [FromServices] IPlanLimitService planLimits = null!)
        {
            var jwt = HttpContext?.Items["User"] as MyChurch.Application.Dtos.JwtMemberDto;
            if (jwt == null || jwt.UserId <= 0) return Unauthorized();

            var member = await _uow.Members.Query().AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == jwt.UserId, HttpContext.RequestAborted);

            if (member == null) return Unauthorized();

            // Excel está sendo tratado como exportação tabular; usamos a permissão CanExportCsv
            await planLimits.EnsureExportAllowedAsync(member.ChurchId, ExportType.Csv, HttpContext.RequestAborted);

            var q = AuthorizationRequestCreate<GetFinancialReportQuery>();
            q.StartDate = startDate;
            q.EndDate = endDate;
            q.DepartmentId = departmentId;

            var report = await Mediator.Send(q);

            var churchName = await _uow.Churchs.Query().AsNoTracking()
                .Where(c => c.Id == member.ChurchId)
                .Select(c => c.Name)
                .FirstOrDefaultAsync(HttpContext.RequestAborted) ?? "MyChurch";

            var bytes = await _reportGenerator.GenerateFinancialReportExcelAsync(report, churchName);
            var fileName = $"financial-report_{DateTime.UtcNow:yyyyMMddHHmmss}.xlsx";
            return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        /// <summary>
        /// ?? Relatório de Engajamento dos Membros
        /// </summary>
        [HttpGet("engagement")]
        public async Task<IActionResult> GetEngagementReport([FromQuery] DateTime startDate, [FromQuery] DateTime endDate, [FromQuery] string? format = "json")
        {
            return Ok(new { message = "Engagement report endpoint - to be implemented" });
        }

        /// <summary>
        /// ?? Análise de Visitantes e Conversão
        /// </summary>
        [HttpGet("visitors")]
        public async Task<IActionResult> GetVisitorAnalytics([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            return Ok(new { message = "Visitor analytics endpoint - to be implemented" });
        }
    }
}
