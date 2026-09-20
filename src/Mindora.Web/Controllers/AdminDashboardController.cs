using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Mindora.Application.DTOs.AdminDashboard;
using Mindora.Application.Interfaces;
using Mindora.Infrastructure.Persistence.DbContext;

namespace Mindora.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminDashboardController : Controller
    {
        private readonly IAdminDashboardService _dashboardService;
        private readonly MindoraDbContext _context;

        public AdminDashboardController(
            IAdminDashboardService dashboardService,
            MindoraDbContext context)
        {
            _dashboardService = dashboardService;
            _context = context;
        }

        // GET: /AdminDashboard
        public async Task<IActionResult> Index()
        {
            var dto = await _dashboardService.GetOverviewAsync();

            var vm = new AdminDashboardViewModel
            {
                TotalUsers = dto.TotalUsers,
                TotalProfessionals = dto.TotalProfessionals,
                TotalAppointments = dto.TotalAppointments,
                TotalCommunityPosts = dto.TotalCommunityPosts,
                TotalAssessmentsTaken = dto.TotalAssessmentsTaken,
                TotalAIConversations = dto.TotalAIConversations,
                TotalRevenue = dto.TotalRevenue
            };

            var today = DateTime.UtcNow.Date;
            var start = today.AddDays(-29);

            // ============================================================
            // USER GROWTH (last 30 days)
            // ============================================================
            var userCounts = await _context.Users
                .Where(u => u.CreatedAt >= start)
                .GroupBy(u => u.CreatedAt.Date)
                .Select(g => new { Date = g.Key, Count = g.Count() })
                .ToListAsync();

            for (int i = 29; i >= 0; i--)
            {
                var d = today.AddDays(-i);
                var count = userCounts.FirstOrDefault(x => x.Date == d)?.Count ?? 0;
                vm.UserGrowth.Add(new DailyPoint { Date = d, Value = count });
            }

            // ============================================================
            // REVENUE TREND (last 30 days)
            // ============================================================
            var revenueData = await _context.Payments
                .Where(p => p.CreatedAt >= start && p.Status == "Completed")
                .GroupBy(p => p.CreatedAt.Date)
                .Select(g => new { Date = g.Key, Total = g.Sum(p => p.Amount) })
                .ToListAsync();

            for (int i = 29; i >= 0; i--)
            {
                var d = today.AddDays(-i);
                var total = revenueData.FirstOrDefault(x => x.Date == d)?.Total ?? 0;
                vm.RevenueTrend.Add(new DailyPoint { Date = d, Value = total });
            }

            // ============================================================
            // CONTENT DISTRIBUTION
            // ============================================================
            vm.ContentDistribution = new List<CategoryCount>
            {
                new() { Name = "Community Posts", Count = dto.TotalCommunityPosts },
                new() { Name = "Assessments", Count = dto.TotalAssessmentsTaken },
                new() { Name = "AI Chats", Count = dto.TotalAIConversations },
                new() { Name = "Appointments", Count = dto.TotalAppointments }
            };

            // ============================================================
            // REVENUE BREAKDOWN
            // ============================================================
            var subscriptionRevenue = await _context.Payments
                .Where(p => p.Status == "Completed" && p.PaymentType == "Subscription")
                .SumAsync(p => (decimal?)p.Amount) ?? 0;

            var consultationRevenue = await _context.Payments
                .Where(p => p.Status == "Completed" && p.PaymentType == "Consultation")
                .SumAsync(p => (decimal?)p.Amount) ?? 0;

            vm.RevenueBreakdown = new List<CategoryCount>
            {
                new() { Name = "Subscriptions", Amount = subscriptionRevenue, Count = 0 },
                new() { Name = "Consultations", Amount = consultationRevenue, Count = 0 }
            };

            return View(vm);
        }
    }
}