
using System.Collections.Generic;

namespace MyChurch.Domain.Entities
{
    public class Presentation 
    {
        public int Id { get; set; }
        public int ChurchId { get; set; }
        public int AdminUserId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int CurrentSlideIndex { get; set; } = 0;
        public bool IsLive { get; set; } = false;

        public virtual Church Church { get; set; }
        public virtual Member AdminUser { get; set; }
        public virtual ICollection<Slide> Slides { get; set; }
    }
}
