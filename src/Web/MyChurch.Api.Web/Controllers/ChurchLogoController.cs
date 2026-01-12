using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using MyChurch.Application.Church.Commands.UploadChurchLogo;

namespace MyChurch.Api.Web.Controllers
{
    [ApiController]
    [Route("api/church")]
    public class ChurchLogoController : BaseController
    {
        public class UploadLogoRequest
        {
            public string? Base64Image { get; set; }
            public string? FileName { get; set; }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("{id:int}/logo")]
        [EnableRateLimiting("upload-limiter")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> UploadLogo([FromRoute] int id, [FromBody] UploadLogoRequest request)
        {
            var command = AuthorizationRequestCreate<UploadChurchLogoCommand>();
            command.ChurchId = id;
            command.Base64Image = request.Base64Image;
            command.FileName = request.FileName;

            var result = await Mediator.Send(command);
            return Ok(new { logoUrl = result.LogoUrl });
        }
    }
}
