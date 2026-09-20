namespace Mindora.Application.DTOs.AdminDashboard
{
    public class AdminWellnessResourceDto
    {
        public Guid ResourceId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public bool IsPublished { get; set; }
    }
}