namespace Mindora.Application.DTOs.Home
{
    public class HomeViewModel
    {
        // Statistics (public)
        public int TotalMembers { get; set; }
        public int TotalHabitsCompleted { get; set; }
        public int TotalSessions { get; set; }
        public int PositiveProgressRate { get; set; } = 92;

        // Top providers (public)
        public List<LandingProviderDto> TopProviders { get; set; } = new();

        // Testimonials (public)
        public List<LandingTestimonialDto> Testimonials { get; set; } = new();

        // Today's Progress (only if logged in)
        public bool IsUserLoggedIn { get; set; }
        public int? TodayHabitsDone { get; set; }
        public int? TodayHabitsTotal { get; set; }
        public int? DailyGoalPercent { get; set; }
        public double? LatestMoodScore { get; set; }
        public int? CurrentStreak { get; set; }

        // Formatted displays
        public string MembersDisplay => FormatCount(TotalMembers);
        public string HabitsDisplay => FormatCount(TotalHabitsCompleted);
        public string SessionsDisplay => FormatCount(TotalSessions);

        private static string FormatCount(int count)
        {
            if (count >= 1000000) return $"{count / 1000000.0:F1}M+";
            if (count >= 1000) return $"{count / 1000.0:F1}K+";
            return count.ToString();
        }
    }

    public class LandingProviderDto
    {
        public Guid ProviderId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Specialty { get; set; }
        public double? Rating { get; set; }
        public int ReviewCount { get; set; }
        public int YearsOfExperience { get; set; }
        public string Initial =>
            !string.IsNullOrEmpty(Name) ? Name.Substring(0, 1).ToUpper() : "P";
    }

    public class LandingTestimonialDto
    {
        public string ReviewerName { get; set; } = string.Empty;
        public string Comment { get; set; } = string.Empty;
        public int Rating { get; set; }
        public string Initial =>
            !string.IsNullOrEmpty(ReviewerName) ? ReviewerName.Substring(0, 1).ToUpper() : "U";
    }
}