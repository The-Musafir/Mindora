namespace Mindora.Application.DTOs.Payment
{
    public class SSLCommerzInitRequest
    {
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "BDT";
        public string TransactionId { get; set; } = string.Empty; // Merchant-generated unique ID
        public string ProductName { get; set; } = string.Empty;
        public string ProductCategory { get; set; } = "General";
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public string CustomerPhone { get; set; } = string.Empty;
        public string CustomerAddress { get; set; } = string.Empty;
        public string CustomerCity { get; set; } = string.Empty;
        public string CustomerPostcode { get; set; } = string.Empty;
        public string CustomerCountry { get; set; } = "BD";
    }
}