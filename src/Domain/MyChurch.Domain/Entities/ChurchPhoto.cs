namespace MyChurch.Domain.Entities
{
    /// <summary>
    /// Fotos da igreja enviadas pela comunidade (membros e visitantes)
    /// Similar ao sistema de fotos do TripAdvisor
    /// </summary>
    public class ChurchPhoto
    {
        public int Id { get; set; }
        
        // Relacionamento com Igreja
        public int ChurchId { get; set; }
        public virtual Church Church { get; set; }
        
        // Quem enviou (pode ser membro ou visitante)
        public int? UploadedByMemberId { get; set; }
        public virtual Member? UploadedBy { get; set; }
        
        public int? UploadedByVisitorId { get; set; }
        public virtual Visitor? UploadedByVisitor { get; set; }
        
        // Dados da foto
        public string PhotoUrl { get; set; } = string.Empty;
        public string? OriginalFileName { get; set; }
        public string? Caption { get; set; }
        public PhotoCategory Category { get; set; }
        
        // Metadados
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
        public int Likes { get; set; } = 0;
        
        // Moderação
        public bool IsApproved { get; set; } = false; // Precisa aprovação de admin
        public bool IsRejected { get; set; } = false;
        public int? ApprovedByAdminId { get; set; }
        public virtual Member? ApprovedByAdmin { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public string? RejectionReason { get; set; }
        
        // Destaque
        public bool IsFeatured { get; set; } = false; // Foto em destaque
        public int DisplayOrder { get; set; } = 0;
        
        // Likes
        public virtual ICollection<ChurchPhotoLike> PhotoLikes { get; set; } = new List<ChurchPhotoLike>();
        
        /// <summary>
        /// Aprova a foto
        /// </summary>
        public void Approve(int adminId)
        {
            IsApproved = true;
            IsRejected = false;
            ApprovedByAdminId = adminId;
            ApprovedAt = DateTime.UtcNow;
            RejectionReason = null;
        }
        
        /// <summary>
        /// Rejeita a foto
        /// </summary>
        public void Reject(int adminId, string reason)
        {
            IsApproved = false;
            IsRejected = true;
            ApprovedByAdminId = adminId;
            ApprovedAt = DateTime.UtcNow;
            RejectionReason = reason;
        }
        
        /// <summary>
        /// Marca como destaque
        /// </summary>
        public void SetAsFeatured(int displayOrder)
        {
            IsFeatured = true;
            DisplayOrder = displayOrder;
        }
        
        /// <summary>
        /// Incrementa likes
        /// </summary>
        public void IncrementLikes()
        {
            Likes++;
        }
        
        /// <summary>
        /// Decrementa likes
        /// </summary>
        public void DecrementLikes()
        {
            if (Likes > 0)
                Likes--;
        }
    }
    
    /// <summary>
    /// Categorias de fotos da igreja
    /// </summary>
    public enum PhotoCategory
    {
        Exterior = 1,       // Fachada da igreja
        Interior = 2,       // Interior do templo
        Worship = 3,        // Durante o culto
        Events = 4,         // Eventos especiais
        Community = 5,      // Comunidade/pessoas
        Facilities = 6,     // Instalações (banheiro, estacionamento, etc)
        ChildMinistry = 7,  // Ministério infantil
        YouthMinistry = 8,  // Ministério jovem
        Menu = 9,           // "Cardápio" de atividades/programação
        Other = 10          // Outras
    }
    
    /// <summary>
    /// Likes em fotos da igreja
    /// </summary>
    public class ChurchPhotoLike
    {
        public int Id { get; set; }
        
        public int ChurchPhotoId { get; set; }
        public virtual ChurchPhoto ChurchPhoto { get; set; }
        
        public int? MemberId { get; set; }
        public virtual Member? Member { get; set; }
        
        public int? VisitorId { get; set; }
        public virtual Visitor? Visitor { get; set; }
        
        public DateTime LikedAt { get; set; } = DateTime.UtcNow;
    }
}
