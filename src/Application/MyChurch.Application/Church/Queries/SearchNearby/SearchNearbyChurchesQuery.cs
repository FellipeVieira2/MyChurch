using MediatR;
using Microsoft.EntityFrameworkCore;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using System.Linq.Expressions;

namespace MyChurch.Application.Church.Queries.SearchNearby
{
    /// <summary>
    /// Query para buscar igrejas próximas usando geolocalização
    /// Sistema similar ao TripAdvisor
    /// </summary>
    public class SearchNearbyChurchesQuery : IRequest<ChurchSearchResultDto>
    {
        // ?? LOCALIZAÇÃO
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int RadiusKm { get; set; } = 10; // Raio padrão 10km
        
        // ?? PAGINAÇÃO
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        
        // ?? FILTROS
        public List<string>? Denominations { get; set; } // Ex: ["Batista", "Assembleia"]
        public List<string>? Languages { get; set; } // Ex: ["Português", "Inglês"]
        public List<DayOfWeek>? WorshipDays { get; set; } // Ex: [Sunday, Wednesday]
        public TimeSpan? PreferredTime { get; set; } // Ex: 10:00 AM
        public int? MinRating { get; set; } // Ex: 4 (apenas igrejas com 4+ estrelas)
        
        // ?? CARACTERÍSTICAS
        public bool? HasParking { get; set; }
        public bool? IsAccessible { get; set; }
        public bool? HasLiveStream { get; set; }
        public bool? HasChildMinistry { get; set; }
        public bool? IsVerified { get; set; }
        
        // ?? ORDENAÇÃO
        public string SortBy { get; set; } = "distance"; // distance, rating, popularity, name
    }
    
    public class SearchNearbyChurchesQueryHandler : IRequestHandler<SearchNearbyChurchesQuery, ChurchSearchResultDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private const double EARTH_RADIUS_KM = 6371.0;

        public SearchNearbyChurchesQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ChurchSearchResultDto> Handle(SearchNearbyChurchesQuery request, CancellationToken cancellationToken)
        {
            // Buscar todas as igrejas com coordenadas
            var churchesQuery = _unitOfWork.Churchs.Query()
                .Include(c => c.Address)
                .Include(c => c.Subscription)
                .Where(c => c.Latitude.HasValue && c.Longitude.HasValue);

            // Aplicar filtros
            churchesQuery = ApplyFilters(churchesQuery, request);

            // Buscar igrejas e calcular distância
            var allChurches = await churchesQuery.ToListAsync(cancellationToken);

            var churchesWithDistance = allChurches
                .Select(c => new
                {
                    Church = c,
                    Distance = CalculateDistance(
                        request.Latitude, request.Longitude,
                        c.Latitude!.Value, c.Longitude!.Value)
                })
                .Where(x => x.Distance <= request.RadiusKm)
                .ToList();

            // Ordenar
            churchesWithDistance = SortResults(churchesWithDistance, request.SortBy);

            // Paginar
            var totalResults = churchesWithDistance.Count;
            var pagedChurches = churchesWithDistance
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            // Mapear para DTOs
            var churchCards = pagedChurches.Select(x => MapToChurchCard(x.Church, x.Distance)).ToList();

            // Buscar fotos e reviews para enriquecer os dados
            await EnrichChurchCards(churchCards, cancellationToken);

            // Calcular limites do mapa
            var mapBounds = CalculateMapBounds(pagedChurches.Select(x => x.Church).ToList());

            // Gerar sugestões
            var suggestions = GenerateSearchSuggestions(allChurches, request);

            // Gerar filtros disponíveis
            var availableFilters = GenerateAvailableFilters(allChurches);

            // Calcular paginação
            var totalPages = (int)Math.Ceiling(totalResults / (double)request.PageSize);

            return new ChurchSearchResultDto
            {
                Churches = churchCards,
                TotalResults = totalResults,
                CurrentPage = request.Page,
                PageSize = request.PageSize,
                TotalPages = totalPages,
                HasNextPage = request.Page < totalPages,
                HasPreviousPage = request.Page > 1,
                MapBounds = mapBounds,
                Suggestions = suggestions,
                AvailableFilters = availableFilters
            };
        }

        /// <summary>
        /// Aplica filtros na query
        /// </summary>
        private IQueryable<Domain.Entities.Church> ApplyFilters(
            IQueryable<Domain.Entities.Church> query, 
            SearchNearbyChurchesQuery request)
        {
            // Filtro por denominação
            if (request.Denominations != null && request.Denominations.Any())
            {
                query = query.Where(c => request.Denominations.Contains(c.Denomination));
            }

            // Filtro por avaliação mínima
            if (request.MinRating.HasValue)
            {
                query = query.Where(c => c.AverageRating >= request.MinRating.Value);
            }

            // Filtro por características
            if (request.HasParking == true)
                query = query.Where(c => c.HasParking);

            if (request.IsAccessible == true)
                query = query.Where(c => c.IsAccessible);

            if (request.HasLiveStream == true)
                query = query.Where(c => c.HasLiveStream);

            if (request.HasChildMinistry == true)
                query = query.Where(c => c.HasChildMinistry);

            if (request.IsVerified == true)
                query = query.Where(c => c.IsVerified);

            // Filtro por idiomas (JSON array)
            if (request.Languages != null && request.Languages.Any())
            {
                foreach (var language in request.Languages)
                {
                    query = query.Where(c => c.Languages != null && c.Languages.Contains(language));
                }
            }

            return query;
        }

        /// <summary>
        /// Calcula distância usando fórmula Haversine
        /// </summary>
        private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            var dLat = ToRadians(lat2 - lat1);
            var dLon = ToRadians(lon2 - lon1);

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            
            return EARTH_RADIUS_KM * c;
        }

        private double ToRadians(double degrees) => degrees * Math.PI / 180.0;

        /// <summary>
        /// Ordena resultados
        /// </summary>
        private List<T> SortResults<T>(List<T> churches, string sortBy) where T : class
        {
            return sortBy.ToLower() switch
            {
                "rating" => churches.OrderByDescending(x => {
                    var prop = x.GetType().GetProperty("Church");
                    var church = prop?.GetValue(x) as Domain.Entities.Church;
                    return church?.AverageRating ?? 0;
                }).ThenBy(x => {
                    var prop = x.GetType().GetProperty("Distance");
                    return prop?.GetValue(x) ?? 0;
                }).ToList(),
                "popularity" => churches.OrderByDescending(x => {
                    var prop = x.GetType().GetProperty("Church");
                    var church = prop?.GetValue(x) as Domain.Entities.Church;
                    return church?.TotalVisits ?? 0;
                }).ThenBy(x => {
                    var prop = x.GetType().GetProperty("Distance");
                    return prop?.GetValue(x) ?? 0;
                }).ToList(),
                "name" => churches.OrderBy(x => {
                    var prop = x.GetType().GetProperty("Church");
                    var church = prop?.GetValue(x) as Domain.Entities.Church;
                    return church?.Name ?? "";
                }).ToList(),
                _ => churches.OrderBy(x => {
                    var prop = x.GetType().GetProperty("Distance");
                    return prop?.GetValue(x) ?? 0;
                }).ToList() // distance (padrão)
            };
        }

        /// <summary>
        /// Mapeia entidade para DTO
        /// </summary>
        private ChurchSearchCardDto MapToChurchCard(Domain.Entities.Church church, double distance)
        {
            var features = new List<string>();
            if (church.HasParking) features.Add("Estacionamento");
            if (church.IsAccessible) features.Add("Acessível");
            if (church.HasLiveStream) features.Add("Transmissão ao vivo");
            if (church.HasChildMinistry) features.Add("Ministério infantil");

            var badges = new List<string>();
            if (church.IsVerified) badges.Add("Verificada");
            if (church.AverageRating >= 4.5) badges.Add("Excelente");
            if (church.TotalReviews >= 50) badges.Add("Popular");

            return new ChurchSearchCardDto
            {
                Id = church.Id,
                Name = church.Name,
                CoverPhoto = church.CoverPhoto,
                Logo = church.LogoFileName,
                AverageRating = church.AverageRating,
                TotalReviews = church.TotalReviews,
                DistanceKm = Math.Round(distance, 2),
                Address = $"{church.Address.Street}, {church.Address.Number}",
                City = church.Address.City,
                State = church.Address.State,
                Denomination = church.Denomination,
                IsVerified = church.IsVerified,
                Badges = badges,
                Features = features,
                TotalVisits = church.TotalVisits,
                Latitude = church.Latitude,
                Longitude = church.Longitude
            };
        }

        /// <summary>
        /// Enriquece cards com fotos e reviews
        /// </summary>
        private async Task EnrichChurchCards(List<ChurchSearchCardDto> cards, CancellationToken cancellationToken)
        {
            var churchIds = cards.Select(c => c.Id).ToList();

            // Buscar total de fotos
            var photosCounts = await _unitOfWork.ChurchPhotos.Query()
                .Where(p => churchIds.Contains(p.ChurchId) && p.IsApproved)
                .GroupBy(p => p.ChurchId)
                .Select(g => new { ChurchId = g.Key, Count = g.Count() })
                .ToListAsync(cancellationToken);

            foreach (var card in cards)
            {
                var photoCount = photosCounts.FirstOrDefault(p => p.ChurchId == card.Id);
                card.TotalPhotos = photoCount?.Count ?? 0;
            }

            // Buscar top review highlights (3 mais úteis)
            var topReviews = await _unitOfWork.Reviews.Query()
                .Where(r => churchIds.Contains(r.EntityId) && r.EntityType == "Church")
                .Include(r => r.Votes)
                .ToListAsync(cancellationToken);

            var reviewsGrouped = topReviews
                .GroupBy(r => r.EntityId)
                .Select(g => new
                {
                    ChurchId = g.Key,
                    Highlights = g.OrderByDescending(r => r.Votes.Count(v => v.IsHelpful) - r.Votes.Count(v => !v.IsHelpful))
                                 .Take(3)
                                 .Select(r => r.Comment)
                                 .ToList()
                })
                .ToList();

            foreach (var card in cards)
            {
                var reviews = reviewsGrouped.FirstOrDefault(r => r.ChurchId == card.Id);
                if (reviews != null && reviews.Highlights != null)
                {
                    card.TopReviewHighlights = reviews.Highlights
                        .Where(h => !string.IsNullOrEmpty(h))
                        .Select(h => h.Length > 100 ? h.Substring(0, 100) + "..." : h)
                        .ToList();
                }
            }
        }

        /// <summary>
        /// Calcula limites do mapa
        /// </summary>
        private MapBoundsDto? CalculateMapBounds(List<Domain.Entities.Church> churches)
        {
            if (!churches.Any()) return null;

            var lats = churches.Select(c => c.Latitude!.Value).ToList();
            var lons = churches.Select(c => c.Longitude!.Value).ToList();

            return new MapBoundsDto
            {
                MinLatitude = lats.Min(),
                MaxLatitude = lats.Max(),
                MinLongitude = lons.Min(),
                MaxLongitude = lons.Max(),
                CenterLatitude = lats.Average(),
                CenterLongitude = lons.Average()
            };
        }

        /// <summary>
        /// Gera sugestões de busca
        /// </summary>
        private SearchSuggestionsDto GenerateSearchSuggestions(
            List<Domain.Entities.Church> allChurches, 
            SearchNearbyChurchesQuery request)
        {
            var nearbyDenominations = allChurches
                .Where(c => !string.IsNullOrEmpty(c.Denomination))
                .GroupBy(c => c.Denomination)
                .OrderByDescending(g => g.Count())
                .Take(5)
                .Select(g => g.Key!)
                .ToList();

            return new SearchSuggestionsDto
            {
                NearbyDenominations = nearbyDenominations,
                PopularSearches = new List<string>
                {
                    "Igrejas com culto em inglês",
                    "Igrejas para jovens",
                    "Igrejas com ministério infantil",
                    "Igrejas acessíveis"
                }
            };
        }

        /// <summary>
        /// Gera filtros disponíveis com contagem
        /// </summary>
        private List<FilterOptionDto> GenerateAvailableFilters(List<Domain.Entities.Church> churches)
        {
            var filters = new List<FilterOptionDto>();

            // Filtro de denominação
            var denominations = churches
                .Where(c => !string.IsNullOrEmpty(c.Denomination))
                .GroupBy(c => c.Denomination)
                .Select(g => new FilterValueDto
                {
                    Value = g.Key!,
                    Label = g.Key!,
                    Count = g.Count()
                })
                .OrderByDescending(f => f.Count)
                .ToList();

            if (denominations.Any())
            {
                filters.Add(new FilterOptionDto
                {
                    Key = "denomination",
                    Label = "Denominação",
                    Type = "checkbox",
                    Values = denominations
                });
            }

            // Filtro de características
            var features = new List<FilterValueDto>();
            
            var parkingCount = churches.Count(c => c.HasParking);
            if (parkingCount > 0)
                features.Add(new FilterValueDto { Value = "parking", Label = "Estacionamento", Count = parkingCount });

            var accessibleCount = churches.Count(c => c.IsAccessible);
            if (accessibleCount > 0)
                features.Add(new FilterValueDto { Value = "accessible", Label = "Acessível", Count = accessibleCount });

            var liveStreamCount = churches.Count(c => c.HasLiveStream);
            if (liveStreamCount > 0)
                features.Add(new FilterValueDto { Value = "livestream", Label = "Transmissão ao vivo", Count = liveStreamCount });

            var childMinistryCount = churches.Count(c => c.HasChildMinistry);
            if (childMinistryCount > 0)
                features.Add(new FilterValueDto { Value = "childministry", Label = "Ministério infantil", Count = childMinistryCount });

            if (features.Any())
            {
                filters.Add(new FilterOptionDto
                {
                    Key = "features",
                    Label = "Características",
                    Type = "checkbox",
                    Values = features
                });
            }

            // Filtro de avaliação
            filters.Add(new FilterOptionDto
            {
                Key = "rating",
                Label = "Avaliação",
                Type = "select",
                Values = new List<FilterValueDto>
                {
                    new() { Value = "4", Label = "4+ estrelas", Count = churches.Count(c => c.AverageRating >= 4) },
                    new() { Value = "3", Label = "3+ estrelas", Count = churches.Count(c => c.AverageRating >= 3) },
                    new() { Value = "2", Label = "2+ estrelas", Count = churches.Count(c => c.AverageRating >= 2) }
                }
            });

            return filters;
        }
    }
}
