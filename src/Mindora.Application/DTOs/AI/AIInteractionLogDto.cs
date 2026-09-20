namespace Mindora.Application.DTOs.AI
{
    public class AIInteractionLogDto
    {
        public Guid LogId { get; set; }
        public Guid SessionId { get; set; }
        public Guid? TemplateId { get; set; }
        public string Action { get; set; } = string.Empty;
        public string? Metadata { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}