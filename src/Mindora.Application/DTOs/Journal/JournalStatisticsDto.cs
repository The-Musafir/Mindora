namespace Mindora.Application.DTOs.Journal
{
    public class JournalStatisticsDto
    {
        public int TotalEntries { get; set; }
        public int PrivateEntries { get; set; }
        public int PublicEntries { get; set; }
        public int ThisWeekEntries { get; set; }
        public int ThisMonthEntries { get; set; }
        public Dictionary<string, int> MoodCounts { get; set; } = new();
        public Dictionary<string, int> CategoryCounts { get; set; } = new();
    }
}