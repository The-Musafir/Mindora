namespace Mindora.Application.DTOs.Auth
{
    public class AuthResponse
    {
        public bool Succeeded { get; set; }
        public string Message { get; set; } = string.Empty;
        public string[] Errors { get; set; } = Array.Empty<string>();
    }
}