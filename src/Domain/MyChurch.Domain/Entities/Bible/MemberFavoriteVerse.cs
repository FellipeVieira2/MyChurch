using System;

namespace MyChurch.Domain.Entities.Bible
{
    public class MemberFavoriteVerse
    {
        public int Id { get; set; }
        public int MemberId { get; set; }
        public int VersionId { get; set; }
        public string BookName { get; set; }
        public int ChapterNumber { get; set; }
        public int VerseNumber { get; set; }
        public DateTime DateFavorited { get; set; }

        // Navigation properties
        public Member Member { get; set; }
    }
}