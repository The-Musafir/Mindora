using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mindora.Domain.Entities;

namespace Mindora.Infrastructure.Configurations
{
    public class ConsultationReminderConfiguration : IEntityTypeConfiguration<ConsultationReminder>
    {
        public void Configure(EntityTypeBuilder<ConsultationReminder> builder)
        {
            builder.HasKey(r => r.ReminderId);

            builder.Property(r => r.ReminderAt)
                .IsRequired();

            builder.Property(r => r.Channel)
                .IsRequired()
                .HasMaxLength(20)
                .HasDefaultValue("InApp");

            builder.Property(r => r.IsSent)
                .HasDefaultValue(false);

            // Relationship
            builder.HasOne(r => r.Session)
                .WithMany()
                .HasForeignKey(r => r.SessionId)
                .OnDelete(DeleteBehavior.Cascade);

            // Index
            builder.HasIndex(r => new { r.SessionId, r.ReminderAt });
        }
    }
}