using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyChurch.Domain.Entities;

namespace MyChurch.Infrastructure.Mappings
{
    public class GroupMeetingMap : IEntityTypeConfiguration<GroupMeeting>
    {
        public void Configure(EntityTypeBuilder<GroupMeeting> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Topic).IsRequired().HasMaxLength(200);
            builder.Property(x => x.MeetingDate).IsRequired();
            builder.HasOne(x => x.Group)
                .WithMany()
                .HasForeignKey(x => x.GroupId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(x => x.Attendances)
                .WithOne(x => x.GroupMeeting)
                .HasForeignKey(x => x.GroupMeetingId);
            builder.HasMany(x => x.MemberNotes)
                .WithOne(x => x.GroupMeeting)
                .HasForeignKey(x => x.GroupMeetingId);
        }
    }

    public class GroupMeetingAttendanceMap : IEntityTypeConfiguration<GroupMeetingAttendance>
    {
        public void Configure(EntityTypeBuilder<GroupMeetingAttendance> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.IsPresent).IsRequired();
            builder.HasOne(x => x.GroupMeeting)
                .WithMany(x => x.Attendances)
                .HasForeignKey(x => x.GroupMeetingId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

    public class GroupMeetingMemberNoteMap : IEntityTypeConfiguration<GroupMeetingMemberNote>
    {
        public void Configure(EntityTypeBuilder<GroupMeetingMemberNote> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Note).IsRequired().HasMaxLength(2000);
            builder.Property(x => x.CreatedAt).IsRequired();
            builder.HasOne(x => x.GroupMeeting)
                .WithMany(x => x.MemberNotes)
                .HasForeignKey(x => x.GroupMeetingId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
