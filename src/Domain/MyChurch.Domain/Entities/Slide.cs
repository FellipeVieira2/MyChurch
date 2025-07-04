using MyChurch.Domain.Enum;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyChurch.Domain.Entities
{
    public class Slide 
    {
        public int Id { get; set; }
        public int PresentationId { get; set; }
        public int OrderIndex { get; set; }

        public SlideContentType ContentType { get; set; }

        [Column(TypeName = "jsonb")]
        public string ContentReferenceJson { get; set; }

        public string CachedDisplayText { get; set; }

        public string CachedMediaUrl { get; set; }

        public virtual Presentation Presentation { get; set; }
    }
}
