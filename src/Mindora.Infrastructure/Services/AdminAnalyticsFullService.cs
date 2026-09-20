using Microsoft.EntityFrameworkCore;
using Mindora.Application.DTOs.Analytics;
using Mindora.Application.Interfaces;
using Mindora.Infrastructure.Persistence.DbContext;

namespace Mindora.Infrastructure.Services
{
    public class AdminAnalyticsFullService : IAdminAnalyticsFullService
    {
        private readonly MindoraDbContext _context;

        public AdminAnalyticsFullService(MindoraDbContext context)
        {
            _context = context;
        }

        public async Task<AdminAnalyticsFullDto> GetFullAnalyticsAsync()
        {
            var now = DateTime.UtcNow;
            var monthStart = new DateTime(now.Year, now.Month, 1);
            var yearStart = new DateTime(now.Year, 1, 1);
            var weekAgo = now.AddDays(-7);

            // ============ USERS ============
            var totalUsers = await _context.Users.CountAsync();
            var newUsersThisMonth = await _context.Users.CountAsync(u => u.CreatedAt >= monthStart);

            var activeUsersThisWeek = await _context.Users
                .Where(u => _context.UserLoginHistories
                    .Any(l => l.UserId == u.Id && l.LoginAt >= weekAgo))
                .CountAsync();

            var activeUsersThisMonth = await _context.Users
                .Where(u => _context.UserLoginHistories
                    .Any(l => l.UserId == u.Id && l.LoginAt >= monthStart))
                .CountAsync();

            // ============ REVENUE ============
            var completedPayments = await _context.Payments
                .Where(p => p.Status == "Completed")
                .ToListAsync();

            var totalRevenue = completedPayments.Sum(p => p.Amount);
            var monthlyRevenue = completedPayments.Where(p => p.CreatedAt >= monthStart).Sum(p => p.Amount);
            var yearlyRevenue = completedPayments.Where(p => p.CreatedAt >= yearStart).Sum(p => p.Amount);
            var arpu = totalUsers > 0 ? totalRevenue / totalUsers : 0;

            // ============ SUBSCRIPTIONS ============
            var activeSubs = await _context.UserSubscriptions
                .CountAsync(s => s.Status == "Active" && s.EndDate > now);
            var newSubsThisMonth = await _context.UserSubscriptions
                .CountAsync(s => s.StartDate >= monthStart);
            var cancelledSubsThisMonth = await _context.UserSubscriptions
                .CountAsync(s => s.Status == "Cancelled" && s.CreatedAt >= monthStart);

            // ============ CONTENT ============
            var totalHabits = await _context.Habits.CountAsync(h => !h.IsArchived);
            var totalJournalEntries = await _context.JournalEntries.CountAsync(j => !j.IsDeleted);
            var totalAssessments = await _context.UserAssessments.CountAsync(a => a.Status == "Completed");
            var totalCommunityPosts = await _context.CommunityPosts.CountAsync(p => !p.IsDeleted);
            var totalAIConversations = await _context.AIWellnessCoachSessions.CountAsync();

            // ============ PROFESSIONALS ============
            var totalProfessionals = await _context.ProfessionalProviders.CountAsync(p => p.IsActive);
            var totalAppointments = await _context.Appointments.CountAsync();
            var appointmentsThisMonth = await _context.Appointments
                .CountAsync(a => a.CreatedAt >= monthStart);

            // ============ TRENDS ============
            var userGrowthTrend = new List<UserGrowthTrendDto>();
            for (int i = 29; i >= 0; i--)
            {
                var day = now.Date.AddDays(-i);
                var nextDay = day.AddDays(1);
                var newUsers = await _context.Users
                    .CountAsync(u => u.CreatedAt >= day && u.CreatedAt < nextDay);
                var totalAtDay = await _context.Users
                    .CountAsync(u => u.CreatedAt < nextDay);

                userGrowthTrend.Add(new UserGrowthTrendDto
                {
                    Date = day,
                    NewUsers = newUsers,
                    TotalUsers = totalAtDay
                });
            }

            var revenueTrend = new List<RevenueTrendDto>();
            for (int i = 29; i >= 0; i--)
            {
                var day = now.Date.AddDays(-i);
                var nextDay = day.AddDays(1);
                var dayPayments = completedPayments
                    .Where(p => p.CreatedAt >= day && p.CreatedAt < nextDay)
                    .ToList();

                revenueTrend.Add(new RevenueTrendDto
                {
                    Date = day,
                    Revenue = dayPayments.Sum(p => p.Amount),
                    PaymentCount = dayPayments.Count
                });
            }

            return new AdminAnalyticsFullDto
            {
                TotalUsers = totalUsers,
                NewUsersThisMonth = newUsersThisMonth,
                ActiveUsersThisWeek = activeUsersThisWeek,
                ActiveUsersThisMonth = activeUsersThisMonth,
                TotalRevenue = totalRevenue,
                MonthlyRevenue = monthlyRevenue,
                YearlyRevenue = yearlyRevenue,
                AverageRevenuePerUser = arpu,
                ActiveSubscriptions = activeSubs,
                NewSubscriptionsThisMonth = newSubsThisMonth,
                CancelledSubscriptionsThisMonth = cancelledSubsThisMonth,
                TotalHabitsCreated = totalHabits,
                TotalJournalEntries = totalJournalEntries,
                TotalAssessmentsTaken = totalAssessments,
                TotalCommunityPosts = totalCommunityPosts,
                TotalAIConversations = totalAIConversations,
                TotalProfessionals = totalProfessionals,
                TotalAppointments = totalAppointments,
                AppointmentsThisMonth = appointmentsThisMonth,
                UserGrowthTrend = userGrowthTrend,
                RevenueTrend = revenueTrend
            };
        }
    }
}