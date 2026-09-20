using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mindora.Domain.Entities;

namespace Mindora.Infrastructure.Configurations
{
    public class HabitLogConfiguration : IEntityTypeConfiguration<HabitLog>
    {
        public void Configure(EntityTypeBuilder<HabitLog> builder)
        {
            builder.HasKey(l => l.HabitLogId);

            builder.Property(l => l.LogDate)
                .IsRequired();

            builder.Property(l => l.IsCompleted)
                .HasDefaultValue(false);

            builder.Property(l => l.Note)
                .HasMaxLength(2000);

            builder.Property(l => l.Reflection)
                .HasMaxLength(2000);

            builder.Property(l => l.MoodBefore)
                .HasComment("1-10 scale");

            builder.Property(l => l.MoodAfter)
                .HasComment("1-10 scale");

            builder.Property(l => l.CompletionTimeMinutes);

            // Relationship
            builder.HasOne(l => l.Habit)
                .WithMany(h => h.Logs)
                .HasForeignKey(l => l.HabitId)
                .OnDelete(DeleteBehavior.Cascade);

            // Index
            builder.HasIndex(l => new { l.HabitId, l.LogDate })
                .IsUnique();
        }
    }
}