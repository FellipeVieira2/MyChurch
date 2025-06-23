using System;
using System.Collections.Generic;

namespace MyChurch.Domain.Entities.Bible
{
    public class BibleReadingPlan
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int DurationInDays { get; set; }
        public bool IsDefault { get; set; }
        public bool IsPublic { get; set; }
        public int? ChurchId { get; set; } // Nullable, para planos padrão ou públicos
        public Church Church { get; set; }
        
        public ICollection<BibleReadingPlanStage> BibleReadingPlanStages { get; set; }
        public ICollection<MemberBibleReadingProgress> MemberBibleReadingProgresses { get; set; }
        
        public DateTime Created { get; set; } = DateTime.UtcNow;
        public DateTime? Updated { get; set; }
    }
}