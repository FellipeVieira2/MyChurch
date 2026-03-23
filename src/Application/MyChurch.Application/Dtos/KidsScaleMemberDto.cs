namespace MyChurch.Application.Dtos
{
    public class KidsScaleMemberDto
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public int MemberId { get; set; }
        public string? MemberName { get; set; }
        public string? MemberEmail { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public int Order { get; set; }
        public string? Notes { get; set; }

        public static KidsScaleMemberDto New(Domain.Entities.KidsScaleMember entity)
        {
            return new KidsScaleMemberDto
            {
                Id = entity.Id,
                EventId = entity.EventId,
                MemberId = entity.MemberId,
                MemberName = entity.Member?.Name,
                MemberEmail = entity.Member?.Email,
                RoleName = entity.RoleName,
                Order = entity.Order,
                Notes = entity.Notes
            };
        }
    }
}
