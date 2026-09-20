using Mindora.Application.DTOs.Home;

namespace Mindora.Application.Interfaces
{
    public interface ILandingPageService
    {
        Task<HomeViewModel> BuildHomeViewModelAsync(Guid? currentUserId);
    }
}