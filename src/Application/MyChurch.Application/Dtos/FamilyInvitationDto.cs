using System;

namespace MyChurch.Application.Dtos
{
    public class FamilyInvitationDto
    {
        public int InvitationId { get; set; }
        public int InviterMemberId { get; set; }
        public int InvitedMemberId { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
