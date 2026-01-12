using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyChurch.Application.Donation.Queries.GetTransferHistory;

namespace MyChurch.Api.Web.Controllers
{
    [ApiController]
    [Route("api/transfers")]
    public class TransferHistoryController : BaseController
    {
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Get([FromQuery] GetTransferHistoryQuery query)
        {
            var q = AuthorizationRequestCreate<GetTransferHistoryQuery>();
            q.Page = query.Page;
            q.PageSize = query.PageSize;
            q.Status = query.Status;

            var result = await Mediator.Send(q);
            return Ok(result);
        }
    }
}
