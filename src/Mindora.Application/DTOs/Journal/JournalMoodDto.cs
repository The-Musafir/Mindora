namespace Mindora.Application.DTOs.Journal
{
    public class JournalMoodDto
    {
        public Guid MoodId { get; set; }
        public string MoodLabel { get; set; } = string.Empty;
        public int Intensity { get; set; }
    }
}