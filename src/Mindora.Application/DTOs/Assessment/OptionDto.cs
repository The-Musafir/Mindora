namespace Mindora.Application.DTOs.Assessment
{
    public class OptionDto
    {
        public Guid OptionId { get; set; }
        public string OptionText { get; set; } = string.Empty;
        public int ScoreValue { get; set; }
    }
}