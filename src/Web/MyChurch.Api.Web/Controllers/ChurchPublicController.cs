using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyChurch.Application.Church.Queries.SearchNearby;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using Mychurch.Common.Utils.Objects; // added for PagedResultDto
using System.Linq; // ensure LINQ

namespace MyChurch.Api.Web.Controllers
{
    [ApiController]
    [Route("api/church/public")] // moved under /api/church/public to avoid overlap
    public class ChurchPublicController : BaseController
    {
        private readonly IUnitOfWork _uow;
        private readonly IMediator _mediator;
        public ChurchPublicController(IUnitOfWork uow, IMediator mediator)
        {
            _uow = uow;
            _mediator = mediator;
        }

        // GET: /api/church/public/search
        // Public, paginated, safe fields only
        [HttpGet("search")]
        [AllowAnonymous]
        public async Task<ActionResult<PagedResultDto<PublicChurchListItemDto>>> PublicSearch(
            [FromQuery] string? name,
            [FromQuery] string? city,
            [FromQuery] string? state,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20)
        {
            if (pageNumber <= 0) pageNumber = 1;
            if (pageSize <= 0 || pageSize > 100) pageSize = 20;

            var query = _uow.Churchs.Query().AsNoTracking();

            if (!string.IsNullOrWhiteSpace(name))
            {
                var n = name.Trim().ToLower();
                query = query.Where(c => (c.Name ?? string.Empty).ToLower().Contains(n));
            }
            if (!string.IsNullOrWhiteSpace(city))
            {
                var ct = city.Trim().ToLower();
                query = query.Where(c => (c.Address.City ?? string.Empty).ToLower().Contains(ct));
            }
            if (!string.IsNullOrWhiteSpace(state))
            {
                var st = state.Trim().ToLower();
                query = query.Where(c => (c.Address.State ?? string.Empty).ToLower().Contains(st));
            }

            var total = await query.CountAsync();

            var items = await query
                .OrderByDescending(c => c.TotalVisits)
                .ThenBy(c => c.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new PublicChurchListItemDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    City = c.Address.City,
                    State = c.Address.State,
                    Denomination = c.Denomination,
                    CoverPhoto = c.CoverPhoto,
                    AverageRating = c.AverageRating,
                    TotalReviews = c.TotalReviews,
                    TotalVisits = c.TotalVisits,
                    HasParking = c.HasParking,
                    IsAccessible = c.IsAccessible,
                    HasChildMinistry = c.HasChildMinistry,
                    Latitude = c.Latitude,
                    Longitude = c.Longitude,
                    Logo = c.LogoFileName
                })
                .ToListAsync();

            return Ok(new PagedResultDto<PublicChurchListItemDto>
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = total
            });
        }

        // GET: /api/church/public/nearby
        [HttpGet("nearby")]
        [AllowAnonymous]
        public async Task<ActionResult<List<NearbyChurchDto>>> Nearby([FromQuery] double latitude, [FromQuery] double longitude, [FromQuery] double radiusKm = 5, [FromQuery] int? maxResults = 50)
        {
            var result = await _mediator.Send(new GetNearbyChurchesQuery
            {
                Latitude = latitude,
                Longitude = longitude,
                RadiusKm = radiusKm,
                MaxResults = maxResults
            });
            return Ok(result);
        }

        // GET: /api/church/public/{id}
        // Public details with safe fields
        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<PublicChurchDetailsDto>> PublicDetails([FromRoute] int id)
        {
            var dto = await _uow.Churchs.Query()
                .AsNoTracking()
                .Where(c => c.Id == id)
                .Select(c => new PublicChurchDetailsDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    City = c.Address.City,
                    State = c.Address.State,
                    Denomination = c.Denomination,
                    CoverPhoto = c.CoverPhoto,
                    AverageRating = c.AverageRating,
                    TotalReviews = c.TotalReviews,
                    TotalVisits = c.TotalVisits,
                    HasParking = c.HasParking,
                    IsAccessible = c.IsAccessible,
                    HasChildMinistry = c.HasChildMinistry,
                    Latitude = c.Latitude,
                    Longitude = c.Longitude,
                    Logo = c.LogoFileName,
                    Website = c.Website,
                    Email = c.Email,
                    InstagramUrl = c.InstagramUrl,
                    FacebookUrl = c.FacebookUrl,
                    YoutubeUrl = c.YoutubeUrl,
                    WhatsAppNumber = c.WhatsAppNumber
                })
                .FirstOrDefaultAsync();

            if (dto == null) return NotFound();
            return Ok(dto);
        }

        // GET: /api/church/public/{id}/current-worship
        // Returns the current live worship (or null) so visitors can fetch worshipServiceId
        [HttpGet("{id:int}/current-worship")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCurrentWorship([FromRoute] int id, [FromQuery] int liveWindowHours = 6)
        {
            var now = DateTime.UtcNow;
            var minStart = now.AddHours(-Math.Abs(liveWindowHours));

            // Heuristic: latest worship that started within the window
            var ws = await _uow.WorshipServices.Query()
                .AsNoTracking()
                .Where(w => w.ChurchId == id && w.StartTime <= now && w.StartTime >= minStart)
                .OrderByDescending(w => w.StartTime)
                .Select(w => new CurrentWorshipDto
                {
                    WorshipServiceId = w.Id,
                    StartTime = w.StartTime
                })
                .FirstOrDefaultAsync();

            return Ok(ws); // can be null if none
        }

        // GET: /api/church/public/{id}/upcoming-worships
        // Returns upcoming worships within a time range for public display
        [HttpGet("{id:int}/upcoming-worships")]
        [AllowAnonymous]
        public async Task<IActionResult> GetUpcomingWorships([FromRoute] int id, [FromQuery] int hoursAhead = 48, [FromQuery] int take = 5)
        {
            var now = DateTime.UtcNow;
            var maxTime = now.AddHours(Math.Abs(hoursAhead));

            var list = await _uow.WorshipServices.Query()
                .AsNoTracking()
                .Where(w => w.ChurchId == id && w.StartTime > now && w.StartTime <= maxTime)
                .OrderBy(w => w.StartTime)
                .Take(take <= 0 ? 5 : take)
                .Select(w => new UpcomingWorshipDto
                {
                    WorshipServiceId = w.Id,
                    StartTime = w.StartTime
                })
                .ToListAsync();

            return Ok(list);
        }

        // POST: /api/church/public/{id}/visit
        // Increment simple visit counter (anonymous allowed)
        [HttpPost("{id:int}/visit")]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterVisit([FromRoute] int id)
        {
            var church = await _uow.Churchs.Query().FirstOrDefaultAsync(c => c.Id == id);
            if (church == null) return NotFound();

            church.IncrementVisitCount();
            _uow.Churchs.Update(church);
            await _uow.CommitAsync();
            return Ok(new { success = true, totalVisits = church.TotalVisits });
        }

        // DTOs for public outputs
        public class PublicChurchListItemDto
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string? Description { get; set; }
            public string? City { get; set; }
            public string? State { get; set; }
            public string? Denomination { get; set; }
            public string? CoverPhoto { get; set; }
            public double? AverageRating { get; set; }
            public int TotalReviews { get; set; }
            public int TotalVisits { get; set; }
            public bool HasParking { get; set; }
            public bool IsAccessible { get; set; }
            public bool HasChildMinistry { get; set; }
            public double? Latitude { get; set; }
            public double? Longitude { get; set; }
            public string? Logo { get; set; }
        }

        public class PublicChurchDetailsDto : PublicChurchListItemDto
        {
            public string? Website { get; set; }
            public string? Email { get; set; }
            public string? InstagramUrl { get; set; }
            public string? FacebookUrl { get; set; }
            public string? YoutubeUrl { get; set; }
            public string? WhatsAppNumber { get; set; }
        }

        public class CurrentWorshipDto
        {
            public int WorshipServiceId { get; set; }
            public DateTime StartTime { get; set; }
        }

        public class UpcomingWorshipDto
        {
            public int WorshipServiceId { get; set; }
            public DateTime StartTime { get; set; }
        }
    }
}
