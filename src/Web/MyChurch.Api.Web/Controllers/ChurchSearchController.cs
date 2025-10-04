using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyChurch.Application.Church.Queries.SearchNearby;
using MyChurch.Application.Dtos;

namespace MyChurch.Api.Web.Controllers
{
    /// <summary>
    /// Controller para busca e descoberta de igrejas
    /// Sistema similar ao TripAdvisor
    /// </summary>
    [ApiController]
    [Route("api/church")]
    public class ChurchSearchController : BaseController
    {
        private readonly IMediator _mediator;

        public ChurchSearchController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// ?? Buscar igrejas próximas por geolocalização
        /// </summary>
        /// <param name="latitude">Latitude do ponto de busca</param>
        /// <param name="longitude">Longitude do ponto de busca</param>
        /// <param name="radiusKm">Raio de busca em km (padrão: 10km)</param>
        /// <param name="page">Página (padrão: 1)</param>
        /// <param name="pageSize">Tamanho da página (padrão: 20)</param>
        /// <param name="denominations">Filtro por denominações (ex: Batista,Assembleia)</param>
        /// <param name="languages">Filtro por idiomas (ex: Português,Inglês)</param>
        /// <param name="minRating">Avaliação mínima (1-5)</param>
        /// <param name="hasParking">Filtrar apenas com estacionamento</param>
        /// <param name="isAccessible">Filtrar apenas acessíveis</param>
        /// <param name="hasLiveStream">Filtrar apenas com transmissão ao vivo</param>
        /// <param name="hasChildMinistry">Filtrar apenas com ministério infantil</param>
        /// <param name="isVerified">Filtrar apenas igrejas verificadas</param>
        /// <param name="sortBy">Ordenar por: distance, rating, popularity, name (padrão: distance)</param>
        /// <response code="200">Lista de igrejas encontradas</response>
        [HttpGet("search/nearby")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ChurchSearchResultDto), 200)]
        public async Task<ActionResult<ChurchSearchResultDto>> SearchNearby(
            [FromQuery] double latitude,
            [FromQuery] double longitude,
            [FromQuery] int radiusKm = 10,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] string? denominations = null,
            [FromQuery] string? languages = null,
            [FromQuery] int? minRating = null,
            [FromQuery] bool? hasParking = null,
            [FromQuery] bool? isAccessible = null,
            [FromQuery] bool? hasLiveStream = null,
            [FromQuery] bool? hasChildMinistry = null,
            [FromQuery] bool? isVerified = null,
            [FromQuery] string sortBy = "distance")
        {
            var query = new SearchNearbyChurchesQuery
            {
                Latitude = latitude,
                Longitude = longitude,
                RadiusKm = radiusKm,
                Page = page,
                PageSize = pageSize,
                Denominations = denominations?.Split(',').ToList(),
                Languages = languages?.Split(',').ToList(),
                MinRating = minRating,
                HasParking = hasParking,
                IsAccessible = isAccessible,
                HasLiveStream = hasLiveStream,
                HasChildMinistry = hasChildMinistry,
                IsVerified = isVerified,
                SortBy = sortBy
            };

            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// ??? Buscar igrejas em uma cidade
        /// </summary>
        /// <param name="city">Nome da cidade</param>
        /// <param name="state">Estado (UF)</param>
        /// <param name="page">Página</param>
        /// <param name="pageSize">Tamanho da página</param>
        /// <param name="sortBy">Ordenação</param>
        [HttpGet("search/city")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ChurchSearchResultDto), 200)]
        public async Task<ActionResult<ChurchSearchResultDto>> SearchByCity(
            [FromQuery] string city,
            [FromQuery] string? state = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] string sortBy = "rating")
        {
            // TODO: Implementar busca por cidade
            // Por enquanto, retornar vazio
            return Ok(new ChurchSearchResultDto
            {
                Churches = new List<ChurchSearchCardDto>(),
                TotalResults = 0,
                CurrentPage = page,
                PageSize = pageSize,
                TotalPages = 0,
                HasNextPage = false,
                HasPreviousPage = false
            });
        }

        /// <summary>
        /// ?? Buscar top igrejas ranqueadas
        /// </summary>
        /// <param name="city">Cidade (opcional)</param>
        /// <param name="rankingType">Tipo: top-rated, most-popular, trending, family-friendly</param>
        /// <param name="limit">Limite de resultados (padrão: 10)</param>
        [HttpGet("rankings")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(List<ChurchSearchCardDto>), 200)]
        public async Task<ActionResult<List<ChurchSearchCardDto>>> GetRankings(
            [FromQuery] string? city = null,
            [FromQuery] string rankingType = "top-rated",
            [FromQuery] int limit = 10)
        {
            // TODO: Implementar rankings
            return Ok(new List<ChurchSearchCardDto>());
        }

        /// <summary>
        /// ?? Autocomplete de igrejas para busca
        /// </summary>
        /// <param name="query">Texto de busca</param>
        /// <param name="limit">Limite de sugestões (padrão: 5)</param>
        [HttpGet("search/autocomplete")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(List<ChurchAutocompleteDto>), 200)]
        public async Task<ActionResult<List<ChurchAutocompleteDto>>> Autocomplete(
            [FromQuery] string query,
            [FromQuery] int limit = 5)
        {
            // TODO: Implementar autocomplete
            return Ok(new List<ChurchAutocompleteDto>());
        }
    }

    /// <summary>
    /// DTO para autocomplete
    /// </summary>
    public class ChurchAutocompleteDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public double? Rating { get; set; }
    }
}
