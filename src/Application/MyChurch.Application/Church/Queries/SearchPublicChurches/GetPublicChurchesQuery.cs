using MediatR;
using Microsoft.EntityFrameworkCore;
using MyChurch.Domain.Contracts;
using Mychurch.Common.Utils.Objects;

namespace MyChurch.Application.Church.Queries.SearchPublicChurches
{
    public class GetPublicChurchesQuery : IRequest<PagedResultDto<ChurchPublicListItemDto>>
    {
        public string? Search { get; set; }
         
        public string? City { get; set; }
        public string? State { get; set; }
        public bool? HasServiceToday { get; set; }
        public double? UserLatitude { get; set; }
        public double? UserLongitude { get; set; }
        public double? RadiusKm { get; set; }
        
        // NOVOS FILTROS AVANÇADOS
        public double? MinRating { get; set; } // Filtrar por nota mínima (ex: 4.0+)
        public int? MinReviews { get; set; } // Filtrar por quantidade mínima de avaliações
        public string? SortBy { get; set; } = "relevance"; // relevance, rating, distance, reviews, newest
        public List<string>? Amenities { get; set; } // Ex: ["estacionamento", "acessibilidade", "transmissao_online"]
        public List<string>? Denominations { get; set; } // Filtrar por denominação
        
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class ChurchPublicListItemDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Logo { get; set; }
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public bool HasServiceToday { get; set; }
        public DateTime? NextServiceStartTime { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public double? DistanceKm { get; set; }
        
        // NOVOS CAMPOS DE AVALIAÇÃO
        public double AverageRating { get; set; }
        public int TotalReviews { get; set; }
        public Dictionary<int, int> RatingDistribution { get; set; } = new(); // Ex: {5: 120, 4: 45, 3: 10, 2: 3, 1: 2}
        public List<ReviewSummaryDto>? TopReviews { get; set; } // 2-3 reviews mais úteis
        public List<string>? Photos { get; set; } // Fotos da igreja
        public List<string>? Amenities { get; set; } // Comodidades disponíveis
        public bool IsVerified { get; set; } // Igreja verificada pela plataforma
        public string? PriceLevel { get; set; } // "Gratuito", "Dízimo Sugerido", etc
        public int? CapacityEstimate { get; set; } // Lotação estimada
        public bool HasLiveStream { get; set; } // Transmissão online
        public string? ResponseRate { get; set; } // Taxa de resposta da igreja
        public TimeSpan? AverageResponseTime { get; set; } // Tempo médio de resposta
        public double RelevanceScore { get; set; } // Score de relevância calculado
    }

    public class ReviewSummaryDto
    {
        public int Id { get; set; }
        public string ReviewerName { get; set; } = string.Empty;
        public int Score { get; set; }
        public string Comment { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public int HelpfulCount { get; set; }
        public string? ReviewerPhoto { get; set; }
    }

    public class GetPublicChurchesQueryHandler : IRequestHandler<GetPublicChurchesQuery, PagedResultDto<ChurchPublicListItemDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public GetPublicChurchesQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PagedResultDto<ChurchPublicListItemDto>> Handle(GetPublicChurchesQuery request, CancellationToken cancellationToken)
        {
            var today = DateTime.UtcNow.Date;
            var tomorrow = today.AddDays(1);
            var now = DateTime.UtcNow;

            var query = _unitOfWork.Churchs
                .Query()
                .Include(c => c.Address)
                .AsNoTracking();

            // Filtros existentes
            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim();
                if (!string.IsNullOrWhiteSpace(request.Search))
                {
                    int searchId;
                    bool isInt = int.TryParse(search, out searchId);
                    query = query.Where(c =>
                        c.Name.Contains(search) ||
                        (c.Description != null && c.Description.Contains(search)) ||
                        c.Address.City.Contains(search) ||
                        c.Address.State.Contains(search) ||
                        (isInt && c.Id == searchId)
                    );
                }
            }
            if (!string.IsNullOrWhiteSpace(request.City))
            {
                var city = request.City.Trim();
                query = query.Where(c => c.Address.City == city);
            }
            if (!string.IsNullOrWhiteSpace(request.State))
            {
                var state = request.State.Trim();
                query = query.Where(c => c.Address.State == state);
            }
            if (request.HasServiceToday.HasValue && request.HasServiceToday.Value)
            {
                query = query.Where(c => _unitOfWork.WorshipServices.Query().Any(ws => ws.ChurchId == c.Id && ws.StartTime >= today && ws.StartTime < tomorrow));
            }

            // Pré-filtragem por bounding box se tiver raio e coordenadas
            if (request.UserLatitude.HasValue && request.UserLongitude.HasValue && request.RadiusKm.HasValue)
            {
                const double earthRadiusKm = 6371.0;
                var lat = request.UserLatitude.Value * Math.PI / 180.0;
                var lon = request.UserLongitude.Value * Math.PI / 180.0;
                var angularDistance = request.RadiusKm.Value / earthRadiusKm;
                var minLat = request.UserLatitude.Value - (angularDistance * 180.0 / Math.PI);
                var maxLat = request.UserLatitude.Value + (angularDistance * 180.0 / Math.PI);
                var deltaLon = Math.Asin(Math.Sin(angularDistance) / Math.Cos(lat));
                var minLon = request.UserLongitude.Value - (deltaLon * 180.0 / Math.PI);
                var maxLon = request.UserLongitude.Value + (deltaLon * 180.0 / Math.PI);

                query = query.Where(c => c.Latitude != null && c.Longitude != null && c.Latitude >= minLat && c.Latitude <= maxLat && c.Longitude >= minLon && c.Longitude <= maxLon);
            }

            var total = await query.CountAsync(cancellationToken);

            // Materializa e calcula distância em memória (para futura otimização com PostGIS)
            var churches = await query
                .OrderBy(c => c.Name)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(c => new ChurchPublicListItemDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    Logo = c.LogoFileName,
                    City = c.Address.City,
                    State = c.Address.State,
                    HasServiceToday = _unitOfWork.WorshipServices.Query().Any(ws => ws.ChurchId == c.Id && ws.StartTime >= today && ws.StartTime < tomorrow),
                    NextServiceStartTime = _unitOfWork.WorshipServices.Query()
                        .Where(ws => ws.ChurchId == c.Id && ws.StartTime >= now)
                        .OrderBy(ws => ws.StartTime)
                        .Select(ws => (DateTime?)ws.StartTime)
                        .FirstOrDefault(),
                    Latitude = c.Latitude,
                    Longitude = c.Longitude
                })
                .ToListAsync(cancellationToken);

            // Enriquecer com dados de avaliações
            foreach (var church in churches)
            {
                // Calcular média e total de reviews
                var reviews = await _unitOfWork.Reviews.Query()
                    .Include(r => r.Reviewer)
                    .Where(r => r.EntityId == church.Id && r.EntityType == "Church")
                    .ToListAsync(cancellationToken);

                church.TotalReviews = reviews.Count;
                church.AverageRating = reviews.Any() ? Math.Round(reviews.Average(r => r.Score), 1) : 0;

                // Distribuição de ratings
                church.RatingDistribution = reviews
                    .GroupBy(r => r.Score)
                    .ToDictionary(g => g.Key, g => g.Count());

                // Top 2 reviews mais recentes
                church.TopReviews = reviews
                    .OrderByDescending(r => r.CreatedAt)
                    .Take(2)
                    .Select(r => new ReviewSummaryDto
                    {
                        Id = r.Id,
                        ReviewerName = r.Reviewer?.Name ?? "Anônimo",
                        Score = r.Score,
                        Comment = r.Comment.Length > 150 ? r.Comment.Substring(0, 147) + "..." : r.Comment,
                        CreatedAt = r.CreatedAt,
                        HelpfulCount = 0, // TODO: Implementar sistema de votos úteis
                        ReviewerPhoto = r.Reviewer?.Photo
                    })
                    .ToList();

                // Calcular distância se coordenadas fornecidas
                if (request.UserLatitude.HasValue && request.UserLongitude.HasValue && church.Latitude.HasValue && church.Longitude.HasValue)
                {
                    church.DistanceKm = Haversine(request.UserLatitude.Value, request.UserLongitude.Value, church.Latitude.Value, church.Longitude.Value);
                }

                // Calcular score de relevância
                church.RelevanceScore = CalculateRelevanceScore(church, request.UserLatitude.HasValue);
            }

            // Aplicar filtros de rating
            if (request.MinRating.HasValue)
            {
                churches = churches.Where(c => c.AverageRating >= request.MinRating.Value).ToList();
            }

            if (request.MinReviews.HasValue)
            {
                churches = churches.Where(c => c.TotalReviews >= request.MinReviews.Value).ToList();
            }

            // Filtrar por raio preciso
            if (request.RadiusKm.HasValue && request.UserLatitude.HasValue && request.UserLongitude.HasValue)
            {
                churches = churches.Where(c => c.DistanceKm.HasValue && c.DistanceKm.Value <= request.RadiusKm.Value).ToList();
            }

            // Aplicar ordenação avançada
            churches = request.SortBy?.ToLower() switch
            {
                "rating" => churches.OrderByDescending(c => c.AverageRating).ThenByDescending(c => c.TotalReviews).ToList(),
                "distance" => churches.OrderBy(c => c.DistanceKm ?? double.MaxValue).ToList(),
                "reviews" => churches.OrderByDescending(c => c.TotalReviews).ToList(),
                "newest" => churches.OrderByDescending(c => c.NextServiceStartTime).ToList(),
                "relevance" or _ => churches
                    .OrderByDescending(c => c.RelevanceScore)
                    .ToList()
            };

            return new PagedResultDto<ChurchPublicListItemDto>
            {
                TotalCount = churches.Count, // Total após filtros
                PageNumber = request.Page,
                PageSize = request.PageSize,
                Items = churches
            };
        }

        private static double Haversine(double lat1, double lon1, double lat2, double lon2)
        {
            const double R = 6371.0; // km
            var dLat = ToRad(lat2 - lat1);
            var dLon = ToRad(lon2 - lon1);
            lat1 = ToRad(lat1);
            lat2 = ToRad(lat2);

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2) * Math.Cos(lat1) * Math.Cos(lat2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c;
        }
        
        private static double ToRad(double value) => value * Math.PI / 180;

        private static double CalculateRelevanceScore(ChurchPublicListItemDto church, bool hasUserLocation)
        {
            double score = 0;

            // Peso para avaliação (40%)
            score += church.AverageRating * 8;

            // Peso para quantidade de reviews (30%)
            score += Math.Min(church.TotalReviews / 10.0, 15);

            // Peso para proximidade (30% se localização fornecida)
            if (hasUserLocation && church.DistanceKm.HasValue)
            {
                var distanceScore = Math.Max(0, 15 - (church.DistanceKm.Value * 0.5));
                score += distanceScore;
            }

            // Bônus para igrejas verificadas
            if (church.IsVerified)
                score += 5;

            // Bônus para igrejas com culto hoje
            if (church.HasServiceToday)
                score += 3;

            return Math.Round(score, 2);
        }
    }
}
