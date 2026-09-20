namespace Mindora.Application.DTOs.AdminDashboard
{
    public class AdminDashboardViewModel
    {
        // ============================================================
        // BASE STATS
        // ============================================================
        public int TotalUsers { get; set; }
        public int TotalProfessionals { get; set; }
        public int TotalAppointments { get; set; }
        public int TotalCommunityPosts { get; set; }
        public int TotalAssessmentsTaken { get; set; }
        public int TotalAIConversations { get; set; }
        public decimal TotalRevenue { get; set; }

        // ============================================================
        // CHART DATA
        // ============================================================
        public List<DailyPoint> UserGrowth { get; set; } = new();
        public List<DailyPoint> RevenueTrend { get; set; } = new();
        public List<CategoryCount> ContentDistribution { get; set; } = new();
        public List<CategoryCount> RevenueBreakdown { get; set; } = new();

        // ============================================================
        // COMPUTED
        // ============================================================
        public string TotalRevenueDisplay => $"৳{TotalRevenue:N0}";
        public int TotalContent =>
            TotalCommunityPosts + TotalAssessmentsTaken + TotalAIConversations;
    }

    public class DailyPoint
    {
        public DateTime Date { get; set; }
        public decimal Value { get; set; }
    }

    public class CategoryCount
    {
        public string Name { get; set; } = string.Empty;
        public int Count { get; set; }
        public decimal Amount { get; set; }
    }
}