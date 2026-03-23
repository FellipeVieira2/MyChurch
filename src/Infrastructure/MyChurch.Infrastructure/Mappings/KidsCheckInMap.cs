using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyChurch.Domain.Entities;

namespace MyChurch.Infrastructure.Mappings
{
    public class KidsCheckInMap : IEntityTypeConfiguration<KidsCheckIn>
    {
        public void Configure(EntityTypeBuilder<KidsCheckIn> builder)
        {
            builder.ToTable("kids_check_ins");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();

            builder.Property(x => x.ChildId)
                .HasColumnName("child_id")
                .HasColumnType("integer")
                .IsRequired();

            builder.Property(x => x.ChurchId)
                .HasColumnName("church_id")
                .HasColumnType("int")
                .IsRequired();

            builder.Property(x => x.CheckedInByMemberId)
                .HasColumnName("checked_in_by_member_id")
                .HasColumnType("int")
                .IsRequired();

            builder.Property(x => x.CheckedInAt)
                .HasColumnName("checked_in_at")
                .HasColumnType("timestamp with time zone")
                .IsRequired();

            builder.Property(x => x.EnvironmentName)
                .HasColumnName("environment_name")
                .HasMaxLength(150)
                .IsRequired();

            builder.Property(x => x.PickupToken)
                .HasColumnName("pickup_token")
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(x => x.PickupTokenExpiresAt)
                .HasColumnName("pickup_token_expires_at")
                .HasColumnType("timestamp with time zone")
                .IsRequired();

            builder.Property(x => x.Notes)
                .HasColumnName("notes")
                .HasMaxLength(500);

            builder.Property(x => x.CheckedOutAt)
                .HasColumnName("checked_out_at")
                .HasColumnType("timestamp with time zone");

            builder.Property(x => x.CheckedOutByMemberId)
                .HasColumnName("checked_out_by_member_id")
                .HasColumnType("int");

            builder.Property(x => x.AuthorizedPickupId)
                .HasColumnName("authorized_pickup_id")
                .HasColumnType("integer");

            builder.HasIndex(x => x.PickupToken)
                .IsUnique();

            builder.HasIndex(x => x.ChildId)
                .HasDatabaseName("IX_kids_check_ins_child_active")
                .HasFilter("checked_out_at IS NULL")
                .IsUnique();

            builder.HasIndex(x => new { x.ChurchId, x.CheckedInAt })
                .HasDatabaseName("IX_kids_check_ins_church_checked_in_at");

            builder.HasOne(x => x.Child)
                .WithMany(x => x.KidsCheckIns)
                .HasForeignKey(x => x.ChildId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Church)
                .WithMany()
                .HasForeignKey(x => x.ChurchId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.CheckedInByMember)
                .WithMany()
                .HasForeignKey(x => x.CheckedInByMemberId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.CheckedOutByMember)
                .WithMany()
                .HasForeignKey(x => x.CheckedOutByMemberId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(x => x.AuthorizedPickup)
                .WithMany()
                .HasForeignKey(x => x.AuthorizedPickupId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
