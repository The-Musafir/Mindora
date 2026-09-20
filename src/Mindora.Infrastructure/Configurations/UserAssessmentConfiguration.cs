using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mindora.Domain.Entities;

namespace Mindora.Infrastructure.Configurations
{
    public class UserAssessmentConfiguration : IEntityTypeConfiguration<UserAssessment>
    {
        public void Configure(EntityTypeBuilder<UserAssessment> builder)
        {
            builder.HasKey(ua => ua.UserAssessmentId);

            builder.Property(ua => ua.Status)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValue("InProgress");

            builder.Property(ua => ua.StartedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            // Relationships
            builder.HasOne(ua => ua.User)
                .WithMany()
                .HasForeignKey(ua => ua.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(ua => ua.Questionnaire)
                .WithMany(q => q.UserAssessments)
                .HasForeignKey(ua => ua.QuestionnaireId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}