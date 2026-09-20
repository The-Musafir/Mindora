namespace Mindora.Application.DTOs.Payment
{
    public class CreatePaymentRequest
    {
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "BDT";
        public string Gateway { get; set; } = "SSLCommerz";
        public string PaymentType { get; set; } = "Subscription";
        public Guid? SubscriptionId { get; set; }
        public Guid? ConsultationPaymentId { get; set; }
        public string? IdempotencyKey { get; set; }
        public string? PaymentMethod { get; set; } // ✅ bKash, Nagad, Rocket, Card

        // Customer Info
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public string CustomerPhone { get; set; } = string.Empty;
        public string CustomerAddress { get; set; } = string.Empty;
        public string CustomerCity { get; set; } = string.Empty;
        public string CustomerPostcode { get; set; } = string.Empty;
        public string CustomerCountry { get; set; } = "BD";
    }
}