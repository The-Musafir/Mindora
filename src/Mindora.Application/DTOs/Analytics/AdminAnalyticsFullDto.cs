namespace Mindora.Application.DTOs.Analytics
{
    public class AdminAnalyticsFullDto
    {
        // User Metrics
        public int TotalUsers { get; set; }
        public int NewUsersThisMonth { get; set; }
        public int ActiveUsersThisWeek { get; set; }
        public int ActiveUsersThisMonth { get; set; }

        // Revenue Metrics
        public decimal TotalRevenue { get; set; }
        public decimal MonthlyRevenue { get; set; }
        public decimal YearlyRevenue { get; set; }
        public decimal AverageRevenuePerUser { get; set; }

        // Subscription Metrics
        public int ActiveSubscriptions { get; set; }
        public int NewSubscriptionsThisMonth { get; set; }
        public int CancelledSubscriptionsThisMonth { get; set; }

        // Content Metrics
        public int TotalHabitsCreated { get; set; }
        public int TotalJournalEntries { get; set; }
        public int TotalAssessmentsTaken { get; set; }
        public int TotalCommunityPosts { get; set; }
        public int TotalAIConversations { get; set; }

        // Professional Metrics
        public int TotalProfessionals { get; set; }
        public int TotalAppointments { get; set; }
        public int AppointmentsThisMonth { get; set; }

        // Trend Data
        public List<UserGrowthTrendDto> UserGrowthTrend { get; set; } = new();
        public List<RevenueTrendDto> RevenueTrend { get; set; } = new();
    }
}