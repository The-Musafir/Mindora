namespace Mindora.Application.DTOs.AI
{
    public class AIPromptTemplateDto
    {
        public Guid TemplateId { get; set; }
        public string TemplateKey { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string PromptText { get; set; } = string.Empty;
        public string Tone { get; set; } = "Supportive";
        public bool IsActive { get; set; }
    }
}