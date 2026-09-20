namespace Mindora.Application.Interfaces
{
    public interface IGeminiService
    {
        Task<string> GenerateResponseAsync(string prompt, string? context = null);
    }
}