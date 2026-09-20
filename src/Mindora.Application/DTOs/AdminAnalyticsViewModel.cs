namespace Mindora.Application.DTOs.AdminDashboard
{
    public class AdminAnalyticsViewModel
    {
        // ============================================================
        // BASE STATS (from DTO)
        // ============================================================
        public int TotalUsers { get; set; }
        public int TotalActiveUsers { get; set; }
        public int TotalProfessionals { get; set; }
        public int TotalAppointments { get; set; }
        public int TotalCommunityPosts { get; set; }
        public int TotalJournalEntries { get; set; }
        public int TotalHabitsCreated { get; set; }
        public int TotalAssessmentsTaken { get; set; }
        public int TotalAIConversations { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal MonthlyRevenue { get; set; }
        public decimal SubscriptionRevenue { get; set; }
        public decimal ConsultationRevenue { get; set; }
        public int ActiveSubscriptions { get; set; }
        public int ExpiredSubscriptions { get; set; }
        public double HabitCompletionRate { get; set; }
        public double AssessmentCompletionRate { get; set; }

        // ============================================================
        // CHART DATA
        // ============================================================
        public List<CategoryCount> EngagementBreakdown { get; set; } = new();
        public List<CategoryCount> RevenueSplit { get; set; } = new();
        public List<CategoryCount> SubscriptionStatus { get; set; } = new();
        public List<RatePoint> ConversionRates { get; set; } = new();

        // ============================================================
        // COMPUTED
        // ============================================================
        public string TotalRevenueDisplay => $"৳{TotalRevenue:N0}";
        public string MonthlyRevenueDisplay => $"৳{MonthlyRevenue:N0}";
        public int TotalEngagement =>
            TotalCommunityPosts + TotalJournalEntries + TotalHabitsCreated +
            TotalAssessmentsTaken + TotalAIConversations;
    }

    public class RatePoint
    {
        public string Label { get; set; } = string.Empty;
        public double Value { get; set; }
    }
}