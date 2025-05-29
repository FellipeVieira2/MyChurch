namespace MyChurch.Domain.Entities
{
    public class FeedLike
    {
        public int Id { get; set; }
        public int FeedPostId { get; set; }
        public int MemberId { get; set; }
        public DateTime Created { get; set; }

        public FeedPost FeedPost { get; set; }
        public Member Member { get; set; }
    }
}
