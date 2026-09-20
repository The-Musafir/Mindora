using Mindora.Application.DTOs.Payment;

namespace Mindora.Application.Interfaces
{
    public interface IPaymentService
    {
        Task<IReadOnlyList<SubscriptionPlanDto>> GetAllPlansAsync();
        Task<SubscriptionPlanDto?> GetPlanByIdAsync(Guid planId);
        Task<Guid> CreatePlanAsync(SubscriptionPlanDto dto);
        Task<bool> UpdatePlanAsync(SubscriptionPlanDto dto);
        Task<bool> TogglePlanActiveAsync(Guid planId, bool isActive);

        Task<UserSubscriptionDto> SubscribeAsync(Guid userId, Guid planId);
        Task<UserSubscriptionDto?> GetCurrentSubscriptionAsync(Guid userId);
        Task<IReadOnlyList<UserSubscriptionDto>> GetUserSubscriptionHistoryAsync(Guid userId);
        Task<bool> CancelSubscriptionAsync(Guid subscriptionId, Guid userId);

        Task<PaymentResponseDto> CreatePaymentAsync(Guid userId, CreatePaymentRequest request);
        Task<PaymentResponseDto?> GetPaymentByTransactionIdAsync(string transactionId);
        Task<PaymentResponseDto?> GetPaymentByIdAsync(Guid paymentId);
        Task<IReadOnlyList<PaymentResponseDto>> GetUserPaymentsAsync(Guid userId);
        Task<bool> UpdatePaymentStatusAsync(Guid paymentId, string status, string? transactionId = null);

        Task<ConsultationPaymentDto> CreateConsultationPaymentAsync(Guid userId, ConsultationPaymentDto dto);
        Task<ConsultationPaymentDto?> GetConsultationPaymentByIdAsync(Guid consultationPaymentId);
        Task<IReadOnlyList<ConsultationPaymentDto>> GetUserConsultationPaymentsAsync(Guid userId);
        Task<bool> UpdateConsultationPaymentStatusAsync(Guid consultationPaymentId, string status);

        Task<InvoiceDto> GenerateInvoiceAsync(Guid paymentId);
        Task<InvoiceDto?> GetInvoiceByPaymentIdAsync(Guid paymentId);
        Task<IReadOnlyList<InvoiceDto>> GetUserInvoicesAsync(Guid userId);

        Task<IReadOnlyList<TransactionDto>> GetUserTransactionsAsync(Guid userId);

        // SSLCommerz
        Task<SSLCommerzInitResponse?> InitiateGatewayPaymentAsync(Guid userId, CreatePaymentRequest request);
        Task<bool> ValidateIpnAsync(SSLCommerzIpnResponse ipnResponse);
        
    }
}