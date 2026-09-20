namespace Mindora.Application.DTOs.Community
{
    public class CreateGroupRequest
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}