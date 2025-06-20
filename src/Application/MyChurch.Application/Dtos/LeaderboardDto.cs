namespace MyChurch.Application.Dtos
{
    public class LeaderboardDto
    {
        public int MemberId { get; set; }
        public string MemberName { get; set; }
        public int TotalFaithPoints { get; set; }
        public int DevotionalStreak { get; set; }
    }
}
