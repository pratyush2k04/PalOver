using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pdr.Api.Models.Engagements;

namespace Pdr.Api.Data.Configuration;

public class AssumptionConfiguration : IEntityTypeConfiguration<Assumption>
{
    public void Configure(EntityTypeBuilder<Assumption> builder)
    {
        builder.HasOne(a => a.Owner)
            .WithMany()
            .HasForeignKey(a => a.OwnerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(a => a.Status)
            .HasDefaultValue(AssumptionStatus.Unvalidated)
            .HasSentinel((AssumptionStatus)0)
            .IsRequired();

        builder.HasQueryFilter(a => !a.IsDeleted);
    }
}
