namespace Mindora.Application.DTOs.Analytics
{
    public class DailyMoodTrendDto
    {
        public DateTime Date { get; set; }
        public double MoodScore { get; set; }
        public int JournalEntries { get; set; }
    }
}