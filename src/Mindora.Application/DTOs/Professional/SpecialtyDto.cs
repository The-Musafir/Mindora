namespace Mindora.Application.DTOs.Professional
{
    public class SpecialtyDto
    {
        public Guid SpecialtyId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}