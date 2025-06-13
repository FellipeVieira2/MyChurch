using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyChurch.Domain.Entities;

namespace MyChurch.Infrastructure.Mappings
{
    public class WorshipActivityHymnMap : IEntityTypeConfiguration<WorshipActivityHymn>
    {
        public void Configure(EntityTypeBuilder<WorshipActivityHymn> builder)
        {
            builder.ToTable("worship_activity_hymns", "postgres");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();

            builder.Property(x => x.WorshipActivityId)
                .HasColumnName("worship_activity_id")
                .IsRequired();

            builder.Property(x => x.HymnId)
                .HasColumnName("hymn_id")
                .IsRequired();

            builder.Property(x => x.HymnTitle)
                .HasColumnName("hymn_title");

            builder.Property(x => x.HymnNumber)
                .HasColumnName("hymn_number");

            builder.HasOne(x => x.WorshipActivity)
                .WithMany(x => x.Hymns)
                .HasForeignKey(x => x.WorshipActivityId);
        }
    }
}