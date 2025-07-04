using System.Collections.Generic;

namespace MyChurch.Application.Dtos
{
    public class PresentationDto
    {
        public int Id { get; set; }
        public int ChurchId { get; set; }
        public int AdminUserId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int CurrentSlideIndex { get; set; }
        public bool IsLive { get; set; }
        public List<SlideDto> Slides { get; set; }
    }
}