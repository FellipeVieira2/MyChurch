using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyChurch.Domain.Entities;

namespace MyChurch.Infrastructure.Mappings
{
    public class FamilyInvitationMap : IEntityTypeConfiguration<FamilyInvitation>
    {
        public void Configure(EntityTypeBuilder<FamilyInvitation> builder)
        {
            builder.ToTable("family_invitations");
            builder.HasKey(f => f.Id);
            builder.Property(f => f.Id).HasColumnName("id");
            builder.Property(f => f.ChurchId).HasColumnName("church_id");
            builder.Property(f => f.InviterMemberId).HasColumnName("inviter_member_id");
            builder.Property(f => f.Status).HasColumnName("status");
            builder.Property(f => f.CreatedAt).HasColumnName("created_at");
            builder.Property(f => f.RespondedAt).HasColumnName("responded_at");
        }
    }
}
