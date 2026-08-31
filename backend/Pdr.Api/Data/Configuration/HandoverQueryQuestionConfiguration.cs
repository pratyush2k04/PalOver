using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pdr.Api.Models.Handover;

namespace Pdr.Api.Data.Configuration;

public class HandoverQueryQuestionConfiguration : IEntityTypeConfiguration<HandoverQueryQuestion>
{
    public void Configure(EntityTypeBuilder<HandoverQueryQuestion> builder)
    {
        builder.ToTable("handover_query_questions");

        builder.HasKey(q => q.QueryId);
        builder.Property(q => q.QueryId).HasColumnName("query_id").ValueGeneratedNever();

        builder.Property(q => q.QuestionText).HasColumnName("question_text").HasMaxLength(2000).IsRequired();

        // BaseAuditableEntity Column Mappings
        builder.Property(q => q.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("NOW()");
        builder.Property(q => q.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("NOW()");
        builder.Property(q => q.CreatedBy).HasColumnName("created_by").HasMaxLength(255);
        builder.Property(q => q.UpdatedBy).HasColumnName("updated_by").HasMaxLength(255);
        builder.Property(q => q.IsDeleted).HasColumnName("is_deleted").HasDefaultValue(false);

        builder.HasQueryFilter(x => !x.IsDeleted);

        builder.HasOne(q => q.Query)
            .WithOne(hq => hq.Question)
            .HasForeignKey<HandoverQueryQuestion>(q => q.QueryId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
