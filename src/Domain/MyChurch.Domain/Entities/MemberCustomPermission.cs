using MyChurch.Domain.Enum;

namespace MyChurch.Domain.Entities
{
    /// <summary>
    /// Permissões customizadas para membros específicos
    /// Sobrescreve as permissões padrão da role
    /// </summary>
    public class MemberCustomPermission
    {
        public int Id { get; set; }
        
        /// <summary>
        /// Membro que recebe a permissão customizada
        /// </summary>
        public int MemberId { get; set; }
        public Member Member { get; set; }
        
        /// <summary>
        /// Permissão específica
        /// </summary>
        public Permission Permission { get; set; }
        
        /// <summary>
        /// true = conceder permissão, false = revogar permissão
        /// </summary>
        public bool IsGranted { get; set; }
        
        /// <summary>
        /// Membro que concedeu/revogou a permissão
        /// </summary>
        public int? GrantedByMemberId { get; set; }
        public Member? GrantedByMember { get; set; }
        
        /// <summary>
        /// Motivo da concessão/revogação
        /// </summary>
        public string? Reason { get; set; }
        
        /// <summary>
        /// Data de expiração da permissão (null = permanente)
        /// </summary>
        public DateTime? ExpiresAt { get; set; }
        
        public DateTime Created { get; set; } = DateTime.UtcNow;
        public DateTime? Updated { get; set; }
        
        /// <summary>
        /// Verifica se a permissão está ativa
        /// </summary>
        public bool IsActive()
        {
            return !ExpiresAt.HasValue || ExpiresAt.Value > DateTime.UtcNow;
        }
    }
}
