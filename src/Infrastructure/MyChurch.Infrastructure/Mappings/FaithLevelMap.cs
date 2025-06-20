using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyChurch.Domain.Entities;

namespace MyChurch.Infrastructure.Mappings
{
    public class FaithLevelMap : IEntityTypeConfiguration<FaithLevel>
    {
        public void Configure(EntityTypeBuilder<FaithLevel> builder)
        {
            builder.ToTable("faith_levels");

            builder.HasKey(fl => fl.Id);

            builder.Property(fl => fl.Id).HasColumnName("id");
            builder.Property(fl => fl.Name).HasColumnName("name").IsRequired().HasMaxLength(100);
            builder.Property(fl => fl.PointsRequired).HasColumnName("points_required").IsRequired();
            builder.Property(fl => fl.IconUrl).HasColumnName("icon_url").IsRequired();

            // Relacionamento com Member (um FaithLevel pode ter muitos Members)
            builder.HasMany<Member>()
                .WithOne(m => m.FaithLevel)
                .HasForeignKey(m => m.FaithLevelId)
                .OnDelete(DeleteBehavior.SetNull); // Se um nível for deletado, o nível do membro fica nulo
        }
    }
}
