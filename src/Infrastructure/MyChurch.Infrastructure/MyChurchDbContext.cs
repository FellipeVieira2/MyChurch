using Microsoft.EntityFrameworkCore;
using MyChurch.Domain.Entities;
using MyChurch.Domain.Entities.Bible;

namespace MyChurch.Infrastructure
{
    public class MyChurchDbContext : DbContext
    {
        public MyChurchDbContext(DbContextOptions<MyChurchDbContext> options) : base(options) { }

        public DbSet<BibleReadingPlan> BibleReadingPlans { get; set; }
        public DbSet<BibleReadingPlanStage> BibleReadingPlanStages { get; set; }
        public DbSet<MemberBibleReadingProgress> MemberBibleReadingProgresses { get; set; }
        public DbSet<EngagementEvent> EngagementEvents { get; set; } // Novo DbSet
        public DbSet<GroupMeeting> GroupMeetings { get; set; }
        public DbSet<GroupMeetingAttendance> GroupMeetingAttendances { get; set; }
        public DbSet<GroupMeetingMemberNote> GroupMeetingMemberNotes { get; set; }
        public DbSet<MemberBibleReadingAssignment> MemberBibleReadingAssignments { get; set; }
        public DbSet<UserActionHistory> UserActionHistories { get; set; }
        public DbSet<Presentation> Presentations { get; set; }
        public DbSet<Slide> Slides { get; set; }
        public DbSet<ImportedHymn> ImportedHymns { get; set; }
        public DbSet<ImportedHymnStanza> ImportedHymnStanzas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasPostgresExtension("unaccent");
            modelBuilder.HasPostgresExtension("uuid-ossp");
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(MyChurchDbContext).Assembly);
        }
        [DbFunction("unaccent")]
        public string Unaccent()
        {
            throw new NotSupportedException();
        }
    }
}
