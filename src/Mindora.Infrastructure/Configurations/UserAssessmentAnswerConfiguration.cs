using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mindora.Domain.Entities;

namespace Mindora.Infrastructure.Configurations
{
    public class UserAssessmentAnswerConfiguration : IEntityTypeConfiguration<UserAssessmentAnswer>
    {
        public void Configure(EntityTypeBuilder<UserAssessmentAnswer> builder)
        {
            builder.HasKey(a => a.AnswerId);

            // Relationships
            builder.HasOne(a => a.UserAssessment)
                .WithMany(ua => ua.Answers)
                .HasForeignKey(a => a.UserAssessmentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(a => a.Question)
                .WithMany()
                .HasForeignKey(a => a.QuestionId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(a => a.SelectedOption)
                .WithMany()
                .HasForeignKey(a => a.SelectedOptionId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}