using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Mindora.Application.DTOs.Payment;
using Mindora.Application.Interfaces;
using Mindora.Domain.Entities;
using System.Security.Claims;

namespace Mindora.Web.Controllers
{
    [Authorize]
    public class PaymentController : Controller
    {
        private readonly IPaymentService _paymentService;
        private readonly IInvoiceService _invoiceService;
        private readonly UserManager<User> _userManager;

        public PaymentController(
            IPaymentService paymentService,
            IInvoiceService invoiceService,
            UserManager<User> userManager)
        {
            _paymentService = paymentService;
            _invoiceService = invoiceService;
            _userManager = userManager;
        }

        private Guid CurrentUserId => Guid.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier) ?? Guid.Empty.ToString());

        // ============================================================
        // BILLING DASHBOARD (NEW)
        // ============================================================
        // GET: /Payment
        public async Task<IActionResult> Index()
        {
            // Get all payment-related data for the billing dashboard
            var current = await _paymentService.GetCurrentSubscriptionAsync(CurrentUserId);
            var allPayments = await _paymentService.GetUserPaymentsAsync(CurrentUserId);
            var allInvoices = await _paymentService.GetUserInvoicesAsync(CurrentUserId);
            var plans = await _paymentService.GetAllPlansAsync();

            // Summary stats
            var completedPayments = allPayments.Where(p => p.Status == "Completed").ToList();
            decimal totalSpent = completedPayments.Sum(p => p.Amount);
            int paymentCount = allPayments.Count;
            int invoiceCount = allInvoices.Count;

            // Recent items (last 5)
            var recentPayments = allPayments
                .OrderByDescending(p => p.CreatedAt)
                .Take(5)
                .ToList();

            var recentInvoices = allInvoices
                .OrderByDescending(i => i.IssuedAt)
                .Take(5)
                .ToList();

            // Pass to view
            ViewBag.CurrentSubscription = current;
            ViewBag.RecentPayments = recentPayments;
            ViewBag.RecentInvoices = recentInvoices;
            ViewBag.TotalSpent = totalSpent;
            ViewBag.PaymentCount = paymentCount;
            ViewBag.InvoiceCount = invoiceCount;

            return View(plans);
        }

        // ============================
        // SUBSCRIPTION PLANS
        // ============================

        // GET: /Payment/Plans
        public async Task<IActionResult> Plans()
        {
            var plans = await _paymentService.GetAllPlansAsync();
            return View(plans);
        }

        // POST: /Payment/Subscribe
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Subscribe(Guid planId)
        {
            if (planId == Guid.Empty)
                return RedirectToAction(nameof(Plans));

            var subscription = await _paymentService.SubscribeAsync(CurrentUserId, planId);
            return RedirectToAction(nameof(MySubscription));
        }

        // GET: /Payment/MySubscription
        public async Task<IActionResult> MySubscription()
        {
            var current = await _paymentService.GetCurrentSubscriptionAsync(CurrentUserId);
            var history = await _paymentService.GetUserSubscriptionHistoryAsync(CurrentUserId);
            ViewBag.Current = current;
            return View(history);
        }

        // POST: /Payment/CancelSubscription
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelSubscription(Guid subscriptionId)
        {
            await _paymentService.CancelSubscriptionAsync(subscriptionId, CurrentUserId);
            return RedirectToAction(nameof(MySubscription));
        }

        // ============================
        // PAYMENTS
        // ============================

        // GET: /Payment/History
        public async Task<IActionResult> History()
        {
            var payments = await _paymentService.GetUserPaymentsAsync(CurrentUserId);
            var transactions = await _paymentService.GetUserTransactionsAsync(CurrentUserId);
            ViewBag.Payments = payments;
            return View(transactions);
        }

        // POST: /Payment/Create (Manual test/later gateway integration)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreatePaymentRequest request)
        {
            if (!ModelState.IsValid)
                return RedirectToAction(nameof(Plans));

            await _paymentService.CreatePaymentAsync(CurrentUserId, request);
            return RedirectToAction(nameof(History));
        }

        // ============================
        // SSLCOMMERZ PAYMENT
        // ============================

        // GET: /Payment/InitiatePayment
        [HttpGet]
        public IActionResult InitiatePayment()
        {
            return View(new CreatePaymentRequest());
        }

        // POST: /Payment/InitiatePayment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> InitiatePayment(CreatePaymentRequest request)
        {
            if (!ModelState.IsValid) return View(request);

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Challenge();

            request.CustomerName = user.UserName ?? user.Email ?? "Customer";
            request.CustomerEmail = user.Email ?? "";
            request.CustomerPhone = user.PhoneNumber ?? "01700000000";
            request.CustomerAddress = "Dhaka";
            request.CustomerCity = "Dhaka";
            request.CustomerPostcode = "1212";
            request.CustomerCountry = "BD";
            request.IdempotencyKey = Guid.NewGuid().ToString("N");

            var response = await _paymentService.InitiateGatewayPaymentAsync(user.Id, request);

            if (response != null && response.Status == "SUCCESS")
            {
                return Redirect(response.GatewayPageURL);
            }

            TempData["Error"] = response?.FailedReason ?? "Payment initiation failed.";
            return RedirectToAction(nameof(History));
        }

        // GET: /Payment/Success
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Success()
        {
            return View();
        }

        // GET: /Payment/Fail
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Fail()
        {
            return View();
        }

        // GET: /Payment/Cancel
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Cancel()
        {
            return View();
        }

        // POST: /Payment/Ipn
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Ipn([FromForm] SSLCommerzIpnResponse ipnResponse)
        {
            bool isValid = await _paymentService.ValidateIpnAsync(ipnResponse);
            if (isValid)
            {
                var payment = await _paymentService.GetPaymentByTransactionIdAsync(ipnResponse.TranId);
                if (payment != null)
                {
                    await _paymentService.UpdatePaymentStatusAsync(payment.PaymentId, "Completed", ipnResponse.TranId);
                }
            }
            return Ok();
        }

        // ============================
        // INVOICES
        // ============================

        // GET: /Payment/Invoices
        public async Task<IActionResult> Invoices()
        {
            var invoices = await _paymentService.GetUserInvoicesAsync(CurrentUserId);
            return View(invoices);
        }

        // POST: /Payment/GenerateInvoice
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GenerateInvoice(Guid paymentId)
        {
            await _paymentService.GenerateInvoiceAsync(paymentId);
            return RedirectToAction(nameof(Invoices));
        }

        // GET: /Payment/DownloadInvoice/{invoiceId}
        public async Task<IActionResult> DownloadInvoice(Guid invoiceId)
        {
            var pdfBytes = await _invoiceService.DownloadInvoicePdfAsync(invoiceId);
            return File(pdfBytes, "application/pdf", $"Invoice-{invoiceId}.pdf");
        }

        // ============================
        // CONSULTATION PAYMENTS (Admin/Provider/User)
        // ============================

        // GET: /Payment/Consultation
        public async Task<IActionResult> Consultation()
        {
            var payments = await _paymentService.GetUserConsultationPaymentsAsync(CurrentUserId);
            return View(payments);
        }
    }
}