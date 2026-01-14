using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyChurch.Domain.Entities;

namespace MyChurch.Infrastructure.Mappings
{
    public class PlatformUserMap : IEntityTypeConfiguration<PlatformUser>
    {
        public void Configure(EntityTypeBuilder<PlatformUser> builder)
        {
            builder.ToTable("platform_user", "postgres");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasColumnName("id")
                .HasColumnType("int")
                .ValueGeneratedOnAdd()
                .IsRequired();

            builder.Property(x => x.Name)
                .HasColumnName("name")
                .HasColumnType("varchar(200)")
                .IsRequired();

            builder.Property(x => x.Email)
                .HasColumnName("email")
                .HasColumnType("varchar(200)")
                .IsRequired();

            builder.HasIndex(x => x.Email)
                .IsUnique()
                .HasDatabaseName("ux_platform_user_email");

            builder.Property(x => x.Photo)
                .HasColumnName("photo")
                .HasColumnType("varchar(500)")
                .IsRequired(false);

            builder.Property(x => x.PasswordHash)
                .HasColumnName("password_hash")
                .HasColumnType("varchar(500)")
                .IsRequired(false);

            builder.Property(x => x.IsActive)
                .HasColumnName("is_active")
                .HasColumnType("boolean")
                .HasDefaultValue(true)
                .IsRequired();

            builder.Property(x => x.Role)
                .HasColumnName("role")
                .HasColumnType("int")
                .IsRequired();

            builder.Property(x => x.Created)
                .HasColumnName("created")
                .HasColumnType("timestamp")
                .IsRequired();

            builder.Property(x => x.Updated)
                .HasColumnName("updated")
                .HasColumnType("timestamp")
                .IsRequired(false);
        }
    }
}
