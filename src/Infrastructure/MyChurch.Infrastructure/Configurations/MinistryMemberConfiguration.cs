using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyChurch.Domain.Entities;

namespace MyChurch.Infrastructure.Configurations
{
    public class MinistryMemberConfiguration : IEntityTypeConfiguration<MinistryMember>
    {
        public void Configure(EntityTypeBuilder<MinistryMember> builder)
        {
            builder.ToTable("MinistryMembers");

            builder.HasKey(mm => mm.Id);

            builder.Property(mm => mm.Role)
                .HasMaxLength(100);

            builder.Property(mm => mm.Notes)
                .HasMaxLength(500);

            builder.Property(mm => mm.JoinedAt)
                .IsRequired();

            builder.Property(mm => mm.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            // Relacionamento com Ministry
            builder.HasOne(mm => mm.Ministry)
                .WithMany(m => m.MinistryMembers)
                .HasForeignKey(mm => mm.MinistryId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relacionamento com Member
            builder.HasOne(mm => mm.Member)
                .WithMany()
                .HasForeignKey(mm => mm.MemberId)
                .OnDelete(DeleteBehavior.Cascade);

            // Índices
            builder.HasIndex(mm => mm.MinistryId);
            builder.HasIndex(mm => mm.MemberId);
            builder.HasIndex(mm => mm.IsActive);
            
            // Índice único composto para evitar duplicatas
            builder.HasIndex(mm => new { mm.MinistryId, mm.MemberId })
                .IsUnique();
        }
    }
}
