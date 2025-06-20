using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyChurch.Domain.Entities;

namespace MyChurch.Infrastructure.Mappings
{
    public class MemberAchievementMap : IEntityTypeConfiguration<MemberAchievement>
    {
        public void Configure(EntityTypeBuilder<MemberAchievement> builder)
        {
            builder.ToTable("member_achievements");

            builder.HasKey(ma => ma.Id);

            builder.Property(ma => ma.Id).HasColumnName("id");
            builder.Property(ma => ma.MemberId).HasColumnName("member_id").IsRequired();
            builder.Property(ma => ma.AchievementId).HasColumnName("achievement_id").IsRequired();
            builder.Property(ma => ma.AwardedAt).HasColumnName("awarded_at").IsRequired();
        }
    }
}
