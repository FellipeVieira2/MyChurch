namespace MyChurch.Domain.Entities
{
    public class FeedPost
    {
        public int Id { get; set; }
        public int MemberId { get; set; }
        public int ChurchId { get; set; }
        public string Content { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }

        public Member Member { get; set; }
        public Church Church { get; set; }
        public ICollection<FeedLike> Likes { get; set; }
        public ICollection<FeedPostImage> Images { get; set; } = new List<FeedPostImage>();
    }
}
