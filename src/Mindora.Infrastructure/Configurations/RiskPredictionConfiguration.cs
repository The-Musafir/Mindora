using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mindora.Domain.Entities;

namespace Mindora.Infrastructure.Configurations
{
    public class RiskPredictionConfiguration : IEntityTypeConfiguration<RiskPrediction>
    {
        public void Configure(EntityTypeBuilder<RiskPrediction> builder)
        {
            builder.HasKey(r => r.RiskPredictionId);

            builder.Property(r => r.RiskLevel)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValue("Low");

            builder.Property(r => r.Probability)
                .IsRequired();

            builder.Property(r => r.Reason)
                .HasMaxLength(2000);

            builder.Property(r => r.PredictedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            // Relationship
            builder.HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Index
            builder.HasIndex(r => new { r.UserId, r.PredictedAt });
        }
    }
}