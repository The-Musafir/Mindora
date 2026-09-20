using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mindora.Domain.Entities;

namespace Mindora.Infrastructure.Configurations
{
    public class AIFeedbackConfiguration : IEntityTypeConfiguration<AIFeedback>
    {
        public void Configure(EntityTypeBuilder<AIFeedback> builder)
        {
            builder.HasKey(f => f.FeedbackId);

            builder.Property(f => f.Rating)
                .IsRequired();

            builder.Property(f => f.Comment)
                .HasMaxLength(2000);

            builder.Property(f => f.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            // Relationships
            builder.HasOne(f => f.Session)
                .WithMany(s => s.Feedbacks)
                .HasForeignKey(f => f.SessionId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(f => f.User)
                .WithMany()
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(f => f.Message)
                .WithMany()
                .HasForeignKey(f => f.MessageId)
                .OnDelete(DeleteBehavior.SetNull);

            // Unique feedback per user per message (optional)
            builder.HasIndex(f => new { f.SessionId, f.UserId, f.MessageId });
        }
    }
}