using System;
using System.Collections.Generic;
using System.Linq;

namespace MyChurch.Domain.Entities
{
    public class Review
    {
        public int Id { get; set; }
        public int EntityId { get; set; } // Ex: ChurchId
        public string EntityType { get; set; } = string.Empty; // Ex: "Church"
        public int ReviewerId { get; set; }
        public int Score { get; set; } // 1-5
        public string Comment { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        
        // Verificação de presença
        public bool IsVerified { get; set; } = false; // Review verificada por check-in
        public int? WorshipPresenceId { get; set; } // ID da presença que valida a review
        public DateTime? VerifiedAt { get; set; } // Quando foi verificada
        
        // Relacionamento opcional
        public Member? Reviewer { get; set; }
        public WorshipPresence? WorshipPresence { get; set; }
        
        // Votos úteis/não úteis
        public ICollection<ReviewVote> Votes { get; set; } = new List<ReviewVote>();
        
        /// <summary>
        /// Calcula total de votos úteis (upvotes)
        /// </summary>
        public int GetHelpfulVotesCount()
        {
            return Votes?.Count(v => v.IsHelpful) ?? 0;
        }
        
        /// <summary>
        /// Calcula total de votos não úteis (downvotes)
        /// </summary>
        public int GetNotHelpfulVotesCount()
        {
            return Votes?.Count(v => !v.IsHelpful) ?? 0;
        }
        
        /// <summary>
        /// Score de utilidade (upvotes - downvotes)
        /// </summary>
        public int GetHelpfulnessScore()
        {
            return GetHelpfulVotesCount() - GetNotHelpfulVotesCount();
        }
        
        /// <summary>
        /// Marca review como verificada por presença
        /// </summary>
        public void MarkAsVerified(int worshipPresenceId)
        {
            IsVerified = true;
            WorshipPresenceId = worshipPresenceId;
            VerifiedAt = DateTime.UtcNow;
        }
        
        /// <summary>
        /// Remove verificação da review
        /// </summary>
        public void RemoveVerification()
        {
            IsVerified = false;
            WorshipPresenceId = null;
            VerifiedAt = null;
        }
    }
}
