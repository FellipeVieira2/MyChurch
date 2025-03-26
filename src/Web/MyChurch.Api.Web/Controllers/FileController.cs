using Microsoft.AspNetCore.Mvc;
using MyChurch.Infrastructure.Utils.S3;

namespace MyChurch.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FileController : ControllerBase
    {
        private readonly IS3Helper _s3Helper;

        public FileController(IS3Helper s3Helper)
        {
            _s3Helper = s3Helper;
        }

        [HttpGet("storage")]
        public async Task<IActionResult> DownloadFile([FromQuery] string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return BadRequest("File name is required.");
            }

            var fileStream = await _s3Helper.DownloadFileAsync(fileName);

            if (fileStream == null)
            {
                return NotFound();
            }

            return File(fileStream, "application/octet-stream", fileName);
        }
    }
}