using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pdr.Api.Models.Handover;

namespace Pdr.Api.Data.Configuration;

public class HandoverQueryMessageConfiguration : IEntityTypeConfiguration<HandoverQueryMessage>
{
    public void Configure(EntityTypeBuilder<HandoverQueryMessage> builder)
    {
        builder.ToTable("handover_query_messages");

        builder.HasKey(m => m.Id);
        builder.Property(m => m.Id).HasColumnName("id").UseIdentityAlwaysColumn();

        builder.Property(m => m.QueryId).HasColumnName("query_id").IsRequired();
        builder.Property(m => m.SenderUserId).HasColumnName("sender_user_id").IsRequired();
        builder.Property(m => m.MessageType).HasColumnName("message_type").HasConversion<int>().IsRequired();
        builder.Property(m => m.MessageText).HasColumnName("message_text").HasMaxLength(2000).IsRequired();
        builder.Property(m => m.SequenceNumber).HasColumnName("sequence_number").HasDefaultValue(1);

        // BaseAuditableEntity Column Mappings
        builder.Property(m => m.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("NOW()");
        builder.Property(m => m.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("NOW()");
        builder.Property(m => m.CreatedBy).HasColumnName("created_by").HasMaxLength(255);
        builder.Property(m => m.UpdatedBy).HasColumnName("updated_by").HasMaxLength(255);
        builder.Property(m => m.IsDeleted).HasColumnName("is_deleted").HasDefaultValue(false);

        builder.HasQueryFilter(x => !x.IsDeleted);

        // Relationships
        builder.HasOne(m => m.Query)
            .WithMany(q => q.Messages)
            .HasForeignKey(m => m.QueryId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(m => m.SenderUser)
            .WithMany()
            .HasForeignKey(m => m.SenderUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(m => new { m.QueryId, m.SequenceNumber });
    }
}
