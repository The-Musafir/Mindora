namespace Mindora.Application.DTOs.Journal
{
    public class UpdateJournalRequest
    {
        public Guid EntryId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public bool IsPrivate { get; set; } = true;
        public string Category { get; set; } = "Personal";
        public DateOnly EntryDate { get; set; }
    }
}