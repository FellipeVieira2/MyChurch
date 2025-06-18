using MyChurch.Domain.Enum;
using System;

namespace MyChurch.Domain.Entities
{
    public class FamilyInvitation
    {
        public int Id { get; set; }
        public int ChurchId { get; set; }
        public int InviterMemberId { get; set; }
        public int InvitedMemberId { get; set; }
        public InvitationStatus Status { get; set; } // Enum
        public DateTime CreatedAt { get; set; }
        public DateTime? RespondedAt { get; set; }
    }
}
