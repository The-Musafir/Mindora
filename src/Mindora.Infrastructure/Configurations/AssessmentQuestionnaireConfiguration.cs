using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mindora.Domain.Entities;

namespace Mindora.Infrastructure.Configurations
{
    public class AssessmentQuestionnaireConfiguration : IEntityTypeConfiguration<AssessmentQuestionnaire>
    {
        public void Configure(EntityTypeBuilder<AssessmentQuestionnaire> builder)
        {
            builder.HasKey(q => q.QuestionnaireId);

            builder.Property(q => q.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(q => q.Description)
                .HasMaxLength(2000);

            builder.Property(q => q.IsActive)
                .HasDefaultValue(true);
        }
    }
}