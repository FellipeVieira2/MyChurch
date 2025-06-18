namespace MyChurch.Application.Dtos
{
    public class GroupListItemDto
    {
        public int GroupId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public int LeaderId { get; set; }
    }
}
