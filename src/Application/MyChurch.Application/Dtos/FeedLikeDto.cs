using System;
using MyChurch.Domain.Entities;

namespace MyChurch.Application.Dtos
{
    public class FeedLikeDto
    {
        public int Id { get; set; }
        public int FeedPostId { get; set; }
        public int MemberId { get; set; }
        public DateTime Created { get; set; }
        public MemberDto Member { get; set; }

        public static FeedLikeDto New(FeedLike like)
        {
            return new FeedLikeDto
            {
                Id = like.Id,
                FeedPostId = like.FeedPostId,
                MemberId = like.MemberId,
                Created = like.Created,
                Member = like.Member != null ? MemberDto.New(like.Member) : null
            };
        }
    }
}
