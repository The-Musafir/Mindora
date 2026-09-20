namespace Mindora.Domain.Entities
{
    public class AssessmentOption
    {
        public Guid OptionId { get; set; }
        public Guid QuestionId { get; set; }
        public string OptionText { get; set; } = string.Empty;
        public int ScoreValue { get; set; }

        public virtual AssessmentQuestion Question { get; set; } = null!;
    }
}