using MyChurch.Domain.Enum;

namespace MyChurch.Domain.Entities
{
    /// <summary>
    /// Relacionamento entre roles e permissões
    /// Define quais permissões cada role possui por padrão
    /// </summary>
    public class RolePermission
    {
        public int Id { get; set; }
        
        /// <summary>
        /// Role do sistema (Admin, Member, Minister, etc)
        /// </summary>
        public UserRole Role { get; set; }
        
        /// <summary>
        /// Permissão específica
        /// </summary>
        public Permission Permission { get; set; }
        
        /// <summary>
        /// Indica se essa permissão está ativa para esse role
        /// </summary>
        public bool IsActive { get; set; } = true;
        
        /// <summary>
        /// ID da igreja (null = permissão global da plataforma)
        /// </summary>
        public int? ChurchId { get; set; }
        public Church? Church { get; set; }
        
        public DateTime Created { get; set; } = DateTime.UtcNow;
        public DateTime? Updated { get; set; }
    }
}
