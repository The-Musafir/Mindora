namespace Mindora.Application.DTOs.Journal
{
    public class JournalResponse
    {
        public Guid EntryId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public bool IsPrivate { get; set; }
        public string Category { get; set; } = string.Empty;
        public DateOnly EntryDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<string> Tags { get; set; } = new();
        public List<JournalMoodDto> Moods { get; set; } = new();
    }
}