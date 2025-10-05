namespace MyChurch.Domain.Entities
{
    /// <summary>
    /// Relação entre Ministério e Membro (muitos-para-muitos)
    /// Um membro pode participar de vários ministérios
    /// </summary>
    public class MinistryMember
    {
        public int Id { get; set; }
        
        /// <summary>
        /// ID do ministério
        /// </summary>
        public int MinistryId { get; set; }
        public Ministry Ministry { get; set; } = null!;
        
        /// <summary>
        /// ID do membro
        /// </summary>
        public int MemberId { get; set; }
        public Member Member { get; set; } = null!;
        
        /// <summary>
        /// Função/cargo do membro no ministério (ex: Vocal, Instrumentista, Coordenador)
        /// </summary>
        public string? Role { get; set; }
        
        /// <summary>
        /// Data de entrada no ministério
        /// </summary>
        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
        
        /// <summary>
        /// Observações sobre a participação do membro
        /// </summary>
        public string? Notes { get; set; }
        
        /// <summary>
        /// Se o membro está ativo no ministério
        /// </summary>
        public bool IsActive { get; set; } = true;
    }
}
