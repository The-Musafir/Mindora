namespace Mindora.Application.Interfaces
{
    public interface ISubscriptionRenewalService
    {
        Task<int> RenewExpiredSubscriptionsAsync();
    }
}