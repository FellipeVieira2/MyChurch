using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyChurch.Domain.Entities;
using MyChurch.Domain.Enum;

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
                .Property(x => x.FamilyId)
                .HasColumnName("family_id")
                .IsRequired(false);

            builder
                .Property(x => x.PendingApproval)
                .HasColumnName("pending_approval")
                .HasColumnType("boolean");

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
                .Property(m => m.BirthCity)
                .HasColumnName("birth_city")
                .HasColumnType("varchar(100)")
                .IsRequired(false);

            builder
                .Property(m => m.BirthState)
                .HasColumnName("birth_state")
                .HasColumnType("varchar(100)")
                .IsRequired(false);

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
                .HasDefaultValue(false)
                .IsRequired();

            builder
                .Property(m => m.ChurchId)
                .HasColumnName("church_id")
                .HasColumnType("int")
                .IsRequired();

            builder
                .Property(m => m.MaritalStatus)
                .HasColumnName("marital_status")
                .HasColumnType("varchar(50)")
                .HasConversion(
                    v => v.HasValue ? v.Value.ToString() : null, // Enum para string
                    v => string.IsNullOrEmpty(v) ? null : Enum.Parse<MaritalStatus>(v) // String para Enum
                )
                .IsRequired(false);

            builder
                .Property(m => m.MemberSince)
                .HasColumnName("member_since")
                .HasColumnType("date")
                .IsRequired(false);

            builder
                .Property(m => m.Ministry)
                .HasColumnName("ministry")
                .HasColumnType("varchar(100)")
                .IsRequired(false);

            builder
                .Property(m => m.IsActive)
                .HasColumnName("is_active")
                .HasColumnType("boolean")
                .HasDefaultValue(true)
                .IsRequired();

            builder
                .Property(m => m.Notes)
                .HasColumnName("notes")
                .HasColumnType("varchar(1000)")
                .IsRequired(false);

            // Novo campo AddressId
            builder
                .Property(m => m.AddressId)
                .HasColumnName("address_id")
                .HasColumnType("int")
                .IsRequired(false);

            // Relacionamento Address
            builder
                .HasOne(m => m.Address)
                .WithMany(a => a.Members)
                .HasForeignKey(m => m.AddressId)
                .OnDelete(DeleteBehavior.SetNull);

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

            // Engagement Score
            builder
                .Property(m => m.EngagementScore)
                .HasColumnName("engagement_score")
                .HasColumnType("int")
                .HasDefaultValue(0)
                .IsRequired();

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
                .HasMany(m => m.CashFlowEntries)
                .WithOne(cfe => cfe.Member)
                .HasForeignKey(cfe => cfe.MemberId)
                .OnDelete(DeleteBehavior.SetNull);

            builder
                .HasMany(m => m.CreditCardInfos)
                .WithOne(c => c.Member)
                .HasForeignKey(c => c.MemberId)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .HasMany(m => m.Documents)
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