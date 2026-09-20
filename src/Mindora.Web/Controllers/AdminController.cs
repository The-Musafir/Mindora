using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Mindora.Infrastructure.Persistence.DbContext;
using Mindora.Application.Interfaces;

namespace Mindora.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly MindoraDbContext _context;
        private readonly IAdminDashboardService _dashboardService;

        public AdminController(
            MindoraDbContext context,
            IAdminDashboardService dashboardService)
        {
            _context = context;
            _dashboardService = dashboardService;
        }

        // ============================================================
        // GET: /Admin  (Admin Hub)
        // ============================================================
        public async Task<IActionResult> Index()
        {
            ViewData["Title"] = "Admin Panel";

            // Quick stats
            var overview = await _dashboardService.GetOverviewAsync();

            ViewBag.TotalUsers = overview.TotalUsers;
            ViewBag.TotalPosts = overview.TotalCommunityPosts;
            ViewBag.TotalRevenue = overview.TotalRevenue;
            ViewBag.TotalProfessionals = overview.TotalProfessionals;

            // Pending flags
            ViewBag.PendingFlags = await _context.CommunityModerationFlags
                .Where(f => f.Status == "Pending")
                .CountAsync();

            // Pending verification docs
            ViewBag.PendingVerifications = await _context.ProviderVerificationDocuments
                .Where(d => d.Status == "Pending")
                .CountAsync();

            // Recent users (last 5)
            ViewBag.RecentUsers = await _context.Users
                .Where(u => !u.IsDeleted)
                .OrderByDescending(u => u.CreatedAt)
                .Take(5)
                .Select(u => new
                {
                    u.Id,
                    Name = u.UserName ?? u.Email,
                    u.Email,
                    u.CreatedAt
                })
                .ToListAsync();

            return View();
        }
    }
}