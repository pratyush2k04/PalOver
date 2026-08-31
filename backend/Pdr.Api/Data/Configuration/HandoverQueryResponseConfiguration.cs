using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pdr.Api.Models.Handover;

namespace Pdr.Api.Data.Configuration;

public class HandoverQueryResponseConfiguration : IEntityTypeConfiguration<HandoverQueryResponse>
{
    public void Configure(EntityTypeBuilder<HandoverQueryResponse> builder)
    {
        builder.ToTable("handover_query_responses");

        builder.HasKey(r => r.QueryId);
        builder.Property(r => r.QueryId).HasColumnName("query_id").ValueGeneratedNever();

        builder.Property(r => r.ResponseText).HasColumnName("response_text").HasMaxLength(2000).IsRequired();

        // BaseAuditableEntity Column Mappings
        builder.Property(r => r.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("NOW()");
        builder.Property(r => r.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("NOW()");
        builder.Property(r => r.CreatedBy).HasColumnName("created_by").HasMaxLength(255);
        builder.Property(r => r.UpdatedBy).HasColumnName("updated_by").HasMaxLength(255);
        builder.Property(r => r.IsDeleted).HasColumnName("is_deleted").HasDefaultValue(false);

        builder.HasQueryFilter(x => !x.IsDeleted);

        builder.HasOne(r => r.Query)
            .WithOne(hq => hq.Response)
            .HasForeignKey<HandoverQueryResponse>(r => r.QueryId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
