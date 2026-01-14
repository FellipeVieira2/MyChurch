using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MyChurch.Api.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChurchController : ControllerBase
    {
        // Legacy/compat endpoints: o controller antigo retornava 501. 
        // Mantemos como redirect para as rotas públicas reais.

        // GET /api/Church/{id}  ->  GET /api/church/public/{id}
        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public IActionResult RedirectToPublicDetails([FromRoute] int id)
            => Redirect($"/api/church/public/{id}");

        // GET /api/Church/public/nearby -> GET /api/church/public/nearby
        [HttpGet("public/nearby")]
        [AllowAnonymous]
        public IActionResult RedirectToPublicNearby([FromQuery] double latitude, [FromQuery] double longitude, [FromQuery] double radiusKm = 5, [FromQuery] int? maxResults = 50)
        {
            var qs = $"latitude={latitude}&longitude={longitude}&radiusKm={radiusKm}&maxResults={(maxResults ?? 50)}";
            return Redirect($"/api/church/public/nearby?{qs}");
        }
    }
}