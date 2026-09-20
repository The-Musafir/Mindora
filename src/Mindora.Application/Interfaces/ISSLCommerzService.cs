using Mindora.Application.DTOs.Payment;

namespace Mindora.Application.Interfaces
{
    public interface ISSLCommerzService
    {
        Task<SSLCommerzInitResponse?> InitiatePaymentAsync(SSLCommerzInitRequest request);
        Task<bool> ValidateIpnAsync(SSLCommerzIpnResponse ipnResponse);
    }
}