namespace Mindora.Application.DTOs.Assessment
{
    public class QuestionDto
    {
        public Guid QuestionId { get; set; }
        public string QuestionText { get; set; } = string.Empty;
        public int OrderIndex { get; set; }
        public List<OptionDto> Options { get; set; } = new();
    }
}