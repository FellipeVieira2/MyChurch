using MyChurch.Domain.Entities;

namespace MyChurch.Application.Dtos
{
    /// <summary>
    /// DTO para foto da igreja
    /// </summary>
    public class ChurchPhotoDto
    {
        public int Id { get; set; }
        public int ChurchId { get; set; }
        public string ChurchName { get; set; } = string.Empty;
        
        // Quem enviou
        public int? UploadedByMemberId { get; set; }
        public string? UploadedByMemberName { get; set; }
        public int? UploadedByVisitorId { get; set; }
        public string? UploadedByVisitorName { get; set; }
        
        // Dados da foto
        public string PhotoUrl { get; set; } = string.Empty;
        public string? Caption { get; set; }
        public string Category { get; set; } = string.Empty;
        public string CategoryDisplay { get; set; } = string.Empty;
        
        // Metadados
        public DateTime UploadedAt { get; set; }
        public int Likes { get; set; }
        
        // Status
        public bool IsApproved { get; set; }
        public bool IsRejected { get; set; }
        public bool IsFeatured { get; set; }
        public string? RejectionReason { get; set; }
        
        // Likes do usuário
        public bool HasUserLiked { get; set; }
        
        public static ChurchPhotoDto New(Domain.Entities.ChurchPhoto photo, bool hasUserLiked = false)
        {
            return new ChurchPhotoDto
            {
                Id = photo.Id,
                ChurchId = photo.ChurchId,
                ChurchName = photo.Church?.Name ?? string.Empty,
                UploadedByMemberId = photo.UploadedByMemberId,
                UploadedByMemberName = photo.UploadedBy?.Name,
                UploadedByVisitorId = photo.UploadedByVisitorId,
                UploadedByVisitorName = photo.UploadedByVisitor?.Name,
                PhotoUrl = photo.PhotoUrl,
                Caption = photo.Caption,
                Category = photo.Category.ToString(),
                CategoryDisplay = GetCategoryDisplay(photo.Category),
                UploadedAt = photo.UploadedAt,
                Likes = photo.Likes,
                IsApproved = photo.IsApproved,
                IsRejected = photo.IsRejected,
                IsFeatured = photo.IsFeatured,
                RejectionReason = photo.RejectionReason,
                HasUserLiked = hasUserLiked
            };
        }
        
        private static string GetCategoryDisplay(PhotoCategory category)
        {
            return category switch
            {
                PhotoCategory.Exterior => "Fachada",
                PhotoCategory.Interior => "Interior",
                PhotoCategory.Worship => "Culto",
                PhotoCategory.Events => "Eventos",
                PhotoCategory.Community => "Comunidade",
                PhotoCategory.Facilities => "Instalações",
                PhotoCategory.ChildMinistry => "Ministério Infantil",
                PhotoCategory.YouthMinistry => "Ministério Jovem",
                PhotoCategory.Menu => "Programação",
                _ => "Outros"
            };
        }
    }
    
    /// <summary>
    /// DTO para galeria de fotos
    /// </summary>
    public class ChurchPhotoGalleryDto
    {
        public int ChurchId { get; set; }
        public string ChurchName { get; set; } = string.Empty;
        public int TotalPhotos { get; set; }
        public List<ChurchPhotoDto> Photos { get; set; } = new();
        public List<PhotoCategoryCountDto> CategoriesCount { get; set; } = new();
        
        // Paginação
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public bool HasNextPage { get; set; }
        public bool HasPreviousPage { get; set; }
    }
    
    /// <summary>
    /// Contagem de fotos por categoria
    /// </summary>
    public class PhotoCategoryCountDto
    {
        public string Category { get; set; } = string.Empty;
        public string CategoryDisplay { get; set; } = string.Empty;
        public int Count { get; set; }
    }
}
