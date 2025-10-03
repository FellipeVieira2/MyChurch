using System;

namespace MyChurch.Domain.Entities
{
    /// <summary>
    /// Representa uma foto anexada a uma review
    /// </summary>
    public class ReviewPhoto
    {
        public int Id { get; set; }
        
        /// <summary>
        /// ID da review que contém a foto
        /// </summary>
        public int ReviewId { get; set; }
        
        /// <summary>
        /// URL da foto no S3
        /// </summary>
        public string PhotoUrl { get; set; } = string.Empty;
        
        /// <summary>
        /// Nome original do arquivo
        /// </summary>
        public string? OriginalFileName { get; set; }
        
        /// <summary>
        /// Descrição/legenda da foto
        /// </summary>
        public string? Caption { get; set; }
        
        /// <summary>
        /// Ordem de exibição (para galeria)
        /// </summary>
        public int DisplayOrder { get; set; } = 0;
        
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
        
        // Relacionamentos
        public Review Review { get; set; } = null!;
    }
}
