using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mindora.Domain.Entities;

namespace Mindora.Infrastructure.Configurations
{
    public class ConsultationPaymentConfiguration : IEntityTypeConfiguration<ConsultationPayment>
    {
        public void Configure(EntityTypeBuilder<ConsultationPayment> builder)
        {
            builder.HasKey(cp => cp.ConsultationPaymentId);

            builder.Property(cp => cp.Amount)
                .HasColumnType("decimal(18,2)");

            builder.Property(cp => cp.Status)
                .IsRequired()
                .HasMaxLength(20)
                .HasDefaultValue("Pending");

            builder.Property(cp => cp.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            // Relationships
            builder.HasOne(cp => cp.User)
                .WithMany()
                .HasForeignKey(cp => cp.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(cp => cp.Professional)
                .WithMany()
                .HasForeignKey(cp => cp.ProfessionalId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(cp => cp.Appointment)
                .WithMany()
                .HasForeignKey(cp => cp.AppointmentId)
                .OnDelete(DeleteBehavior.Restrict);

            // Index
            builder.HasIndex(cp => cp.UserId);
            builder.HasIndex(cp => cp.ProfessionalId);
            builder.HasIndex(cp => cp.AppointmentId);
        }
    }
}