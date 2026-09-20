using Mindora.Application.DTOs.Auth;

namespace Mindora.Application.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponse> RegisterAsync(RegisterRequest request);
        Task<AuthResponse> LoginAsync(LoginRequest request);
        Task LogoutAsync();
    }
}