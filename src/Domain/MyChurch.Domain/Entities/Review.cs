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
        
        // Relacionamento opcional
        public Member? Reviewer { get; set; }
        
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
    }
}
