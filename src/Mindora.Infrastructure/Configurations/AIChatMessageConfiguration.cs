using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mindora.Domain.Entities;

namespace Mindora.Infrastructure.Configurations
{
    public class AIChatMessageConfiguration : IEntityTypeConfiguration<AIChatMessage>
    {
        public void Configure(EntityTypeBuilder<AIChatMessage> builder)
        {
            builder.HasKey(m => m.MessageId);

            builder.Property(m => m.Sender)
                .IsRequired()
                .HasMaxLength(20)
                .HasDefaultValue("User");

            builder.Property(m => m.Content)
                .IsRequired();

            builder.Property(m => m.Sentiment)
                .HasMaxLength(50);

            builder.Property(m => m.RiskLevel)
                .HasMaxLength(50);

            builder.Property(m => m.Intent)
                .HasMaxLength(100);

            builder.Property(m => m.SentAt)
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(m => m.Metadata)
                .HasMaxLength(4000);

            // Relationship
            builder.HasOne(m => m.Session)
                .WithMany(s => s.Messages)
                .HasForeignKey(m => m.SessionId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            builder.HasIndex(m => m.SessionId);
            builder.HasIndex(m => new { m.SessionId, m.SentAt });
        }
    }
}