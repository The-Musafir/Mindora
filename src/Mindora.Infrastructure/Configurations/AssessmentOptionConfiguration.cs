using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mindora.Domain.Entities;

namespace Mindora.Infrastructure.Configurations
{
    public class AssessmentOptionConfiguration : IEntityTypeConfiguration<AssessmentOption>
    {
        public void Configure(EntityTypeBuilder<AssessmentOption> builder)
        {
            builder.HasKey(o => o.OptionId);

            builder.Property(o => o.OptionText)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(o => o.ScoreValue)
                .IsRequired();

            // Relationship
            builder.HasOne(o => o.Question)
                .WithMany(q => q.Options)
                .HasForeignKey(o => o.QuestionId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}