using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mindora.Domain.Entities;

namespace Mindora.Infrastructure.Configurations
{
    public class ConsultationSessionConfiguration : IEntityTypeConfiguration<ConsultationSession>
    {
        public void Configure(EntityTypeBuilder<ConsultationSession> builder)
        {
            builder.HasKey(s => s.SessionId);

            builder.Property(s => s.SessionType)
                .IsRequired()
                .HasMaxLength(20)
                .HasDefaultValue("Chat");

            builder.Property(s => s.Status)
                .IsRequired()
                .HasMaxLength(30)
                .HasDefaultValue("Scheduled");

            builder.Property(s => s.ScheduledAt)
                .IsRequired();

            builder.Property(s => s.MeetingLink)
                .HasMaxLength(500);

            builder.Property(s => s.Notes)
                .HasMaxLength(2000);

            builder.Property(s => s.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            // Relationships
            builder.HasOne(s => s.Provider)
                .WithMany()
                .HasForeignKey(s => s.ProviderId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(s => s.User)
                .WithMany()
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(s => s.Appointment)
                .WithMany()
                .HasForeignKey(s => s.AppointmentId)
                .OnDelete(DeleteBehavior.SetNull);

            // Indexes
            builder.HasIndex(s => s.UserId);
            builder.HasIndex(s => s.ProviderId);
            builder.HasIndex(s => s.Status);
            builder.HasIndex(s => new { s.UserId, s.ScheduledAt });
        }
    }
}