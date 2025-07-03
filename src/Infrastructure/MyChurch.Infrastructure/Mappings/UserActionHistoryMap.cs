using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyChurch.Domain.Entities;

namespace MyChurch.Infrastructure.Mappings
{
    public class UserActionHistoryMap : IEntityTypeConfiguration<UserActionHistory>
    {
        public void Configure(EntityTypeBuilder<UserActionHistory> builder)
        {
            builder.ToTable("user_action_history", "postgres");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasColumnName("id").HasColumnType("int").ValueGeneratedOnAdd().IsRequired();
            builder.Property(x => x.MemberId).HasColumnName("member_id").HasColumnType("int").IsRequired();
            builder.Property(x => x.ActionType).HasColumnName("action_type").HasColumnType("varchar(100)").IsRequired();
            builder.Property(x => x.ActionData).HasColumnName("action_data").HasColumnType("text");
            builder.Property(x => x.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp").IsRequired();
        }
    }
}
