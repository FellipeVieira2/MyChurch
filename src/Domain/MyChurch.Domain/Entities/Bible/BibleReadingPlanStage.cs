using System;

namespace MyChurch.Domain.Entities.Bible
{
    public class BibleReadingPlanStage
    {
        public int Id { get; set; }
        public int BibleReadingPlanId { get; set; }
        public BibleReadingPlan BibleReadingPlan { get; set; }

        public int Order { get; set; } // Ex: Dia 1, Dia 2
        public string Description { get; set; } // Ex: "Gênesis 1-3"
        public string VerseReferences { get; set; } // Ex: "Gn 1:1-3:24; Sl 1:1-6"
        
        public DateTime Created { get; set; } = DateTime.UtcNow;
        public DateTime? Updated { get; set; }
    }
}