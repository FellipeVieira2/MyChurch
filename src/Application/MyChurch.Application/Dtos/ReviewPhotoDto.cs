namespace MyChurch.Application.Dtos
{
    /// <summary>
    /// DTO para fotos de reviews
    /// </summary>
    public class ReviewPhotoDto
    {
        public int Id { get; set; }
        public int ReviewId { get; set; }
        public string Url { get; set; } = string.Empty;
        public string? Caption { get; set; }
        public int DisplayOrder { get; set; }
        public DateTime UploadedAt { get; set; }
    }

    /// <summary>
    /// DTO para respostas de reviews
    /// </summary>
    public class ReviewResponseDto
    {
        public int Id { get; set; }
        public int ReviewId { get; set; }
        public string Response { get; set; } = string.Empty;
        public int ResponderId { get; set; }
        public string RespondedBy { get; set; } = string.Empty;
        public DateTime RespondedAt { get; set; }
    }
}
