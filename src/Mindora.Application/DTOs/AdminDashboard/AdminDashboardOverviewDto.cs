namespace Mindora.Application.DTOs.AdminDashboard
{
    public class AdminDashboardOverviewDto
    {
        public int TotalUsers { get; set; }
        public int TotalProfessionals { get; set; }
        public int TotalAppointments { get; set; }
        public int TotalCommunityPosts { get; set; }
        public int TotalAssessmentsTaken { get; set; }
        public int TotalAIConversations { get; set; }
        public decimal TotalRevenue { get; set; }
    }
}