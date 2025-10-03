using System;

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
    }
}
