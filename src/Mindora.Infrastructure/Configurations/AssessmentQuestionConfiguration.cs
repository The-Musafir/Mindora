using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mindora.Domain.Entities;

namespace Mindora.Infrastructure.Configurations
{
    public class AssessmentQuestionConfiguration : IEntityTypeConfiguration<AssessmentQuestion>
    {
        public void Configure(EntityTypeBuilder<AssessmentQuestion> builder)
        {
            builder.HasKey(q => q.QuestionId);

            builder.Property(q => q.QuestionText)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(q => q.OrderIndex)
                .IsRequired();

            // Relationship
            builder.HasOne(q => q.Questionnaire)
                .WithMany(qn => qn.Questions)
                .HasForeignKey(q => q.QuestionnaireId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}