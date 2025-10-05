using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyChurch.Domain.Entities;

namespace MyChurch.Infrastructure.Mappings
{
    public class ChurchPhotoMap : IEntityTypeConfiguration<ChurchPhoto>
    {
        public void Configure(EntityTypeBuilder<ChurchPhoto> builder)
        {
            builder.ToTable("church_photos", "postgres");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();

            builder.Property(x => x.ChurchId)
                .HasColumnName("church_id")
                .HasColumnType("int")
                .IsRequired();

            builder.Property(x => x.UploadedByMemberId)
                .HasColumnName("uploaded_by_member_id")
                .HasColumnType("int");

            builder.Property(x => x.UploadedByVisitorId)
                .HasColumnName("uploaded_by_visitor_id")
                .HasColumnType("int");

            builder.Property(x => x.PhotoUrl)
                .HasColumnName("photo_url")
                .HasColumnType("varchar(500)")
                .HasMaxLength(500)
                .IsRequired();

            builder.Property(x => x.OriginalFileName)
                .HasColumnName("original_file_name")
                .HasColumnType("varchar(255)")
                .HasMaxLength(255);

            builder.Property(x => x.Caption)
                .HasColumnName("caption")
                .HasColumnType("varchar(500)")
                .HasMaxLength(500);

            builder.Property(x => x.Category)
                .HasColumnName("category")
                .HasColumnType("int")
                .IsRequired();

            builder.Property(x => x.UploadedAt)
                .HasColumnName("uploaded_at")
                .HasColumnType("timestamp")
                .IsRequired();

            builder.Property(x => x.Likes)
                .HasColumnName("likes")
                .HasColumnType("int")
                .HasDefaultValue(0)
                .IsRequired();

            builder.Property(x => x.IsApproved)
                .HasColumnName("is_approved")
                .HasColumnType("boolean")
                .HasDefaultValue(false)
                .IsRequired();

            builder.Property(x => x.IsRejected)
                .HasColumnName("is_rejected")
                .HasColumnType("boolean")
                .HasDefaultValue(false)
                .IsRequired();

            builder.Property(x => x.ApprovedByAdminId)
                .HasColumnName("approved_by_admin_id")
                .HasColumnType("int");

            builder.Property(x => x.ApprovedAt)
                .HasColumnName("approved_at")
                .HasColumnType("timestamp");

            builder.Property(x => x.RejectionReason)
                .HasColumnName("rejection_reason")
                .HasColumnType("varchar(500)")
                .HasMaxLength(500);

            builder.Property(x => x.IsFeatured)
                .HasColumnName("is_featured")
                .HasColumnType("boolean")
                .HasDefaultValue(false)
                .IsRequired();

            builder.Property(x => x.DisplayOrder)
                .HasColumnName("display_order")
                .HasColumnType("int")
                .HasDefaultValue(0)
                .IsRequired();

            // Relacionamentos
            builder.HasOne(x => x.Church)
                .WithMany()
                .HasForeignKey(x => x.ChurchId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.UploadedBy)
                .WithMany()
                .HasForeignKey(x => x.UploadedByMemberId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(x => x.UploadedByVisitor)
                .WithMany()
                .HasForeignKey(x => x.UploadedByVisitorId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(x => x.ApprovedByAdmin)
                .WithMany()
                .HasForeignKey(x => x.ApprovedByAdminId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasMany(x => x.PhotoLikes)
                .WithOne(l => l.ChurchPhoto)
                .HasForeignKey(l => l.ChurchPhotoId)
                .OnDelete(DeleteBehavior.Cascade);

            // Índices
            builder.HasIndex(x => new { x.ChurchId, x.IsApproved, x.Category })
                .HasDatabaseName("IX_church_photos_church_approved_category");

            builder.HasIndex(x => x.UploadedAt)
                .HasDatabaseName("IX_church_photos_uploaded_at");

            builder.HasIndex(x => new { x.IsFeatured, x.DisplayOrder })
                .HasDatabaseName("IX_church_photos_featured_order");
        }
    }
    
    public class ChurchPhotoLikeMap : IEntityTypeConfiguration<ChurchPhotoLike>
    {
        public void Configure(EntityTypeBuilder<ChurchPhotoLike> builder)
        {
            builder.ToTable("church_photo_likes", "postgres");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();

            builder.Property(x => x.ChurchPhotoId)
                .HasColumnName("church_photo_id")
                .HasColumnType("int")
                .IsRequired();

            builder.Property(x => x.MemberId)
                .HasColumnName("member_id")
                .HasColumnType("int");

            builder.Property(x => x.VisitorId)
                .HasColumnName("visitor_id")
                .HasColumnType("int");

            builder.Property(x => x.LikedAt)
                .HasColumnName("liked_at")
                .HasColumnType("timestamp")
                .IsRequired();

            // Relacionamentos
            builder.HasOne(x => x.ChurchPhoto)
                .WithMany(p => p.PhotoLikes)
                .HasForeignKey(x => x.ChurchPhotoId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Member)
                .WithMany()
                .HasForeignKey(x => x.MemberId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Visitor)
                .WithMany()
                .HasForeignKey(x => x.VisitorId)
                .OnDelete(DeleteBehavior.Cascade);

            // Índice único: um usuário só pode curtir uma foto uma vez
            builder.HasIndex(x => new { x.ChurchPhotoId, x.MemberId })
                .IsUnique()
                .HasDatabaseName("IX_church_photo_likes_photo_member")
                .HasFilter("member_id IS NOT NULL");

            builder.HasIndex(x => new { x.ChurchPhotoId, x.VisitorId })
                .IsUnique()
                .HasDatabaseName("IX_church_photo_likes_photo_visitor")
                .HasFilter("visitor_id IS NOT NULL");
        }
    }
}
