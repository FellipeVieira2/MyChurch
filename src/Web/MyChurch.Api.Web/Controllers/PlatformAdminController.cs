using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyChurch.Application.Common.Models;
using MyChurch.Application.PlatformAdmin.Dtos;
using MyChurch.Application.PlatformAdmin.Queries.GetChurchKpis;
using MyChurch.Application.PlatformAdmin.Queries.GetChurchLeads;
using MyChurch.Application.PlatformAdmin.Queries.GetPlatformDashboard;
using MyChurch.Application.PlatformAdmin.Queries.GetPlatformDonationSummary;
using MyChurch.Application.PlatformAdmin.Queries.GetPlatformTransferSummary;

namespace MyChurch.Api.Web.Controllers
{
    [ApiController]
    [Route("api/platform-admin")]
    [Authorize(Roles = "PlatformAdmin")]
    public class PlatformAdminController : BaseController
    {
        [HttpGet("churches")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<ChurchLeadDto>))]
        public async Task<IActionResult> GetChurchLeads([FromQuery] GetChurchLeadsQuery query)
        {
            var result = await Mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("dashboard")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PlatformDashboardDto))]
        public async Task<IActionResult> GetDashboard([FromQuery] GetPlatformDashboardQuery query)
        {
            var result = await Mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("kpis/churches")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PaginatedList<ChurchKpiDto>))]
        public async Task<IActionResult> GetChurchKpis([FromQuery] GetChurchKpisQuery query)
        {
            var result = await Mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("kpis/donations")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PlatformDonationSummaryDto))]
        public async Task<IActionResult> GetDonationsKpis([FromQuery] GetPlatformDonationSummaryQuery query)
        {
            var result = await Mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("kpis/transfers")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PlatformTransferSummaryDto))]
        public async Task<IActionResult> GetTransfersKpis([FromQuery] GetPlatformTransferSummaryQuery query)
        {
            var result = await Mediator.Send(query);
            return Ok(result);
        }
    }
}
