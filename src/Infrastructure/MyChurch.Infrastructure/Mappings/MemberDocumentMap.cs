using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyChurch.Domain.Entities;

namespace MyChurch.Infrastructure.Mappings
{
    public class MemberDocumentMap : IEntityTypeConfiguration<MemberDocument>
    {
        public void Configure(EntityTypeBuilder<MemberDocument> builder)
        {
            builder.ToTable("member_document", "postgres");

            builder.HasKey(md => md.Id);

            builder
                .Property(md => md.Id)
                .HasColumnName("id")
                .HasColumnType("int")
                .ValueGeneratedOnAdd()
                .IsRequired();

            builder
                .Property(md => md.Type)
                .HasColumnName("type")
                .HasColumnType("int") // Enum armazenado como int
                .IsRequired();

            builder
                .Property(md => md.Number)
                .HasColumnName("number")
                .HasColumnType("varchar(50)")
                .IsRequired();

            builder
                .Property(md => md.MemberId)
                .HasColumnName("member_id")
                .HasColumnType("int")
                .IsRequired();

            builder
                .HasOne(md => md.Member)
                .WithMany(m => m.Documents)
                .HasForeignKey(md => md.MemberId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}