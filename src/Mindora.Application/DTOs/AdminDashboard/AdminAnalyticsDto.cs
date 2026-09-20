namespace Mindora.Application.DTOs.AdminDashboard
{
    public class AdminAnalyticsDto
    {
        // User & Engagement
        public int TotalUsers { get; set; }
        public int TotalActiveUsers { get; set; }
        public int TotalProfessionals { get; set; }
        public int TotalAppointments { get; set; }

        // Content & Activity
        public int TotalCommunityPosts { get; set; }
        public int TotalJournalEntries { get; set; }
        public int TotalHabitsCreated { get; set; }
        public int TotalAssessmentsTaken { get; set; }
        public int TotalAIConversations { get; set; }

        // Revenue
        public decimal TotalRevenue { get; set; }
        public decimal MonthlyRevenue { get; set; }
        public decimal SubscriptionRevenue { get; set; }
        public decimal ConsultationRevenue { get; set; }

        // Subscription
        public int ActiveSubscriptions { get; set; }
        public int ExpiredSubscriptions { get; set; }

        // Engagement Rates
        public double HabitCompletionRate { get; set; }
        public double AssessmentCompletionRate { get; set; }
    }
}