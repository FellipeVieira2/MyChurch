namespace MyChurch.Application.Dtos
{
    public class WorshipScaleMemberDto
    {
        public int Id { get; set; }
        public int WorshipServiceId { get; set; }
        public int MemberId { get; set; }
        public string? MemberName { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public int Order { get; set; }
        public string? Notes { get; set; }

        public static WorshipScaleMemberDto New(Domain.Entities.WorshipScaleMember entity)
        {
            return new WorshipScaleMemberDto
            {
                Id = entity.Id,
                WorshipServiceId = entity.WorshipServiceId,
                MemberId = entity.MemberId,
                MemberName = entity.Member?.Name,
                RoleName = entity.RoleName,
                Order = entity.Order,
                Notes = entity.Notes
            };
        }
    }
}
