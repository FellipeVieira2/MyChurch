
namespace MyChurch.Application.Dtos
{
    public class EventDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public int ChurchId { get; set; }
        public ChurchDto Church { get; set; }
        public ICollection<MemberDto> Participants { get; set; } = new List<MemberDto>();
    }
}
