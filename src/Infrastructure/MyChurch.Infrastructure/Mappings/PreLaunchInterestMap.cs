using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyChurch.Domain.Entities;

namespace MyChurch.Infrastructure.Mappings
{
    public class PreLaunchInterestMap : IEntityTypeConfiguration<PreLaunchInterest>
    {
        public void Configure(EntityTypeBuilder<PreLaunchInterest> builder)
        {
            builder.ToTable("pre_launch_interests", "postgres");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasColumnName("id").IsRequired();
            builder.Property(x => x.Name).HasColumnName("name").IsRequired();
            builder.Property(x => x.Email).HasColumnName("email").IsRequired();
            builder.Property(x => x.Phone).HasColumnName("phone").IsRequired(false);
            builder.Property(x => x.ChurchName).HasColumnName("church_name").IsRequired(false);
            builder.Property(x => x.ChurchRole).HasColumnName("church_role").IsRequired(false);
            builder.Property(x => x.Comments).HasColumnName("comments").IsRequired(false);
            builder.Property(x => x.RegisterDate).HasColumnName("register_date").IsRequired();
            builder.Property(x => x.IsEmailConfirmed).HasColumnName("is_email_confirmed").IsRequired();
            builder.Property(x => x.ConfirmationToken).HasColumnName("confirmation_token").IsRequired();
        }
    }
}