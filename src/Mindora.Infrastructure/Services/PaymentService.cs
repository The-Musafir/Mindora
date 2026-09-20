using Microsoft.EntityFrameworkCore;
using Mindora.Application.DTOs.Payment;
using Mindora.Application.Interfaces;
using Mindora.Domain.Entities;
using Mindora.Infrastructure.Notifications;
using Mindora.Infrastructure.Persistence.DbContext;

namespace Mindora.Infrastructure.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly MindoraDbContext _context;
        private readonly ISSLCommerzService _sslCommerzService;
        private readonly INotificationDispatcher _dispatcher;

        public PaymentService(
            MindoraDbContext context,
            ISSLCommerzService sslCommerzService,
            INotificationDispatcher dispatcher)
        {
            _context = context;
            _sslCommerzService = sslCommerzService;
            _dispatcher = dispatcher;
        }

        // ============================
        // SUBSCRIPTION PLANS
        // ============================

        public async Task<IReadOnlyList<SubscriptionPlanDto>> GetAllPlansAsync()
        {
            var plans = await _context.SubscriptionPlans
                .Where(p => p.IsActive)
                .OrderBy(p => p.Price)
                .ToListAsync();

            return plans.Select(MapPlanToDto).ToList();
        }

        public async Task<SubscriptionPlanDto?> GetPlanByIdAsync(Guid planId)
        {
            var plan = await _context.SubscriptionPlans.FindAsync(planId);
            return plan == null ? null : MapPlanToDto(plan);
        }

        public async Task<Guid> CreatePlanAsync(SubscriptionPlanDto dto)
        {
            var plan = new SubscriptionPlan
            {
                PlanId = Guid.NewGuid(),
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                DurationDays = dto.DurationDays,
                IsActive = dto.IsActive,
                CreatedAt = DateTime.UtcNow
            };

            _context.SubscriptionPlans.Add(plan);
            await _context.SaveChangesAsync();
            return plan.PlanId;
        }

        public async Task<bool> UpdatePlanAsync(SubscriptionPlanDto dto)
        {
            var plan = await _context.SubscriptionPlans.FindAsync(dto.PlanId);
            if (plan == null) return false;

            plan.Name = dto.Name;
            plan.Description = dto.Description;
            plan.Price = dto.Price;
            plan.DurationDays = dto.DurationDays;
            plan.IsActive = dto.IsActive;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> TogglePlanActiveAsync(Guid planId, bool isActive)
        {
            var plan = await _context.SubscriptionPlans.FindAsync(planId);
            if (plan == null) return false;

            plan.IsActive = isActive;
            await _context.SaveChangesAsync();
            return true;
        }

        // ============================
        // USER SUBSCRIPTION
        // ============================

        public async Task<UserSubscriptionDto> SubscribeAsync(Guid userId, Guid planId)
        {
            var plan = await _context.SubscriptionPlans.FindAsync(planId);
            if (plan == null)
                throw new KeyNotFoundException("Plan not found.");

            var subscription = new UserSubscription
            {
                SubscriptionId = Guid.NewGuid(),
                UserId = userId,
                PlanId = planId,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(plan.DurationDays),
                Status = "Active",
                CreatedAt = DateTime.UtcNow
            };

            _context.UserSubscriptions.Add(subscription);

            var payment = new Payment
            {
                PaymentId = Guid.NewGuid(),
                UserId = userId,
                Amount = plan.Price,
                Currency = "BDT",
                Gateway = "Manual",
                TransactionId = Guid.NewGuid().ToString("N"),
                Status = "Completed",
                PaymentType = "Subscription",
                SubscriptionId = subscription.SubscriptionId,
                IdempotencyKey = Guid.NewGuid().ToString("N"),
                PaymentMethod = "Manual",
                CreatedAt = DateTime.UtcNow
            };
            _context.Payments.Add(payment);

            await _context.SaveChangesAsync();

            // ============================================================
            // NOTIFICATION: Payment success
            // ============================================================
            try
            {
                var notification = MindoraNotifications.PaymentSuccess(plan.Price, plan.Name);
                await _dispatcher.DispatchAsync(userId, notification);
            }
            catch
            {
                // Notification failure must NEVER break the subscription flow
            }

            return await MapSubscriptionToDtoAsync(subscription);
        }

        public async Task<UserSubscriptionDto?> GetCurrentSubscriptionAsync(Guid userId)
        {
            var now = DateTime.UtcNow;
            var subscription = await _context.UserSubscriptions
                .Where(s => s.UserId == userId && s.Status == "Active" && s.EndDate > now)
                .Include(s => s.Plan)
                .OrderByDescending(s => s.EndDate)
                .FirstOrDefaultAsync();

            return subscription == null ? null : await MapSubscriptionToDtoAsync(subscription);
        }

        public async Task<IReadOnlyList<UserSubscriptionDto>> GetUserSubscriptionHistoryAsync(Guid userId)
        {
            var subscriptions = await _context.UserSubscriptions
                .Where(s => s.UserId == userId)
                .Include(s => s.Plan)
                .OrderByDescending(s => s.StartDate)
                .ToListAsync();

            var result = new List<UserSubscriptionDto>();
            foreach (var sub in subscriptions)
                result.Add(await MapSubscriptionToDtoAsync(sub));

            return result;
        }

        public async Task<bool> CancelSubscriptionAsync(Guid subscriptionId, Guid userId)
        {
            var subscription = await _context.UserSubscriptions
                .FirstOrDefaultAsync(s => s.SubscriptionId == subscriptionId && s.UserId == userId);

            if (subscription == null) return false;

            subscription.Status = "Cancelled";
            await _context.SaveChangesAsync();
            return true;
        }

        // ============================
        // PAYMENTS
        // ============================

        public async Task<PaymentResponseDto> CreatePaymentAsync(Guid userId, CreatePaymentRequest request)
        {
            if (!string.IsNullOrWhiteSpace(request.IdempotencyKey))
            {
                var existingPayment = await _context.Payments
                    .FirstOrDefaultAsync(p => p.IdempotencyKey == request.IdempotencyKey);

                if (existingPayment != null)
                {
                    return MapPaymentToDto(existingPayment);
                }
            }

            var payment = new Payment
            {
                PaymentId = Guid.NewGuid(),
                UserId = userId,
                Amount = request.Amount,
                Currency = request.Currency,
                Gateway = request.Gateway,
                Status = "Pending",
                PaymentType = request.PaymentType,
                SubscriptionId = request.SubscriptionId,
                ConsultationPaymentId = request.ConsultationPaymentId,
                IdempotencyKey = request.IdempotencyKey,
                PaymentMethod = request.PaymentMethod,
                CreatedAt = DateTime.UtcNow
            };

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

            return MapPaymentToDto(payment);
        }

        public async Task<SSLCommerzInitResponse?> InitiateGatewayPaymentAsync(Guid userId, CreatePaymentRequest request)
        {
            var payment = new Payment
            {
                PaymentId = Guid.NewGuid(),
                UserId = userId,
                Amount = request.Amount,
                Currency = request.Currency,
                Gateway = "SSLCommerz",
                Status = "Pending",
                PaymentType = request.PaymentType,
                SubscriptionId = request.SubscriptionId,
                ConsultationPaymentId = request.ConsultationPaymentId,
                IdempotencyKey = request.IdempotencyKey,
                PaymentMethod = request.PaymentMethod,
                CreatedAt = DateTime.UtcNow
            };

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

            var sslRequest = new SSLCommerzInitRequest
            {
                Amount = request.Amount,
                Currency = request.Currency,
                TransactionId = payment.PaymentId.ToString("N"),
                ProductName = request.PaymentType == "Subscription" ? "Mindora Subscription" : "Consultation Payment",
                ProductCategory = "Wellness",
                CustomerName = request.CustomerName,
                CustomerEmail = request.CustomerEmail,
                CustomerPhone = request.CustomerPhone,
                CustomerAddress = request.CustomerAddress,
                CustomerCity = request.CustomerCity,
                CustomerPostcode = request.CustomerPostcode,
                CustomerCountry = request.CustomerCountry
            };

            return await _sslCommerzService.InitiatePaymentAsync(sslRequest);
        }

        public async Task<bool> ValidateIpnAsync(SSLCommerzIpnResponse ipnResponse)
        {
            return await _sslCommerzService.ValidateIpnAsync(ipnResponse);
        }

        public async Task<PaymentResponseDto?> GetPaymentByIdAsync(Guid paymentId)
        {
            var payment = await _context.Payments.FindAsync(paymentId);
            return payment == null ? null : MapPaymentToDto(payment);
        }

        public async Task<IReadOnlyList<PaymentResponseDto>> GetUserPaymentsAsync(Guid userId)
        {
            var payments = await _context.Payments
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            return payments.Select(MapPaymentToDto).ToList();
        }

        public async Task<bool> UpdatePaymentStatusAsync(Guid paymentId, string status, string? transactionId = null)
        {
            var payment = await _context.Payments.FindAsync(paymentId);
            if (payment == null) return false;

            var previousStatus = payment.Status;

            payment.Status = status;
            if (!string.IsNullOrWhiteSpace(transactionId))
                payment.TransactionId = transactionId;

            await _context.SaveChangesAsync();

            // ============================================================
            // NOTIFICATION: Only notify on status transitions (not repeats)
            // ============================================================
            if (previousStatus != status)
            {
                try
                {
                    if (status == "Completed")
                    {
                        var notif = MindoraNotifications.PaymentSuccess(payment.Amount, "your plan");
                        await _dispatcher.DispatchAsync(payment.UserId, notif);
                    }
                    else if (status == "Failed")
                    {
                        var notif = MindoraNotifications.PaymentFailed(payment.Amount);
                        await _dispatcher.DispatchAsync(payment.UserId, notif);
                    }
                }
                catch
                {
                    // Silent fail
                }
            }

            return true;
        }

        // ============================
        // CONSULTATION PAYMENTS
        // ============================

        public async Task<ConsultationPaymentDto> CreateConsultationPaymentAsync(Guid userId, ConsultationPaymentDto dto)
        {
            var consultationPayment = new ConsultationPayment
            {
                ConsultationPaymentId = Guid.NewGuid(),
                AppointmentId = dto.AppointmentId,
                UserId = userId,
                ProfessionalId = dto.ProfessionalId,
                Amount = dto.Amount,
                Status = "Pending",
                CreatedAt = DateTime.UtcNow
            };

            _context.ConsultationPayments.Add(consultationPayment);
            await _context.SaveChangesAsync();

            return MapConsultationPaymentToDto(consultationPayment);
        }

        public async Task<ConsultationPaymentDto?> GetConsultationPaymentByIdAsync(Guid consultationPaymentId)
        {
            var cp = await _context.ConsultationPayments.FindAsync(consultationPaymentId);
            return cp == null ? null : MapConsultationPaymentToDto(cp);
        }

        public async Task<IReadOnlyList<ConsultationPaymentDto>> GetUserConsultationPaymentsAsync(Guid userId)
        {
            var payments = await _context.ConsultationPayments
                .Where(cp => cp.UserId == userId)
                .OrderByDescending(cp => cp.CreatedAt)
                .ToListAsync();

            return payments.Select(MapConsultationPaymentToDto).ToList();
        }

        public async Task<bool> UpdateConsultationPaymentStatusAsync(Guid consultationPaymentId, string status)
        {
            var cp = await _context.ConsultationPayments.FindAsync(consultationPaymentId);
            if (cp == null) return false;

            cp.Status = status;
            await _context.SaveChangesAsync();
            return true;
        }

        // ============================
        // INVOICES
        // ============================

        public async Task<InvoiceDto> GenerateInvoiceAsync(Guid paymentId)
        {
            var payment = await _context.Payments.FindAsync(paymentId);
            if (payment == null)
                throw new KeyNotFoundException("Payment not found.");

            var invoice = new Invoice
            {
                InvoiceId = Guid.NewGuid(),
                PaymentId = paymentId,
                InvoiceNumber = $"INV-{DateTime.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid().ToString("N").Substring(0, 6).ToUpper()}",
                Amount = payment.Amount,
                IssuedAt = DateTime.UtcNow
            };

            _context.Invoices.Add(invoice);
            await _context.SaveChangesAsync();

            return MapInvoiceToDto(invoice);
        }

        public async Task<InvoiceDto?> GetInvoiceByPaymentIdAsync(Guid paymentId)
        {
            var invoice = await _context.Invoices
                .FirstOrDefaultAsync(i => i.PaymentId == paymentId);

            return invoice == null ? null : MapInvoiceToDto(invoice);
        }

        public async Task<IReadOnlyList<InvoiceDto>> GetUserInvoicesAsync(Guid userId)
        {
            var invoices = await _context.Invoices
                .Include(i => i.Payment)
                .Where(i => i.Payment.UserId == userId)
                .OrderByDescending(i => i.IssuedAt)
                .ToListAsync();

            return invoices.Select(MapInvoiceToDto).ToList();
        }

        // ============================
        // TRANSACTION HISTORY
        // ============================

        public async Task<IReadOnlyList<TransactionDto>> GetUserTransactionsAsync(Guid userId)
        {
            var transactions = await _context.Transactions
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();

            return transactions.Select(t => new TransactionDto
            {
                TransactionId = t.TransactionId,
                UserId = t.UserId,
                PaymentId = t.PaymentId,
                Amount = t.Amount,
                Status = t.Status,
                CreatedAt = t.CreatedAt
            }).ToList();
        }

        public async Task<PaymentResponseDto?> GetPaymentByTransactionIdAsync(string transactionId)
        {
            var payment = await _context.Payments
                .FirstOrDefaultAsync(p => p.TransactionId == transactionId);

            return payment == null ? null : MapPaymentToDto(payment);
        }

        // ============================
        // PRIVATE MAPPING HELPERS
        // ============================

        private SubscriptionPlanDto MapPlanToDto(SubscriptionPlan plan)
        {
            return new SubscriptionPlanDto
            {
                PlanId = plan.PlanId,
                Name = plan.Name,
                Description = plan.Description,
                Price = plan.Price,
                DurationDays = plan.DurationDays,
                IsActive = plan.IsActive,
                CreatedAt = plan.CreatedAt
            };
        }

        private async Task<UserSubscriptionDto> MapSubscriptionToDtoAsync(UserSubscription subscription)
        {
            var planName = subscription.Plan?.Name ?? "Unknown";
            return new UserSubscriptionDto
            {
                SubscriptionId = subscription.SubscriptionId,
                UserId = subscription.UserId,
                PlanId = subscription.PlanId,
                PlanName = planName,
                StartDate = subscription.StartDate,
                EndDate = subscription.EndDate,
                Status = subscription.Status,
                CreatedAt = subscription.CreatedAt
            };
        }

        private PaymentResponseDto MapPaymentToDto(Payment payment)
        {
            return new PaymentResponseDto
            {
                PaymentId = payment.PaymentId,
                UserId = payment.UserId,
                Amount = payment.Amount,
                Currency = payment.Currency,
                Gateway = payment.Gateway,
                TransactionId = payment.TransactionId,
                Status = payment.Status,
                PaymentType = payment.PaymentType,
                PaymentMethod = payment.PaymentMethod,
                CreatedAt = payment.CreatedAt
            };
        }

        private ConsultationPaymentDto MapConsultationPaymentToDto(ConsultationPayment cp)
        {
            return new ConsultationPaymentDto
            {
                ConsultationPaymentId = cp.ConsultationPaymentId,
                AppointmentId = cp.AppointmentId,
                UserId = cp.UserId,
                ProfessionalId = cp.ProfessionalId,
                Amount = cp.Amount,
                Status = cp.Status,
                CreatedAt = cp.CreatedAt
            };
        }

        private InvoiceDto MapInvoiceToDto(Invoice invoice)
        {
            return new InvoiceDto
            {
                InvoiceId = invoice.InvoiceId,
                PaymentId = invoice.PaymentId,
                InvoiceNumber = invoice.InvoiceNumber,
                Amount = invoice.Amount,
                IssuedAt = invoice.IssuedAt
            };
        }
    }
}