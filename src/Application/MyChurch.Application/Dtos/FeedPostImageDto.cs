using System;

namespace MyChurch.Application.Dtos
{
    public class FeedPostImageDto
    {
        public int Id { get; set; }
        public int FeedPostId { get; set; }
        public string FileName { get; set; }
        public DateTime Created { get; set; }

        public static FeedPostImageDto New(Domain.Entities.FeedPostImage img)
        {
            return new FeedPostImageDto
            {
                Id = img.Id,
                FeedPostId = img.FeedPostId,
                FileName = img.FileName,
                Created = img.Created
            };
        }
    }
}
