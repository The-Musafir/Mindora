namespace Mindora.Application.DTOs.Assessment
{
    public class TakeAssessmentRequest
    {
        public Guid QuestionnaireId { get; set; }
        public List<AnswerDto> Answers { get; set; } = new();
    }

    public class AnswerDto
    {
        public Guid QuestionId { get; set; }
        public Guid? SelectedOptionId { get; set; }
        public string? FreeText { get; set; }
    }
}