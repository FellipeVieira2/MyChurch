using System;

namespace MyChurch.Domain.Entities
{
    /// <summary>
    /// Representa um voto útil/não útil em uma avaliação
    /// </summary>
    public class ReviewVote
    {
        public int Id { get; set; }
        
        /// <summary>
        /// ID da review que recebeu o voto
        /// </summary>
        public int ReviewId { get; set; }
        
        /// <summary>
        /// ID do membro que votou
        /// </summary>
        public int MemberId { get; set; }
        
        /// <summary>
        /// True = Útil (upvote), False = Não útil (downvote)
        /// </summary>
        public bool IsHelpful { get; set; }
        
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        
        // Relacionamentos
        public Review Review { get; set; } = null!;
        public Member Member { get; set; } = null!;
        
        /// <summary>
        /// Atualiza o tipo de voto (de útil para não útil ou vice-versa)
        /// </summary>
        public void ToggleVote()
        {
            IsHelpful = !IsHelpful;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
