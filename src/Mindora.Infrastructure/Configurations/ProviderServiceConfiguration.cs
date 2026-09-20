using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mindora.Domain.Entities;

namespace Mindora.Infrastructure.Configurations
{
    public class ProviderServiceConfiguration : IEntityTypeConfiguration<ProviderService>
    {
        public void Configure(EntityTypeBuilder<ProviderService> builder)
        {
            builder.HasKey(s => s.ServiceId);

            builder.Property(s => s.ServiceName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(s => s.DurationMinutes)
                .IsRequired();

            builder.Property(s => s.Fee)
                .HasColumnType("decimal(18,2)");

            builder.HasOne(s => s.Practice)
                .WithMany(p => p.Services)
                .HasForeignKey(s => s.PracticeId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}