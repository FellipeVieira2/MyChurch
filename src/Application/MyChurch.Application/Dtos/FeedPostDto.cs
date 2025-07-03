using System;
using System.Collections.Generic;
using System.Linq;
using MyChurch.Domain.Entities;

namespace MyChurch.Application.Dtos
{
    public class FeedPostDto
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public int MemberId { get; set; }
        public int ChurchId { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
        public MemberDto Member { get; set; }
        public int LikesCount { get; set; }
        public bool LikedForMember { get; set; } = false;
        public List<FeedPostImageDto> FeedPostImages { get; set; } = new List<FeedPostImageDto>();
        public static FeedPostDto New(FeedPost post)
        {
            return new FeedPostDto
            {
                Id = post.Id,
                Content = post.Content,
                MemberId = post.MemberId,
                ChurchId = post.ChurchId,
                Created = post.Created,
                Updated = post.Updated,
                Member = post.Member != null ? MemberDto.New(post.Member) : null,
                LikesCount = post.Likes?.Count ?? 0,
                FeedPostImages = post.Images != null ? post.Images.Select(FeedPostImageDto.New).ToList() : new List<FeedPostImageDto>()
            };
        }
    }
}
