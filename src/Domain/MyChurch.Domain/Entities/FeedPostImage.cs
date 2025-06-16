using System;
using System.Collections.Generic;

namespace MyChurch.Domain.Entities
{
    public class FeedPostImage
    {
        public int Id { get; set; }
        public int FeedPostId { get; set; }
        public string FileName { get; set; }
        public DateTime Created { get; set; } = DateTime.UtcNow;

        public FeedPost FeedPost { get; set; }
    }
}
