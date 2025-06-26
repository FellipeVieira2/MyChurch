using System;

namespace MyChurch.Application.Dtos
{
    public record MemberEngagementRankingDto(int Id, string Name, string? PictureUrl, int EngagementScore);
    public record CommunityEngagementScoreDto(int AverageScore);
}
