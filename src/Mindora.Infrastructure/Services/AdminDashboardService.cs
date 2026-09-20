using Microsoft.EntityFrameworkCore;
using Mindora.Application.DTOs.AdminDashboard;
using Mindora.Application.Interfaces;
using Mindora.Infrastructure.Persistence.DbContext;

namespace Mindora.Infrastructure.Services
{
    public class AdminDashboardService : IAdminDashboardService
    {
        private readonly MindoraDbContext _context;

        public AdminDashboardService(MindoraDbContext context)
        {
            _context = context;
        }

        public async Task<AdminDashboardOverviewDto> GetOverviewAsync()
        {
            var totalUsers = await _context.Users.CountAsync();
            var totalProfessionals = await _context.ProfessionalProviders
                .CountAsync(p => p.IsActive);
            var totalAppointments = await _context.Appointments.CountAsync();
            var totalCommunityPosts = await _context.CommunityPosts
                .CountAsync(p => !p.IsDeleted);
            var totalAssessmentsTaken = await _context.UserAssessments
                .CountAsync(ua => ua.Status == "Completed");
            var totalAIConversations = await _context.AIWellnessCoachSessions.CountAsync();
            var totalRevenue = await _context.Payments
                .Where(p => p.Status == "Completed")
                .SumAsync(p => (decimal?)p.Amount) ?? 0;

            return new AdminDashboardOverviewDto
            {
                TotalUsers = totalUsers,
                TotalProfessionals = totalProfessionals,
                TotalAppointments = totalAppointments,
                TotalCommunityPosts = totalCommunityPosts,
                TotalAssessmentsTaken = totalAssessmentsTaken,
                TotalAIConversations = totalAIConversations,
                TotalRevenue = totalRevenue
            };
        }
    }
}