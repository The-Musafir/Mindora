namespace Mindora.Application.DTOs.Boredom
{
    public class BoredomActivityDto
    {
        public Guid ActivityId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Category { get; set; } = string.Empty;
        public int? DurationMinutes { get; set; }

        public string Emoji => Category switch
        {
            "Relaxing" => "🧘",
            "Productive" => "⚡",
            "Fun" => "🎉",
            "Meaningful" => "💗",
            "Physical" => "🏃",
            "Social" => "💬",
            "Creative" => "🎨",
            _ => "✨"
        };

        public string CategoryColor => Category switch
        {
            "Relaxing" => "#06B6D4",
            "Productive" => "#8B5CF6",
            "Fun" => "#F59E0B",
            "Meaningful" => "#EC4899",
            "Physical" => "#10B981",
            "Social" => "#3B82F6",
            "Creative" => "#F97316",
            _ => "#64748B"
        };
    }

    public class BoredomSessionDto
    {
        public Guid SessionId { get; set; }
        public Guid ActivityId { get; set; }
        public string ActivityTitle { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public DateTime StartedAt { get; set; }
        public DateTime? EndedAt { get; set; }
        public int? MoodBefore { get; set; }
        public int? MoodAfter { get; set; }

        public bool IsCompleted => EndedAt.HasValue;
        public int? MoodImprovement => (MoodAfter.HasValue && MoodBefore.HasValue)
            ? MoodAfter.Value - MoodBefore.Value
            : null;
    }

    public class BoredomCategoryDto
    {
        public string Name { get; set; } = string.Empty;
        public int Count { get; set; }
    }

    public class BoredomStatsDto
    {
        public int TotalSessions { get; set; }
        public int CompletedSessions { get; set; }
        public double? AverageMoodImprovement { get; set; }
        public string? FavoriteCategory { get; set; }
    }
}