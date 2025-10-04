using MyChurch.Domain.Entities;

namespace MyChurch.Application.Dtos
{
    /// <summary>
    /// DTO para card de igreja na busca (similar ao TripAdvisor)
    /// </summary>
    public class ChurchSearchCardDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? CoverPhoto { get; set; }
        public string? Logo { get; set; }
        public double? AverageRating { get; set; }
        public int TotalReviews { get; set; }
        public double DistanceKm { get; set; }
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string? Denomination { get; set; }
        public List<string> TopReviewHighlights { get; set; } = new();
        public WorshipScheduleSummaryDto? NextWorship { get; set; }
        public string PriceRange { get; set; } = "Gratuito"; // "Gratuito", "Dízimo opcional", etc
        public bool IsVerified { get; set; }
        public List<string> Badges { get; set; } = new();
        public List<string> Features { get; set; } = new(); // Ex: "Estacionamento", "Acessível", "Transmissão ao vivo"
        
        // Métricas adicionais
        public int TotalPhotos { get; set; }
        public int TotalVisits { get; set; }
        
        // Coordenadas (para exibir no mapa)
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
    }
    
    /// <summary>
    /// Resumo do próximo culto
    /// </summary>
    public class WorshipScheduleSummaryDto
    {
        public int Id { get; set; }
        public DateTime DateTime { get; set; }
        public string DayOfWeek { get; set; } = string.Empty;
        public string Time { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty; // Ex: "Culto Dominical", "Estudo Bíblico"
    }
    
    /// <summary>
    /// Resultado da busca de igrejas
    /// </summary>
    public class ChurchSearchResultDto
    {
        public List<ChurchSearchCardDto> Churches { get; set; } = new();
        public int TotalResults { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public bool HasNextPage { get; set; }
        public bool HasPreviousPage { get; set; }
        
        // Mapa
        public MapBoundsDto? MapBounds { get; set; }
        
        // Sugestões e filtros
        public SearchSuggestionsDto? Suggestions { get; set; }
        public List<FilterOptionDto> AvailableFilters { get; set; } = new();
    }
    
    /// <summary>
    /// Limites do mapa para exibição
    /// </summary>
    public class MapBoundsDto
    {
        public double MinLatitude { get; set; }
        public double MaxLatitude { get; set; }
        public double MinLongitude { get; set; }
        public double MaxLongitude { get; set; }
        public double CenterLatitude { get; set; }
        public double CenterLongitude { get; set; }
    }
    
    /// <summary>
    /// Sugestões de busca
    /// </summary>
    public class SearchSuggestionsDto
    {
        public List<string> NearbyDenominations { get; set; } = new();
        public List<string> PopularSearches { get; set; } = new();
        public string? DidYouMean { get; set; }
    }
    
    /// <summary>
    /// Opção de filtro disponível
    /// </summary>
    public class FilterOptionDto
    {
        public string Key { get; set; } = string.Empty;
        public string Label { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty; // "checkbox", "select", "range"
        public List<FilterValueDto> Values { get; set; } = new();
    }
    
    /// <summary>
    /// Valor de filtro
    /// </summary>
    public class FilterValueDto
    {
        public string Value { get; set; } = string.Empty;
        public string Label { get; set; } = string.Empty;
        public int Count { get; set; } // Quantidade de igrejas com esse filtro
    }
}
