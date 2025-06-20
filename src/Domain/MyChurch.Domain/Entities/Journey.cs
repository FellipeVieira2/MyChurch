
using System.Collections.Generic;

namespace MyChurch.Domain.Entities
{
    public class Journey
    {
        public int Id { get; set; }
        public int ChurchId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string? IconUrl { get; set; }
        public bool IsActive { get; set; }
        public bool IsDefault { get; set; }
        public virtual ICollection<JourneyStage> Stages { get; set; } = new List<JourneyStage>();
    }
}
