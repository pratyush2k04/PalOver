using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pdr.Api.Models.Risk;

namespace Pdr.Api.Data.Configuration
{
    public class RiskConfiguration : IEntityTypeConfiguration<Risk>
    {
        public void Configure(EntityTypeBuilder<Risk> builder)
        {
            builder.HasKey(x => x.RiskId);

            builder.Property(x => x.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.Description)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(x => x.Status)
                .IsRequired();

            builder.HasOne(x => x.Engagement)
                .WithMany(x => x.Risks)
                .HasForeignKey(x => x.EngagementId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
