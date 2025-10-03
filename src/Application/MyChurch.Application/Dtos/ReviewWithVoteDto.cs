namespace MyChurch.Application.Dtos
{
    /// <summary>
    /// DTO de Review com informações de voto do usuário atual
    /// </summary>
    public class ReviewWithVoteDto
    {
        public int Id { get; set; }
        public int EntityId { get; set; }
        public string EntityType { get; set; } = string.Empty;
        public int Score { get; set; }
        public string? Comment { get; set; }
        public bool IsVerified { get; set; }
        public DateTime CreatedAt { get; set; }
        public string MemberName { get; set; } = string.Empty;
        public int MemberId { get; set; }
        public int HelpfulCount { get; set; }
        public int NotHelpfulCount { get; set; }
        public List<ReviewPhotoDto> Photos { get; set; } = new();
        public ReviewResponseDto? Response { get; set; }
        
        /// <summary>
        /// Indica se o usuário atual já votou nesta review
        /// </summary>
        public bool HasCurrentMemberVoted { get; set; }
        
        /// <summary>
        /// Se votou, indica se foi voto positivo (true) ou negativo (false)
        /// </summary>
        public bool? CurrentMemberVoteIsHelpful { get; set; }
    }
}
