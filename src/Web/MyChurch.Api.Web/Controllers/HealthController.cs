using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyChurch.Infrastructure.Utils.S3;

namespace MyChurch.Api.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Get() => Ok(new { status = "ok" });

        [HttpGet("s3")]
        [AllowAnonymous]
        public async Task<IActionResult> CheckS3([FromServices] IS3Helper s3Helper, CancellationToken cancellationToken)
        {
            await s3Helper.CheckConnectionAsync(cancellationToken);
            return Ok(new { status = "ok" });
        }
    }
}
