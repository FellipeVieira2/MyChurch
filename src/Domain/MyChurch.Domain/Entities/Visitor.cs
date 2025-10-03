using MyChurch.Domain.Enum;

namespace MyChurch.Domain.Entities
{
    public class Visitor
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? AsaasCustomerId { get; set; } // identificador no Asaas
        public int Score { get; set; } = 0; // engajamento
        public VisitorStatus? Status { get; set; } // estágio no funil
        public DateTime? LastVisitAt { get; set; }
        public bool NeedsFollowUp { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public ICollection<VisitorStatusHistory> StatusHistory { get; set; } = new List<VisitorStatusHistory>();
    }
}
