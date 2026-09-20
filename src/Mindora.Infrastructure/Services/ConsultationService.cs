using Microsoft.EntityFrameworkCore;
using Mindora.Application.DTOs.Consultation;
using Mindora.Application.Interfaces;
using Mindora.Domain.Entities;
using Mindora.Infrastructure.Notifications;
using Mindora.Infrastructure.Persistence.DbContext;

namespace Mindora.Infrastructure.Services
{
    public class ConsultationService : IConsultationService
    {
        private readonly MindoraDbContext _context;
        private readonly INotificationDispatcher _dispatcher;

        public ConsultationService(MindoraDbContext context, INotificationDispatcher dispatcher)
        {
            _context = context;
            _dispatcher = dispatcher;
        }

        // ============================
        // CONSULTATION SESSION
        // ============================

        public async Task<ConsultationSessionDto> CreateSessionAsync(Guid userId, CreateConsultationSessionRequest request)
        {
            var session = new ConsultationSession
            {
                SessionId = Guid.NewGuid(),
                AppointmentId = request.AppointmentId,
                ProviderId = request.ProviderId,
                UserId = userId,
                SessionType = request.SessionType,
                Status = "Scheduled",
                ScheduledAt = request.ScheduledAt,
                MeetingLink = request.MeetingLink,
                Notes = request.Notes,
                CreatedAt = DateTime.UtcNow
            };

            _context.ConsultationSessions.Add(session);
            await _context.SaveChangesAsync();

            // ============================================================
            // NOTIFICATION: Consultation scheduled
            // ============================================================
            await NotifyUserOfScheduledConsultationAsync(userId, session.SessionId);

            return await MapSessionToDtoAsync(session);
        }

        public async Task<ConsultationSessionDto?> GetSessionByIdAsync(Guid sessionId)
        {
            var session = await _context.ConsultationSessions
                .Include(s => s.Provider).ThenInclude(p => p.User)
                .Include(s => s.User)
                .Include(s => s.NotesCollection)
                .Include(s => s.Prescriptions)
                .Include(s => s.Feedbacks)
                .Include(s => s.FollowUpPlans)
                .FirstOrDefaultAsync(s => s.SessionId == sessionId);

            return session == null ? null : await MapSessionToDtoAsync(session);
        }

        public async Task<IReadOnlyList<ConsultationSessionDto>> GetUserSessionsAsync(Guid userId)
        {
            var sessions = await _context.ConsultationSessions
                .Where(s => s.UserId == userId)
                .Include(s => s.Provider).ThenInclude(p => p.User)
                .Include(s => s.User)
                .Include(s => s.NotesCollection)
                .Include(s => s.Prescriptions)
                .Include(s => s.Feedbacks)
                .Include(s => s.FollowUpPlans)
                .OrderByDescending(s => s.ScheduledAt)
                .ToListAsync();

            var result = new List<ConsultationSessionDto>();
            foreach (var session in sessions)
                result.Add(await MapSessionToDtoAsync(session));

            return result;
        }

        public async Task<IReadOnlyList<ConsultationSessionDto>> GetProviderSessionsAsync(Guid providerId)
        {
            var sessions = await _context.ConsultationSessions
                .Where(s => s.ProviderId == providerId)
                .Include(s => s.Provider).ThenInclude(p => p.User)
                .Include(s => s.User)
                .Include(s => s.NotesCollection)
                .Include(s => s.Prescriptions)
                .Include(s => s.Feedbacks)
                .Include(s => s.FollowUpPlans)
                .OrderByDescending(s => s.ScheduledAt)
                .ToListAsync();

            var result = new List<ConsultationSessionDto>();
            foreach (var session in sessions)
                result.Add(await MapSessionToDtoAsync(session));

            return result;
        }

        public async Task<ConsultationSessionDto> UpdateSessionStatusAsync(UpdateConsultationSessionStatusRequest request)
        {
            var session = await _context.ConsultationSessions
                .Include(s => s.Provider).ThenInclude(p => p.User)
                .Include(s => s.User)
                .Include(s => s.NotesCollection)
                .Include(s => s.Prescriptions)
                .Include(s => s.Feedbacks)
                .Include(s => s.FollowUpPlans)
                .FirstOrDefaultAsync(s => s.SessionId == request.SessionId);

            if (session == null)
                throw new KeyNotFoundException($"Session {request.SessionId} not found.");

            session.Status = request.Status;
            session.StartedAt = request.StartedAt;
            session.EndedAt = request.EndedAt;
            session.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return await MapSessionToDtoAsync(session);
        }

        // ============================
        // CONSULTATION NOTE
        // ============================

        public async Task<Guid> AddNoteAsync(CreateConsultationNoteRequest request)
        {
            var note = new ConsultationNote
            {
                NoteId = Guid.NewGuid(),
                SessionId = request.SessionId,
                ProviderId = request.ProviderId,
                Content = request.Content,
                CreatedAt = DateTime.UtcNow
            };

            _context.ConsultationNotes.Add(note);
            await _context.SaveChangesAsync();
            return note.NoteId;
        }

        public async Task<IReadOnlyList<ConsultationNoteDto>> GetSessionNotesAsync(Guid sessionId)
        {
            var notes = await _context.ConsultationNotes
                .Where(n => n.SessionId == sessionId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();

            return notes.Select(n => new ConsultationNoteDto
            {
                NoteId = n.NoteId,
                SessionId = n.SessionId,
                ProviderId = n.ProviderId,
                Content = n.Content,
                CreatedAt = n.CreatedAt
            }).ToList();
        }

        // ============================
        // CONSULTATION PRESCRIPTION
        // ============================

        public async Task<Guid> AddPrescriptionAsync(CreateConsultationPrescriptionRequest request)
        {
            var prescription = new ConsultationPrescription
            {
                PrescriptionId = Guid.NewGuid(),
                SessionId = request.SessionId,
                Content = request.Content,
                CreatedAt = DateTime.UtcNow
            };

            _context.ConsultationPrescriptions.Add(prescription);
            await _context.SaveChangesAsync();
            return prescription.PrescriptionId;
        }

        public async Task<IReadOnlyList<ConsultationPrescriptionDto>> GetSessionPrescriptionsAsync(Guid sessionId)
        {
            var prescriptions = await _context.ConsultationPrescriptions
                .Where(p => p.SessionId == sessionId)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            return prescriptions.Select(p => new ConsultationPrescriptionDto
            {
                PrescriptionId = p.PrescriptionId,
                SessionId = p.SessionId,
                Content = p.Content,
                CreatedAt = p.CreatedAt
            }).ToList();
        }

        // ============================
        // CONSULTATION FEEDBACK
        // ============================

        public async Task<Guid> AddFeedbackAsync(Guid userId, CreateConsultationFeedbackRequest request)
        {
            var feedback = new ConsultationFeedback
            {
                FeedbackId = Guid.NewGuid(),
                SessionId = request.SessionId,
                UserId = userId,
                Rating = request.Rating,
                Comment = request.Comment,
                CreatedAt = DateTime.UtcNow
            };

            _context.ConsultationFeedbacks.Add(feedback);
            await _context.SaveChangesAsync();
            return feedback.FeedbackId;
        }

        public async Task<IReadOnlyList<ConsultationFeedbackDto>> GetSessionFeedbacksAsync(Guid sessionId)
        {
            var feedbacks = await _context.ConsultationFeedbacks
                .Where(f => f.SessionId == sessionId)
                .Include(f => f.User)
                .OrderByDescending(f => f.CreatedAt)
                .ToListAsync();

            return feedbacks.Select(f => new ConsultationFeedbackDto
            {
                FeedbackId = f.FeedbackId,
                SessionId = f.SessionId,
                UserId = f.UserId,
                UserName = f.User?.UserName ?? f.User?.Email ?? string.Empty,
                Rating = f.Rating,
                Comment = f.Comment,
                CreatedAt = f.CreatedAt
            }).ToList();
        }

        // ============================
        // FOLLOW-UP PLAN
        // ============================

        public async Task<Guid> AddFollowUpPlanAsync(CreateFollowUpPlanRequest request)
        {
            var plan = new FollowUpPlan
            {
                FollowUpPlanId = Guid.NewGuid(),
                SessionId = request.SessionId,
                Description = request.Description,
                DueDate = request.DueDate,
                IsCompleted = request.IsCompleted,
            };

            _context.FollowUpPlans.Add(plan);
            await _context.SaveChangesAsync();
            return plan.FollowUpPlanId;
        }

        public async Task<IReadOnlyList<FollowUpPlanDto>> GetFollowUpPlansAsync(Guid sessionId)
        {
            var plans = await _context.FollowUpPlans
                .Where(p => p.SessionId == sessionId)
                .OrderByDescending(p => p.DueDate)
                .ToListAsync();

            return plans.Select(p => new FollowUpPlanDto
            {
                FollowUpPlanId = p.FollowUpPlanId,
                SessionId = p.SessionId,
                Description = p.Description,
                DueDate = p.DueDate,
                IsCompleted = p.IsCompleted
            }).ToList();
        }

        public async Task<bool> UpdateFollowUpPlanStatusAsync(Guid followUpPlanId, bool isCompleted)
        {
            var plan = await _context.FollowUpPlans.FindAsync(followUpPlanId);
            if (plan == null) return false;

            plan.IsCompleted = isCompleted;
            await _context.SaveChangesAsync();
            return true;
        }

        // ============================
        // CONSULTATION REMINDER
        // ============================

        public async Task<Guid> AddReminderAsync(CreateConsultationReminderRequest request)
        {
            var reminder = new ConsultationReminder
            {
                ReminderId = Guid.NewGuid(),
                SessionId = request.SessionId,
                ReminderAt = request.ReminderAt,
                Channel = request.Channel,
                IsSent = false
            };

            _context.ConsultationReminders.Add(reminder);
            await _context.SaveChangesAsync();
            return reminder.ReminderId;
        }

        public async Task<IReadOnlyList<ConsultationReminderDto>> GetSessionRemindersAsync(Guid sessionId)
        {
            var reminders = await _context.ConsultationReminders
                .Where(r => r.SessionId == sessionId)
                .OrderByDescending(r => r.ReminderAt)
                .ToListAsync();

            return reminders.Select(r => new ConsultationReminderDto
            {
                ReminderId = r.ReminderId,
                SessionId = r.SessionId,
                ReminderAt = r.ReminderAt,
                Channel = r.Channel,
                IsSent = r.IsSent
            }).ToList();
        }

        public async Task<bool> MarkReminderSentAsync(Guid reminderId)
        {
            var reminder = await _context.ConsultationReminders.FindAsync(reminderId);
            if (reminder == null) return false;

            reminder.IsSent = true;
            await _context.SaveChangesAsync();
            return true;
        }

        // ============================================================
        // NOTIFICATION HELPERS
        // ============================================================

        private async Task NotifyUserOfScheduledConsultationAsync(Guid userId, Guid sessionId)
        {
            try
            {
                var fullSession = await _context.ConsultationSessions
                    .AsNoTracking()
                    .Include(s => s.Provider).ThenInclude(p => p.User)
                    .FirstOrDefaultAsync(s => s.SessionId == sessionId);

                var providerName = fullSession?.Provider?.User?.UserName
                                   ?? fullSession?.Provider?.User?.Email
                                   ?? "your provider";

                var notification = MindoraNotifications.ConsultationStartingSoon(
                    providerName, 0, sessionId);

                await _dispatcher.DispatchAsync(userId, notification);
            }
            catch
            {
                // Silent fail — session already created
            }
        }

        // ============================
        // PRIVATE MAPPING
        // ============================

        private async Task<ConsultationSessionDto> MapSessionToDtoAsync(ConsultationSession session)
        {
            return new ConsultationSessionDto
            {
                SessionId = session.SessionId,
                AppointmentId = session.AppointmentId,
                ProviderId = session.ProviderId,
                UserId = session.UserId,
                ProviderName = session.Provider?.User?.UserName ?? session.Provider?.User?.Email ?? "Provider",
                UserName = session.User?.UserName ?? session.User?.Email ?? "User",
                SessionType = session.SessionType,
                Status = session.Status,
                ScheduledAt = session.ScheduledAt,
                StartedAt = session.StartedAt,
                EndedAt = session.EndedAt,
                MeetingLink = session.MeetingLink,
                Notes = session.Notes,
                CreatedAt = session.CreatedAt,
                UpdatedAt = session.UpdatedAt,
                NotesCollection = session.NotesCollection?.Select(n => new ConsultationNoteDto
                {
                    NoteId = n.NoteId,
                    SessionId = n.SessionId,
                    ProviderId = n.ProviderId,
                    Content = n.Content,
                    CreatedAt = n.CreatedAt
                }).ToList() ?? new List<ConsultationNoteDto>(),
                Prescriptions = session.Prescriptions?.Select(p => new ConsultationPrescriptionDto
                {
                    PrescriptionId = p.PrescriptionId,
                    SessionId = p.SessionId,
                    Content = p.Content,
                    CreatedAt = p.CreatedAt
                }).ToList() ?? new List<ConsultationPrescriptionDto>(),
                Feedbacks = session.Feedbacks?.Select(f => new ConsultationFeedbackDto
                {
                    FeedbackId = f.FeedbackId,
                    SessionId = f.SessionId,
                    UserId = f.UserId,
                    UserName = f.User?.UserName ?? f.User?.Email ?? "User",
                    Rating = f.Rating,
                    Comment = f.Comment,
                    CreatedAt = f.CreatedAt
                }).ToList() ?? new List<ConsultationFeedbackDto>(),
                FollowUpPlans = session.FollowUpPlans?.Select(p => new FollowUpPlanDto
                {
                    FollowUpPlanId = p.FollowUpPlanId,
                    SessionId = p.SessionId,
                    Description = p.Description,
                    DueDate = p.DueDate,
                    IsCompleted = p.IsCompleted
                }).ToList() ?? new List<FollowUpPlanDto>()
            };
        }
    }
}