using MyChurch.Domain.Enum;

namespace MyChurch.Application.Dtos
{
    public class SlideDto
    {
        public int Id { get; set; }
        public int OrderIndex { get; set; }
        public SlideContentType ContentType { get; set; }
        public string ContentReferenceJson { get; set; }
        public string CachedDisplayText { get; set; }
        public string CachedMediaUrl { get; set; }
    }
}