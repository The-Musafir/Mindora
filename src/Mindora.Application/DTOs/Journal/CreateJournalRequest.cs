namespace Mindora.Application.DTOs.Journal
{
    public class CreateJournalRequest
    {
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public bool IsPrivate { get; set; } = true;
        public string Category { get; set; } = "Personal";
        public DateOnly EntryDate { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
        public List<string> Tags { get; set; } = new();
        public string? MoodLabel { get; set; }
        public int? MoodIntensity { get; set; }
    }
}