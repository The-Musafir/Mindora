using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mindora.Domain.Entities;

namespace Mindora.Infrastructure.Configurations
{
    public class WellnessScoreConfiguration : IEntityTypeConfiguration<WellnessScore>
    {
        public void Configure(EntityTypeBuilder<WellnessScore> builder)
        {
            builder.HasKey(w => w.WellnessScoreId);

            builder.Property(w => w.Score)
                .IsRequired();

            builder.Property(w => w.Category)
                .HasMaxLength(50);

            builder.Property(w => w.CalculatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            // Relationship
            builder.HasOne(w => w.User)
                .WithMany()
                .HasForeignKey(w => w.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Index
            builder.HasIndex(w => new { w.UserId, w.CalculatedAt });
        }
    }
}