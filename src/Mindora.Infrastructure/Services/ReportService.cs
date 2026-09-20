using Microsoft.EntityFrameworkCore;
using System.Text;
using Mindora.Application.DTOs.Analytics;
using Mindora.Application.Interfaces;
using Mindora.Domain.Entities;
using Mindora.Infrastructure.Persistence.DbContext;

namespace Mindora.Infrastructure.Services
{
    public class ReportService : IReportService
    {
        private readonly MindoraDbContext _context;
        private readonly IUserAnalyticsService _userAnalyticsService;
        private readonly IAdminAnalyticsFullService _adminAnalyticsService;

        public ReportService(
            MindoraDbContext context,
            IUserAnalyticsService userAnalyticsService,
            IAdminAnalyticsFullService adminAnalyticsService)
        {
            _context = context;
            _userAnalyticsService = userAnalyticsService;
            _adminAnalyticsService = adminAnalyticsService;
        }

        public async Task<Guid> RequestReportAsync(Guid userId, CreateReportRequestDto request)
        {
            var report = new ReportRequest
            {
                ReportId = Guid.NewGuid(),
                RequestedByUserId = userId,
                ReportType = request.ReportType,
                Format = request.Format,
                FromDate = request.FromDate,
                ToDate = request.ToDate,
                Status = "Pending",
                RequestedAt = DateTime.UtcNow
            };

            _context.ReportRequests.Add(report);
            await _context.SaveChangesAsync();
            return report.ReportId;
        }

        public async Task<IReadOnlyList<ReportRequestDto>> GetUserReportsAsync(Guid userId)
        {
            var reports = await _context.ReportRequests
                .Where(r => r.RequestedByUserId == userId)
                .Include(r => r.RequestedByUser)
                .OrderByDescending(r => r.RequestedAt)
                .ToListAsync();

            return reports.Select(MapToDto).ToList();
        }

        public async Task<IReadOnlyList<ReportRequestDto>> GetAllReportsAsync()
        {
            var reports = await _context.ReportRequests
                .Include(r => r.RequestedByUser)
                .OrderByDescending(r => r.RequestedAt)
                .ToListAsync();

            return reports.Select(MapToDto).ToList();
        }

        public async Task<ReportRequestDto?> GetReportByIdAsync(Guid reportId)
        {
            var report = await _context.ReportRequests
                .Include(r => r.RequestedByUser)
                .FirstOrDefaultAsync(r => r.ReportId == reportId);

            return report == null ? null : MapToDto(report);
        }

        public async Task<bool> UpdateReportStatusAsync(Guid reportId, string status, string? fileUrl = null)
        {
            var report = await _context.ReportRequests.FindAsync(reportId);
            if (report == null) return false;

            report.Status = status;
            if (!string.IsNullOrWhiteSpace(fileUrl))
                report.FileUrl = fileUrl;

            if (status == "Completed")
                report.CompletedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<byte[]> GenerateUserAnalyticsCsvAsync(Guid userId, DateTime? fromDate, DateTime? toDate)
        {
            var analytics = await _userAnalyticsService.GetUserAnalyticsAsync(userId);

            var sb = new StringBuilder();
            sb.AppendLine("Metric,Value");
            sb.AppendLine($"Total Habits,{analytics.TotalHabits}");
            sb.AppendLine($"Active Habits,{analytics.ActiveHabits}");
            sb.AppendLine($"Completed Habit Logs,{analytics.CompletedHabitLogs}");
            sb.AppendLine($"Total Habit Logs,{analytics.TotalHabitLogs}");
            sb.AppendLine($"Habit Completion Rate (%),{analytics.HabitCompletionRate:F2}");
            sb.AppendLine($"Longest Streak,{analytics.CurrentLongestStreak}");
            sb.AppendLine($"Total Relapses,{analytics.TotalRelapses}");
            sb.AppendLine($"Total Journal Entries,{analytics.TotalJournalEntries}");
            sb.AppendLine($"Total Assessments Taken,{analytics.TotalAssessmentsTaken}");
            sb.AppendLine($"Average Assessment Score,{analytics.AverageAssessmentScore:F2}");
            sb.AppendLine($"Total AI Conversations,{analytics.TotalAIConversations}");
            sb.AppendLine($"AI Messages Sent,{analytics.AIMessagesSent}");
            sb.AppendLine($"Latest Wellness Score,{analytics.LatestWellnessScore:F2}");
            sb.AppendLine($"Average Mood Score,{analytics.AverageMoodScore:F2}");
            sb.AppendLine();
            sb.AppendLine("Date,Mood Score,Journal Entries,Completed Habits,Total Habits,Completion Rate (%)");
            foreach (var day in analytics.HabitTrend)
            {
                var mood = analytics.MoodTrend.FirstOrDefault(m => m.Date == day.Date);
                sb.AppendLine($"{day.Date:yyyy-MM-dd},{mood?.MoodScore:F2},{mood?.JournalEntries},{day.CompletedHabits},{day.TotalHabits},{day.CompletionRate:F2}");
            }

            return Encoding.UTF8.GetBytes(sb.ToString());
        }

        public async Task<byte[]> GenerateAdminAnalyticsCsvAsync(DateTime? fromDate, DateTime? toDate)
        {
            var analytics = await _adminAnalyticsService.GetFullAnalyticsAsync();

            var sb = new StringBuilder();
            sb.AppendLine("Metric,Value");
            sb.AppendLine($"Total Users,{analytics.TotalUsers}");
            sb.AppendLine($"New Users This Month,{analytics.NewUsersThisMonth}");
            sb.AppendLine($"Active Users This Week,{analytics.ActiveUsersThisWeek}");
            sb.AppendLine($"Total Revenue,{analytics.TotalRevenue:F2}");
            sb.AppendLine($"Monthly Revenue,{analytics.MonthlyRevenue:F2}");
            sb.AppendLine($"Yearly Revenue,{analytics.YearlyRevenue:F2}");
            sb.AppendLine($"ARPU,{analytics.AverageRevenuePerUser:F2}");
            sb.AppendLine($"Active Subscriptions,{analytics.ActiveSubscriptions}");
            sb.AppendLine($"New Subscriptions This Month,{analytics.NewSubscriptionsThisMonth}");
            sb.AppendLine($"Cancelled Subscriptions This Month,{analytics.CancelledSubscriptionsThisMonth}");
            sb.AppendLine($"Total Habits Created,{analytics.TotalHabitsCreated}");
            sb.AppendLine($"Total Journal Entries,{analytics.TotalJournalEntries}");
            sb.AppendLine($"Total Assessments Taken,{analytics.TotalAssessmentsTaken}");
            sb.AppendLine($"Total Community Posts,{analytics.TotalCommunityPosts}");
            sb.AppendLine($"Total AI Conversations,{analytics.TotalAIConversations}");
            sb.AppendLine($"Total Professionals,{analytics.TotalProfessionals}");
            sb.AppendLine($"Total Appointments,{analytics.TotalAppointments}");
            sb.AppendLine();
            sb.AppendLine("Date,New Users,Total Users,Revenue,Payments");
            foreach (var day in analytics.UserGrowthTrend)
            {
                var rev = analytics.RevenueTrend.FirstOrDefault(r => r.Date == day.Date);
                sb.AppendLine($"{day.Date:yyyy-MM-dd},{day.NewUsers},{day.TotalUsers},{rev?.Revenue:F2},{rev?.PaymentCount}");
            }

            return Encoding.UTF8.GetBytes(sb.ToString());
        }

        private ReportRequestDto MapToDto(ReportRequest report)
        {
            return new ReportRequestDto
            {
                ReportId = report.ReportId,
                RequestedByUserId = report.RequestedByUserId,
                RequestedByEmail = report.RequestedByUser?.Email ?? "",
                ReportType = report.ReportType,
                Format = report.Format,
                FromDate = report.FromDate,
                ToDate = report.ToDate,
                Status = report.Status,
                FileUrl = report.FileUrl,
                RequestedAt = report.RequestedAt,
                CompletedAt = report.CompletedAt
            };
        }
    }
}