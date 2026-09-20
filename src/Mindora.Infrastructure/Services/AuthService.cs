using Microsoft.AspNetCore.Identity;
using Mindora.Application.DTOs.Auth;
using Mindora.Application.Interfaces;
using Mindora.Domain.Entities;
using System.Linq;
using System.Threading.Tasks;

namespace Mindora.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<Role> _roleManager;

        public AuthService(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            RoleManager<Role> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
        {
            var response = new AuthResponse();

            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            if (existingUser != null)
            {
                response.Succeeded = false;
                response.Message = "Email already exists.";
                response.Errors = new[] { "This email is already registered." };
                return response;
            }

            var user = new User
            {
                UserName = request.Email,
                Email = request.Email,
                EmailConfirmed = true,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            var createResult = await _userManager.CreateAsync(user, request.Password);
            if (!createResult.Succeeded)
            {
                response.Succeeded = false;
                response.Message = "User registration failed.";
                response.Errors = createResult.Errors.Select(e => e.Description).ToArray();
                return response;
            }

            if (await _roleManager.RoleExistsAsync("User"))
            {
                await _userManager.AddToRoleAsync(user, "User");
            }

            response.Succeeded = true;
            response.Message = "Registration successful.";
            return response;
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            var response = new AuthResponse();

            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                response.Succeeded = false;
                response.Message = "Invalid email or password.";
                response.Errors = new[] { "Invalid login attempt." };
                return response;
            }

            var passwordValid = await _userManager.CheckPasswordAsync(user, request.Password);
            if (!passwordValid)
            {
                response.Succeeded = false;
                response.Message = "Invalid email or password.";
                response.Errors = new[] { "Invalid login attempt." };
                return response;
            }

            await _signInManager.SignInAsync(user, isPersistent: request.RememberMe);

            response.Succeeded = true;
            response.Message = "Login successful.";
            return response;
        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }
    }
}