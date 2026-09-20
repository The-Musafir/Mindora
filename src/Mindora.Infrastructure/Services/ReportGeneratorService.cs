using Microsoft.EntityFrameworkCore;
using Mindora.Application.DTOs.Reporting;
using Mindora.Application.Interfaces;
using Mindora.Infrastructure.Persistence.DbContext;

namespace Mindora.Infrastructure.Services
{
    public class ReportGeneratorService : IReportGeneratorService
    {
        private readonly MindoraDbContext _context;

        public ReportGeneratorService(MindoraDbContext context)
        {
            _context = context;
        }

        public async Task<ReportDataResult> GenerateReportDataAsync(ReportFilterDto filter)
        {
            var fromDate = filter.FromDate ?? DateTime.UtcNow.AddDays(-30);
            var toDate = filter.ToDate ?? DateTime.UtcNow;

            return filter.ReportType switch
            {
                "RevenueReport" => await GenerateRevenueReportAsync(fromDate, toDate),
                "UserGrowthReport" => await GenerateUserGrowthReportAsync(fromDate, toDate),
                "SubscriptionReport" => await GenerateSubscriptionReportAsync(fromDate, toDate),
                "HabitRecoveryReport" => await GenerateHabitRecoveryReportAsync(fromDate, toDate, filter.UserId),
                "MoodTrendReport" => await GenerateMoodTrendReportAsync(fromDate, toDate, filter.UserId),
                "AssessmentSummaryReport" => await GenerateAssessmentReportAsync(fromDate, toDate),
                "CommunityEngagementReport" => await GenerateCommunityReportAsync(fromDate, toDate),
                "AIUsageReport" => await GenerateAIUsageReportAsync(fromDate, toDate),
                "AppointmentReport" => await GenerateAppointmentReportAsync(fromDate, toDate),
                "AuditLogReport" => await GenerateAuditLogReportAsync(fromDate, toDate),
                _ => await GenerateGenericReportAsync(filter.ReportType, fromDate, toDate)
            };
        }

        // ================================
        // REVENUE REPORT
        // ================================
        private async Task<ReportDataResult> GenerateRevenueReportAsync(DateTime from, DateTime to)
        {
            var payments = await _context.Payments
                .Where(p => p.Status == "Completed" &&
                            p.CreatedAt >= from && p.CreatedAt <= to)
                .OrderBy(p => p.CreatedAt)
                .ToListAsync();

            var result = new ReportDataResult
            {
                ReportTitle = "Revenue Report",
                Columns = new List<string>
                {
                    "Date", "Amount", "Currency", "Gateway", "PaymentType", "PaymentMethod", "TransactionId"
                }
            };

            foreach (var p in payments)
            {
                result.Rows.Add(new List<string>
                {
                    p.CreatedAt.ToString("yyyy-MM-dd HH:mm"),
                    p.Amount.ToString("F2"),
                    p.Currency,
                    p.Gateway,
                    p.PaymentType,
                    p.PaymentMethod ?? "N/A",
                    p.TransactionId ?? "N/A"
                });
            }

            return result;
        }

        // ================================
        // USER GROWTH REPORT
        // ================================
        private async Task<ReportDataResult> GenerateUserGrowthReportAsync(DateTime from, DateTime to)
        {
            var users = await _context.Users
                .Where(u => u.CreatedAt >= from && u.CreatedAt <= to)
                .OrderBy(u => u.CreatedAt)
                .ToListAsync();

            var result = new ReportDataResult
            {
                ReportTitle = "User Growth Report",
                Columns = new List<string> { "Date", "Email", "UserName", "Status", "Role" }
            };

            foreach (var u in users)
            {
                var roles = await _context.UserRoles
                    .Where(ur => ur.UserId == u.Id)
                    .Join(_context.Roles,
                          ur => ur.RoleId,
                          r => r.Id,
                          (ur, r) => r.Name)
                    .ToListAsync();

                result.Rows.Add(new List<string>
                {
                    u.CreatedAt.ToString("yyyy-MM-dd HH:mm"),
                    u.Email ?? "",
                    u.UserName ?? "",
                    u.IsActive ? "Active" : "Inactive",
                    string.Join(", ", roles)
                });
            }

            return result;
        }

        // ================================
        // SUBSCRIPTION REPORT
        // ================================
        private async Task<ReportDataResult> GenerateSubscriptionReportAsync(DateTime from, DateTime to)
        {
            var subs = await _context.UserSubscriptions
                .Where(s => s.CreatedAt >= from && s.CreatedAt <= to)
                .Include(s => s.Plan)
                .Include(s => s.User)
                .OrderBy(s => s.CreatedAt)
                .ToListAsync();

            var result = new ReportDataResult
            {
                ReportTitle = "Subscription Report",
                Columns = new List<string>
                {
                    "StartDate", "EndDate", "User", "Plan", "Price", "Status"
                }
            };

            foreach (var s in subs)
            {
                result.Rows.Add(new List<string>
                {
                    s.StartDate.ToString("yyyy-MM-dd"),
                    s.EndDate.ToString("yyyy-MM-dd"),
                    s.User?.Email ?? "",
                    s.Plan?.Name ?? "",
                    s.Plan?.Price.ToString("F2") ?? "0",
                    s.Status
                });
            }

            return result;
        }

        // ================================
        // HABIT RECOVERY REPORT
        // ================================
        private async Task<ReportDataResult> GenerateHabitRecoveryReportAsync(DateTime from, DateTime to, Guid? userId)
        {
            var habits = await _context.Habits
                .Where(h => !h.IsArchived &&
                            (userId == null || h.UserId == userId))
                .ToListAsync();

            var habitIds = habits.Select(h => h.HabitId).ToList();

            // ✅ DateTime.Date ভিত্তিক তুলনা — CS0019 ফিক্স
            var fromDate = from.Date;
            var toDate = to.Date;

            var logs = await _context.HabitLogs
                .Where(l => habitIds.Contains(l.HabitId) &&
                            l.LogDate.Date >= fromDate &&
                            l.LogDate.Date <= toDate)
                .ToListAsync();

            var result = new ReportDataResult
            {
                ReportTitle = "Habit Recovery Report",
                Columns = new List<string> { "Date", "HabitId", "IsCompleted", "Note" }
            };

            foreach (var l in logs)
            {
                result.Rows.Add(new List<string>
                {
                    l.LogDate.ToString("yyyy-MM-dd"),
                    l.HabitId.ToString(),
                    l.IsCompleted ? "Yes" : "No",
                    l.Note ?? ""
                });
            }

            return result;
        }

        // ================================
        // MOOD TREND REPORT
        // ================================
        private async Task<ReportDataResult> GenerateMoodTrendReportAsync(DateTime from, DateTime to, Guid? userId)
        {
            var entries = await _context.JournalEntries
                .Where(j => !j.IsDeleted &&
                            (userId == null || j.UserId == userId) &&
                            j.CreatedAt >= from && j.CreatedAt <= to)
                .ToListAsync();

            var entryIds = entries.Select(e => e.EntryId).ToList();

            var moods = await _context.JournalMoods
                .Where(m => entryIds.Contains(m.EntryId))
                .ToListAsync();

            var result = new ReportDataResult
            {
                ReportTitle = "Mood Trend Report",
                Columns = new List<string> { "Date", "MoodLabel", "Intensity" }
            };

            foreach (var m in moods)
            {
                var entry = entries.FirstOrDefault(e => e.EntryId == m.EntryId);
                result.Rows.Add(new List<string>
                {
                    entry?.CreatedAt.ToString("yyyy-MM-dd") ?? "",
                    m.MoodLabel,
                    m.Intensity.ToString()
                });
            }

            return result;
        }

        // ================================
        // ASSESSMENT SUMMARY REPORT
        // ================================
        private async Task<ReportDataResult> GenerateAssessmentReportAsync(DateTime from, DateTime to)
        {
            var assessments = await _context.UserAssessments
                .Where(a => a.Status == "Completed" &&
                            a.CompletedAt >= from && a.CompletedAt <= to)
                .Include(a => a.Questionnaire)
                .Include(a => a.Result)
                .ToListAsync();

            var result = new ReportDataResult
            {
                ReportTitle = "Assessment Summary Report",
                Columns = new List<string>
                {
                    "CompletedAt", "Questionnaire", "TotalScore", "SeverityLevel"
                }
            };

            foreach (var a in assessments)
            {
                result.Rows.Add(new List<string>
                {
                    a.CompletedAt?.ToString("yyyy-MM-dd HH:mm") ?? "",
                    a.Questionnaire?.Title ?? "",
                    a.Result?.TotalScore.ToString() ?? "N/A",
                    a.Result?.SeverityLevel ?? "N/A"
                });
            }

            return result;
        }

        // ================================
        // COMMUNITY ENGAGEMENT REPORT
        // ================================
        private async Task<ReportDataResult> GenerateCommunityReportAsync(DateTime from, DateTime to)
        {
            var posts = await _context.CommunityPosts
                .Where(p => !p.IsDeleted &&
                            p.CreatedAt >= from && p.CreatedAt <= to)
                .ToListAsync();

            var postIds = posts.Select(p => p.PostId).ToList();

            var comments = await _context.CommunityComments
                .Where(c => postIds.Contains(c.PostId) &&
                            !c.IsDeleted &&
                            c.CreatedAt >= from && c.CreatedAt <= to)
                .ToListAsync();

            var reactions = await _context.CommunityReactions
                .Where(r => r.PostId != null &&
                            postIds.Contains(r.PostId.Value) &&
                            r.CreatedAt >= from && r.CreatedAt <= to)
                .ToListAsync();

            var result = new ReportDataResult
            {
                ReportTitle = "Community Engagement Report",
                Columns = new List<string> { "Date", "Posts", "Comments", "Reactions" }
            };

            var days = Enumerable.Range(0, (to.Date - from.Date).Days + 1)
                .Select(offset => from.Date.AddDays(offset))
                .ToList();

            foreach (var day in days)
            {
                var next = day.AddDays(1);
                result.Rows.Add(new List<string>
                {
                    day.ToString("yyyy-MM-dd"),
                    posts.Count(p => p.CreatedAt >= day && p.CreatedAt < next).ToString(),
                    comments.Count(c => c.CreatedAt >= day && c.CreatedAt < next).ToString(),
                    reactions.Count(r => r.CreatedAt >= day && r.CreatedAt < next).ToString()
                });
            }

            return result;
        }

        // ================================
        // AI USAGE REPORT
        // ================================
        private async Task<ReportDataResult> GenerateAIUsageReportAsync(DateTime from, DateTime to)
        {
            var sessions = await _context.AIWellnessCoachSessions
                .Where(s => s.StartedAt >= from && s.StartedAt <= to)
                .ToListAsync();

            var sessionIds = sessions.Select(s => s.SessionId).ToList();

            var messages = await _context.AIChatMessages
                .Where(m => sessionIds.Contains(m.SessionId))
                .ToListAsync();

            var result = new ReportDataResult
            {
                ReportTitle = "AI Usage Report",
                Columns = new List<string>
                {
                    "SessionId", "UserId", "StartedAt", "MessageCount", "Duration (minutes)"
                }
            };

            foreach (var s in sessions)
            {
                var msgCount = messages.Count(m => m.SessionId == s.SessionId);
                var duration = s.EndedAt.HasValue
                    ? (s.EndedAt.Value - s.StartedAt).TotalMinutes
                    : 0;

                result.Rows.Add(new List<string>
                {
                    s.SessionId.ToString(),
                    s.UserId.ToString(),
                    s.StartedAt.ToString("yyyy-MM-dd HH:mm"),
                    msgCount.ToString(),
                    duration.ToString("F1")
                });
            }

            return result;
        }

        // ================================
        // APPOINTMENT REPORT
        // ================================
        private async Task<ReportDataResult> GenerateAppointmentReportAsync(DateTime from, DateTime to)
        {
            var appointments = await _context.Appointments
                .Where(a => a.CreatedAt >= from && a.CreatedAt <= to)
                .Include(a => a.User)
                .Include(a => a.Service)
                .ToListAsync();

            var result = new ReportDataResult
            {
                ReportTitle = "Appointment Report",
                Columns = new List<string>
                {
                    "Date", "Patient", "Service", "Status", "StartTime", "EndTime"
                }
            };

            foreach (var a in appointments)
            {
                result.Rows.Add(new List<string>
                {
                    a.AppointmentDate.ToString("yyyy-MM-dd"),
                    a.User?.Email ?? "",
                    a.Service?.ServiceName ?? "N/A",
                    a.Status,
                    a.StartTime.ToString(@"hh\:mm"),
                    a.EndTime.ToString(@"hh\:mm")
                });
            }

            return result;
        }

        // ================================
        // AUDIT LOG REPORT
        // ================================
        private async Task<ReportDataResult> GenerateAuditLogReportAsync(DateTime from, DateTime to)
        {
            var logs = await _context.AuditLogs
                .Where(l => l.Timestamp >= from && l.Timestamp <= to)
                .OrderByDescending(l => l.Timestamp)
                .Take(5000)
                .ToListAsync();

            var result = new ReportDataResult
            {
                ReportTitle = "Audit Log Report",
                Columns = new List<string>
                {
                    "Timestamp", "Action", "TableName", "RecordId", "UserId"
                }
            };

            foreach (var l in logs)
            {
                result.Rows.Add(new List<string>
                {
                    l.Timestamp.ToString("yyyy-MM-dd HH:mm"),
                    l.Action,
                    l.TableName,
                    l.RecordId,
                    l.UserId?.ToString() ?? "System"
                });
            }

            return result;
        }

        // ================================
        // GENERIC FALLBACK
        // ================================
        private Task<ReportDataResult> GenerateGenericReportAsync(string reportType, DateTime from, DateTime to)
        {
            var result = new ReportDataResult
            {
                ReportTitle = $"{reportType} Report",
                Columns = new List<string> { "Message" },
                Rows = new List<List<string>>
                {
                    new List<string> { "Generic report generator placeholder." }
                }
            };

            return Task.FromResult(result);
        }
    }
}