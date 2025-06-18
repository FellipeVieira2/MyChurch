namespace MyChurch.Application.Dtos
{
    public class GroupDetailsDto
    {
        public int GroupId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public int LeaderId { get; set; }
        public bool AcceptsNewMembers { get; set; }
        public string? CoverImageUrl { get; set; }
    }
}
