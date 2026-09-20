namespace Mindora.Application.DTOs.Reporting
{
    public class ReportTemplateDto
    {
        public Guid TemplateId { get; set; }
        public string TemplateKey { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Category { get; set; } = "Business";
        public string DefaultFormat { get; set; } = "CSV";
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}