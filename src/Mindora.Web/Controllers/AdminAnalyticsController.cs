using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mindora.Application.DTOs.AdminDashboard;
using Mindora.Application.Interfaces;

namespace Mindora.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminAnalyticsController : Controller
    {
        private readonly IAdminAnalyticsService _analyticsService;

        public AdminAnalyticsController(IAdminAnalyticsService analyticsService)
        {
            _analyticsService = analyticsService;
        }

        // ============================================================
        // GET: /AdminAnalytics
        // ============================================================
        public async Task<IActionResult> Index()
        {
            var dto = await _analyticsService.GetAnalyticsAsync();

            var vm = new AdminAnalyticsViewModel
            {
                TotalUsers = dto.TotalUsers,
                TotalActiveUsers = dto.TotalActiveUsers,
                TotalProfessionals = dto.TotalProfessionals,
                TotalAppointments = dto.TotalAppointments,
                TotalCommunityPosts = dto.TotalCommunityPosts,
                TotalJournalEntries = dto.TotalJournalEntries,
                TotalHabitsCreated = dto.TotalHabitsCreated,
                TotalAssessmentsTaken = dto.TotalAssessmentsTaken,
                TotalAIConversations = dto.TotalAIConversations,
                TotalRevenue = dto.TotalRevenue,
                MonthlyRevenue = dto.MonthlyRevenue,
                SubscriptionRevenue = dto.SubscriptionRevenue,
                ConsultationRevenue = dto.ConsultationRevenue,
                ActiveSubscriptions = dto.ActiveSubscriptions,
                ExpiredSubscriptions = dto.ExpiredSubscriptions,
                HabitCompletionRate = dto.HabitCompletionRate,
                AssessmentCompletionRate = dto.AssessmentCompletionRate
            };

            // ============================================================
            // ENGAGEMENT BREAKDOWN
            // ============================================================
            vm.EngagementBreakdown = new List<CategoryCount>
            {
                new() { Name = "Community", Count = dto.TotalCommunityPosts },
                new() { Name = "Journal", Count = dto.TotalJournalEntries },
                new() { Name = "Habits", Count = dto.TotalHabitsCreated },
                new() { Name = "Assessments", Count = dto.TotalAssessmentsTaken },
                new() { Name = "AI Coach", Count = dto.TotalAIConversations }
            };

            // ============================================================
            // REVENUE SPLIT
            // ============================================================
            vm.RevenueSplit = new List<CategoryCount>
            {
                new() { Name = "Subscriptions", Amount = dto.SubscriptionRevenue },
                new() { Name = "Consultations", Amount = dto.ConsultationRevenue }
            };

            // ============================================================
            // SUBSCRIPTION STATUS
            // ============================================================
            vm.SubscriptionStatus = new List<CategoryCount>
            {
                new() { Name = "Active", Count = dto.ActiveSubscriptions },
                new() { Name = "Expired", Count = dto.ExpiredSubscriptions }
            };

            // ============================================================
            // CONVERSION RATES
            // ============================================================
            vm.ConversionRates = new List<RatePoint>
            {
                new() { Label = "Habit Completion", Value = dto.HabitCompletionRate },
                new() { Label = "Assessment Completion", Value = dto.AssessmentCompletionRate }
            };

            return View(vm);
        }
    }
}