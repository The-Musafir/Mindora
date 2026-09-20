using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mindora.Domain.Entities;

namespace Mindora.Infrastructure.Configurations
{
    public class AIInteractionLogConfiguration : IEntityTypeConfiguration<AIInteractionLog>
    {
        public void Configure(EntityTypeBuilder<AIInteractionLog> builder)
        {
            builder.HasKey(l => l.LogId);

            builder.Property(l => l.Action)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(l => l.Metadata)
                .HasMaxLength(4000);

            builder.Property(l => l.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            // Relationships
            builder.HasOne(l => l.Session)
                .WithMany(s => s.InteractionLogs)
                .HasForeignKey(l => l.SessionId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(l => l.Template)
                .WithMany(t => t.InteractionLogs)
                .HasForeignKey(l => l.TemplateId)
                .OnDelete(DeleteBehavior.SetNull);

            // Index
            builder.HasIndex(l => l.SessionId);
            builder.HasIndex(l => l.TemplateId);
        }
    }
}