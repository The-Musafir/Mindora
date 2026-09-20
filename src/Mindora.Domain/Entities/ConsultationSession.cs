using System;
using System.Collections.Generic;

namespace Mindora.Domain.Entities
{
    public class ConsultationSession
    {
        public Guid SessionId { get; set; }
        public Guid? AppointmentId { get; set; }
        public Guid ProviderId { get; set; }
        public Guid UserId { get; set; }
        public string SessionType { get; set; } = "Chat"; // Chat, Video, Audio
        public string Status { get; set; } = "Scheduled"; // Scheduled, InProgress, Completed, Cancelled, NoShow
        public DateTime ScheduledAt { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? EndedAt { get; set; }
        public string? MeetingLink { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Navigation
        public virtual ProfessionalProvider Provider { get; set; } = null!;
        public virtual User User { get; set; } = null!;
        public virtual Appointment? Appointment { get; set; }
        public virtual ICollection<ConsultationNote> NotesCollection { get; set; } = new List<ConsultationNote>();
        public virtual ICollection<ConsultationPrescription> Prescriptions { get; set; } = new List<ConsultationPrescription>();
        public virtual ICollection<ConsultationFeedback> Feedbacks { get; set; } = new List<ConsultationFeedback>();
        public virtual ICollection<FollowUpPlan> FollowUpPlans { get; set; } = new List<FollowUpPlan>();
    }
}