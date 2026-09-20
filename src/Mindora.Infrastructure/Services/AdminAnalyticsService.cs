using Microsoft.EntityFrameworkCore;
using Mindora.Application.DTOs.AdminDashboard;
using Mindora.Application.Interfaces;
using Mindora.Infrastructure.Persistence.DbContext;

namespace Mindora.Infrastructure.Services
{
    public class AdminAnalyticsService : IAdminAnalyticsService
    {
        private readonly MindoraDbContext _context;

        public AdminAnalyticsService(MindoraDbContext context)
        {
            _context = context;
        }

        public async Task<AdminAnalyticsDto> GetAnalyticsAsync()
        {
            var now = DateTime.UtcNow;
            var monthStart = new DateTime(now.Year, now.Month, 1);
            var yearStart = new DateTime(now.Year, 1, 1);

            // Users
            var totalUsers = await _context.Users.CountAsync();
            var activeUsers = await _context.Users.CountAsync(u => u.IsActive && !u.IsDeleted);
            var totalProfessionals = await _context.ProfessionalProviders.CountAsync(p => p.IsActive);

            // Content
            var totalCommunityPosts = await _context.CommunityPosts.CountAsync(p => !p.IsDeleted);
            var totalJournalEntries = await _context.JournalEntries.CountAsync(j => !j.IsDeleted);
            var totalHabitsCreated = await _context.Habits.CountAsync(h => !h.IsArchived);
            var totalAssessmentsTaken = await _context.UserAssessments.CountAsync(ua => ua.Status == "Completed");
            var totalAIConversations = await _context.AIWellnessCoachSessions.CountAsync();

            // Appointments
            var totalAppointments = await _context.Appointments.CountAsync();

            // Revenue
            var completedPayments = await _context.Payments
                .Where(p => p.Status == "Completed")
                .ToListAsync();

            var totalRevenue = completedPayments.Sum(p => p.Amount);
            var monthlyRevenue = completedPayments
                .Where(p => p.CreatedAt >= monthStart)
                .Sum(p => p.Amount);
            var subscriptionRevenue = completedPayments
                .Where(p => p.PaymentType == "Subscription")
                .Sum(p => p.Amount);
            var consultationRevenue = completedPayments
                .Where(p => p.PaymentType == "Consultation")
                .Sum(p => p.Amount);

            // Subscriptions
            var activeSubscriptions = await _context.UserSubscriptions
                .CountAsync(s => s.Status == "Active" && s.EndDate > now);
            var expiredSubscriptions = await _context.UserSubscriptions
                .CountAsync(s => s.Status == "Expired" || (s.Status == "Active" && s.EndDate <= now));

            // Habit completion rate
            var totalHabitLogs = await _context.HabitLogs.CountAsync();
            var completedHabitLogs = await _context.HabitLogs.CountAsync(l => l.IsCompleted);
            double habitCompletionRate = totalHabitLogs > 0
                ? (double)completedHabitLogs / totalHabitLogs * 100
                : 0;

            // Assessment completion rate
            var totalAssessments = await _context.UserAssessments.CountAsync();
            double assessmentCompletionRate = totalAssessments > 0
                ? (double)totalAssessmentsTaken / totalAssessments * 100
                : 0;

            return new AdminAnalyticsDto
            {
                TotalUsers = totalUsers,
                TotalActiveUsers = activeUsers,
                TotalProfessionals = totalProfessionals,
                TotalAppointments = totalAppointments,
                TotalCommunityPosts = totalCommunityPosts,
                TotalJournalEntries = totalJournalEntries,
                TotalHabitsCreated = totalHabitsCreated,
                TotalAssessmentsTaken = totalAssessmentsTaken,
                TotalAIConversations = totalAIConversations,
                TotalRevenue = totalRevenue,
                MonthlyRevenue = monthlyRevenue,
                SubscriptionRevenue = subscriptionRevenue,
                ConsultationRevenue = consultationRevenue,
                ActiveSubscriptions = activeSubscriptions,
                ExpiredSubscriptions = expiredSubscriptions,
                HabitCompletionRate = habitCompletionRate,
                AssessmentCompletionRate = assessmentCompletionRate
            };
        }
    }
}