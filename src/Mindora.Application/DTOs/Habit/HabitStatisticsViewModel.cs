namespace Mindora.Application.DTOs.Habit
{
    public class HabitStatisticsViewModel
    {
        public Guid HabitId { get; set; }
        public string HabitName { get; set; } = string.Empty;
        public string? CategoryName { get; set; }

        // ============================================================
        // OVERVIEW STATS
        // ============================================================
        public int TotalLogs { get; set; }
        public int CompletedLogs { get; set; }
        public double CompletionRate { get; set; }
        public int CurrentStreak { get; set; }
        public int LongestStreak { get; set; }
        public double WeeklyAverage { get; set; }
        public double MonthlyAverage { get; set; }
        public int TotalRelapses { get; set; }

        // ============================================================
        // CHART DATA
        // ============================================================
        /// <summary>Last 30 days logs (line/area chart).</summary>
        public List<DailyLogPoint> Last30Days { get; set; } = new();

        /// <summary>Last 8 weeks progress (bar chart).</summary>
        public List<WeeklyProgressPoint> Last8Weeks { get; set; } = new();

        /// <summary>Streak heatmap (30-day grid).</summary>
        public List<HeatmapDay> HeatmapDays { get; set; } = new();

        // ============================================================
        // COMPUTED
        // ============================================================
        public string CompletionRateDisplay => $"{CompletionRate:F1}%";
        public string WeeklyAverageDisplay => $"{WeeklyAverage:F1}%";
        public string MonthlyAverageDisplay => $"{MonthlyAverage:F1}%";

        // ============================================================
        // FACTORY
        // ============================================================
        public static HabitStatisticsViewModel Build(
            Guid habitId,
            string habitName,
            string? categoryName,
            HabitStatisticsDto stats,
            IEnumerable<HabitLogDto> allLogs)
        {
            var vm = new HabitStatisticsViewModel
            {
                HabitId = habitId,
                HabitName = habitName,
                CategoryName = categoryName,
                TotalLogs = stats.TotalLogs,
                CompletedLogs = stats.CompletedLogs,
                CompletionRate = stats.CompletionRate,
                CurrentStreak = stats.CurrentStreak,
                LongestStreak = stats.LongestStreak,
                WeeklyAverage = stats.WeeklyAverage,
                MonthlyAverage = stats.MonthlyAverage,
                TotalRelapses = stats.TotalRelapses
            };

            var logsByDate = allLogs
                .GroupBy(l => l.LogDate.Date)
                .ToDictionary(g => g.Key, g => g.First());

            var today = DateTime.UtcNow.Date;

            // ============================================================
            // LAST 30 DAYS
            // ============================================================
            for (int i = 29; i >= 0; i--)
            {
                var date = today.AddDays(-i);
                logsByDate.TryGetValue(date, out var log);

                vm.Last30Days.Add(new DailyLogPoint
                {
                    Date = date,
                    Completed = log?.IsCompleted == true
                });
            }

            // ============================================================
            // LAST 8 WEEKS
            // ============================================================
            for (int w = 7; w >= 0; w--)
            {
                var weekStart = today.AddDays(-(w * 7) - 6);
                var weekEnd = today.AddDays(-(w * 7));

                int completed = 0;
                int total = 0;

                for (var d = weekStart; d <= weekEnd; d = d.AddDays(1))
                {
                    total++;
                    if (logsByDate.TryGetValue(d, out var l) && l.IsCompleted)
                        completed++;
                }

                vm.Last8Weeks.Add(new WeeklyProgressPoint
                {
                    WeekLabel = w == 0 ? "This week" : $"{weekStart:MMM dd}",
                    Completed = completed,
                    Total = total
                });
            }

            // ============================================================
            // HEATMAP (LAST 30 DAYS)
            // ============================================================
            foreach (var day in vm.Last30Days)
            {
                vm.HeatmapDays.Add(new HeatmapDay
                {
                    Date = day.Date,
                    Completed = day.Completed
                });
            }

            return vm;
        }
    }

    public class DailyLogPoint
    {
        public DateTime Date { get; set; }
        public bool Completed { get; set; }
    }

    public class WeeklyProgressPoint
    {
        public string WeekLabel { get; set; } = string.Empty;
        public int Completed { get; set; }
        public int Total { get; set; }
        public double Percentage => Total > 0 ? (double)Completed / Total * 100 : 0;
    }

    public class HeatmapDay
    {
        public DateTime Date { get; set; }
        public bool Completed { get; set; }
    }
}