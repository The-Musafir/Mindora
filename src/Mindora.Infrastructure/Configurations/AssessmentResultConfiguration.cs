using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mindora.Domain.Entities;

namespace Mindora.Infrastructure.Configurations
{
    public class AssessmentResultConfiguration : IEntityTypeConfiguration<AssessmentResult>
    {
        public void Configure(EntityTypeBuilder<AssessmentResult> builder)
        {
            builder.HasKey(r => r.ResultId);

            builder.Property(r => r.TotalScore)
                .IsRequired();

            builder.Property(r => r.SeverityLevel)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(r => r.Interpretation)
                .HasMaxLength(2000);

            // Relationship (One-to-One with UserAssessment)
            builder.HasOne(r => r.UserAssessment)
                .WithOne(ua => ua.Result)
                .HasForeignKey<AssessmentResult>(r => r.UserAssessmentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(r => r.UserAssessmentId)
                .IsUnique();
        }
    }
}