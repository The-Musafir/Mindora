using System.Net.Http;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Mindora.Application.DTOs.Payment;
using Mindora.Application.Interfaces;

namespace Mindora.Infrastructure.Services
{
    public class SSLCommerzService : ISSLCommerzService
    {
        private readonly HttpClient _httpClient;
        private readonly string _storeId;
        private readonly string _storePassword;
        private readonly bool _sandboxMode;
        private readonly string _baseUrl;

        public SSLCommerzService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _storeId = configuration["SSLCommerz:StoreId"] ?? "";
            _storePassword = configuration["SSLCommerz:StorePassword"] ?? "";
            _sandboxMode = configuration.GetValue<bool>("SSLCommerz:SandboxMode");
            _baseUrl = _sandboxMode
                ? "https://sandbox.sslcommerz.com"
                : "https://securepay.sslcommerz.com";
        }

        public async Task<SSLCommerzInitResponse?> InitiatePaymentAsync(SSLCommerzInitRequest request)
        {
            var payload = new
            {
                store_id = _storeId,
                store_passwd = _storePassword,
                total_amount = request.Amount.ToString("F2"),
                currency = request.Currency,
                tran_id = request.TransactionId,
                success_url = "http://localhost:5000/Payment/Success",
                fail_url = "http://localhost:5000/Payment/Fail",
                cancel_url = "http://localhost:5000/Payment/Cancel",
                ipn_url = "http://localhost:5000/Payment/Ipn",
                product_name = request.ProductName,
                product_category = request.ProductCategory,
                cus_name = request.CustomerName,
                cus_email = request.CustomerEmail,
                cus_phone = request.CustomerPhone,
                cus_add1 = request.CustomerAddress,
                cus_city = request.CustomerCity,
                cus_postcode = request.CustomerPostcode,
                cus_country = request.CustomerCountry
            };

            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{_baseUrl}/gwprocess/v4/api.php", content);
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<SSLCommerzInitResponse>(json);
        }

        public Task<bool> ValidateIpnAsync(SSLCommerzIpnResponse ipnResponse)
        {
            // Sandbox/Live both accept VALID, VALIDATED, SUCCESS
            bool isValidStatus = ipnResponse.Status == "VALID" ||
                                 ipnResponse.Status == "VALIDATED" ||
                                 ipnResponse.Status == "SUCCESS";

            // Additional checks (amount, currency) can be added here
            return Task.FromResult(isValidStatus);
        }
    }
}