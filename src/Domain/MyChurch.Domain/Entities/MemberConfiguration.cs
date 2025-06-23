using MyChurch.Domain.Entities.Bible;
using System;

namespace MyChurch.Domain.Entities
{
    public class MemberConfiguration
    {
        public int Id { get; set; }
        public int MemberId { get; set; }
        public int? PreferredBibleVersionId { get; set; }
        public string ThemePreference { get; set; } = "Light"; // Light, Dark, System
        public string FontSize { get; set; } = "Medium"; // Small, Medium, Large
        public bool EnableNotifications { get; set; } = true;
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public Member Member { get; set; }
        public Domain.Entities.Bible.Version PreferredBibleVersion { get; set; }
    }
}