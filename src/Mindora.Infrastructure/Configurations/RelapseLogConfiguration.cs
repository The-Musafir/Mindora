using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mindora.Domain.Entities;

namespace Mindora.Infrastructure.Configurations
{
    public class RelapseLogConfiguration : IEntityTypeConfiguration<RelapseLog>
    {
        public void Configure(EntityTypeBuilder<RelapseLog> builder)
        {
            builder.HasKey(r => r.RelapseLogId);

            builder.Property(r => r.RelapseDate)
                .IsRequired();

            builder.Property(r => r.Reason)
                .HasMaxLength(2000);

            builder.Property(r => r.Note)
                .HasMaxLength(2000);

            // Relationship
            builder.HasOne(r => r.Habit)
                .WithMany(h => h.Relapses)
                .HasForeignKey(r => r.HabitId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}