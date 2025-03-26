using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyChurch.Domain.Entities;

namespace MyChurch.Infrastructure.Mappings
{
    public class MemberMap : IEntityTypeConfiguration<Member>
    {
        public void Configure(EntityTypeBuilder<Member> builder)
        {
            builder.ToTable("member", "postgres");

            builder.HasKey(m => m.Id);

            builder
                .Property(m => m.Id)
                .HasColumnName("id")
                .HasColumnType("int")
                .ValueGeneratedOnAdd()
                .IsRequired();

            builder
                .Property(m => m.Name)
                .HasColumnName("name")
                .HasColumnType("varchar(200)")
                .IsRequired();

            builder
                .Property(m => m.Email)
                .HasColumnName("email")
                .HasColumnType("varchar(200)")
                .IsRequired(false);

            builder
                .Property(m => m.Document)
                .HasColumnName("document")
                .HasColumnType("varchar(50)")
                .IsRequired(false);

            builder
                .Property(m => m.Photo)
                .HasColumnName("photo")
                .HasColumnType("varchar(500)")
                .IsRequired(false);

            builder
                .Property(m => m.PasswordHash)
                .HasColumnName("password_hash")
                .HasColumnType("varchar(500)")
                .IsRequired(false);

            builder
                .Property(m => m.Password)
                .HasColumnName("password")
                .HasColumnType("varchar(500)")
                .IsRequired(false);

            builder
                .Property(m => m.Phone)
                .HasColumnName("phone")
                .HasColumnType("varchar(20)")
                .IsRequired(false);

            builder
                .Property(m => m.BirthDate)
                .HasColumnName("birth_date")
                .HasColumnType("date")
                .IsRequired();

            builder
                .Property(m => m.IsBaptized)
                .HasColumnName("is_baptized")
                .HasColumnType("boolean")
                .HasDefaultValue(false)
                .IsRequired();

            builder
                .Property(m => m.BaptizedDate)
                .HasColumnName("baptized_date")
                .HasColumnType("date")
                .IsRequired(false);

            builder
                .Property(m => m.IsTither)
                .HasColumnName("is_tither")
                .HasColumnType("boolean")
                .IsRequired();

            builder
                .Property(m => m.ChurchId)
                .HasColumnName("church_id")
                .HasColumnType("int")
                .IsRequired();

            builder
                .Property(m => m.Role)
                .HasColumnName("role")
                .HasColumnType("int")
                .IsRequired();

            builder
                .Property(m => m.Created)
                .HasColumnName("created")
                .HasColumnType("timestamp")
                .IsRequired();

            builder
                .Property(m => m.Updated)
                .HasColumnName("updated")
                .HasColumnType("timestamp")
                .IsRequired(false);

            builder
                .HasOne(m => m.Church)
                .WithMany(c => c.Members)
                .HasForeignKey(m => m.ChurchId)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .HasMany(m => m.Donations)
                .WithOne(d => d.Member)
                .HasForeignKey(d => d.MemberId)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .HasMany(m => m.Events)
                .WithMany(e => e.Participants)
                .UsingEntity<Dictionary<string, object>>(
                    "EventParticipants",
                    j => j
                        .HasOne<Event>()
                        .WithMany()
                        .HasForeignKey("event_id")
                        .OnDelete(DeleteBehavior.Cascade),
                    j => j
                        .HasOne<Member>()
                        .WithMany()
                        .HasForeignKey("member_id")
                        .OnDelete(DeleteBehavior.Cascade),
                    j =>
                    {
                        j.ToTable("event_participants");
                        j.HasKey("event_id", "member_id");
                    });
        }
    }
}
