using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Mindora.Application.Interfaces;
using Mindora.Application.Interfaces.Repositories;
using Mindora.Domain.Entities;
using Mindora.Infrastructure.BackgroundServices;
using Mindora.Infrastructure.Interceptors;
using Mindora.Infrastructure.Persistence;
using Mindora.Infrastructure.Persistence.DbContext;
using Mindora.Infrastructure.Repositories;
using Mindora.Infrastructure.Services;
using Mindora.Web.Hubs;
using Mindora.Web.Realtime;
using Mindora.Web.Services;


var builder = WebApplication.CreateBuilder(args);

// ============================================================
// EMAIL SENDER
// ============================================================
builder.Services.AddScoped<IEmailSender, EmailSender>();

// ============================================================
// MVC & RAZOR PAGES
// ============================================================
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// ============================================================
// SIGNALR REAL-TIME
// ============================================================
builder.Services.AddSignalR();

// Connection Manager — Hub ও Service উভয়ের জন্য shared
builder.Services.AddSingleton<IConnectionManager, ConnectionManager>();

// SignalR UserId Provider — Clients.User(userId) এর জন্য অপরিহার্য
builder.Services.AddSingleton<IUserIdProvider, NameIdentifierUserIdProvider>();

// Realtime Notification Service — module থেকে push করার জন্য
builder.Services.AddScoped<IRealtimeNotificationService, RealtimeNotificationService>();

// Notification Dispatcher — module থেকে notification পাঠানোর জন্য
builder.Services.AddScoped<INotificationDispatcher, NotificationDispatcher>();

// ============================================================
// DATABASE CONTEXT
// ============================================================
builder.Services.AddDbContext<MindoraDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly("Mindora.Infrastructure"))
    .AddInterceptors(new AuditInterceptor()));

// ============================================================
// REPOSITORY PATTERN
// ============================================================
builder.Services.AddScoped(
    typeof(IGenericRepository<>),
    typeof(GenericRepository<>));

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// ============================================================
// APPLICATION SERVICES (Core Modules)
// ============================================================
builder.Services.AddScoped<IHabitService, HabitService>();
builder.Services.AddScoped<IJournalService, JournalService>();
builder.Services.AddScoped<IAssessmentService, AssessmentService>();
builder.Services.AddScoped<ICommunityService, CommunityService>();
builder.Services.AddScoped<IProfessionalService, ProfessionalService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IConsultationService, ConsultationService>();
builder.Services.AddScoped<IAIWellnessService, AIWellnessService>();
builder.Services.AddScoped<IAuthService, AuthService>();

// ============================================================
// PAYMENT & RELATED SERVICES
// ============================================================
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IInvoiceService, InvoiceService>();
builder.Services.AddScoped<ICouponService, CouponService>();
builder.Services.AddScoped<IRefundService, RefundService>();
builder.Services.AddScoped<IPayoutService, PayoutService>();
builder.Services.AddHttpClient<ISSLCommerzService, SSLCommerzService>();
builder.Services.AddScoped<ISubscriptionRenewalService, SubscriptionRenewalService>();
builder.Services.AddHostedService<SubscriptionRenewalBackgroundService>();
builder.Services.AddScoped<IRevenueService, RevenueService>();

// ============================================================
// ADMIN DASHBOARD SERVICES
// ============================================================
builder.Services.AddScoped<IAdminPaymentDashboardService, AdminPaymentDashboardService>();
builder.Services.AddScoped<IAdminDashboardService, AdminDashboardService>();
builder.Services.AddScoped<IAdminUserManagementService, AdminUserManagementService>();
builder.Services.AddScoped<IAdminProfessionalService, AdminProfessionalService>();
builder.Services.AddScoped<IAdminModerationService, AdminModerationService>();
builder.Services.AddScoped<IAdminPaymentManagementService, AdminPaymentManagementService>();
builder.Services.AddScoped<IAdminNotificationManagementService, AdminNotificationManagementService>();
builder.Services.AddScoped<IAdminContentManagementService, AdminContentManagementService>();
builder.Services.AddScoped<IAdminAnalyticsService, AdminAnalyticsService>();
builder.Services.AddScoped<IAdminSystemSettingsService, AdminSystemSettingsService>();
builder.Services.AddScoped<IAdminAuditService, AdminAuditService>();

// ============================================================
// ANALYTICS MODULE SERVICES
// ============================================================
builder.Services.AddScoped<IUserAnalyticsService, UserAnalyticsService>();
builder.Services.AddScoped<IAdminAnalyticsFullService, AdminAnalyticsFullService>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<IAnalyticsEventService, AnalyticsEventService>();
builder.Services.AddScoped<IAnalyticsSnapshotService, AnalyticsSnapshotService>();
builder.Services.AddHostedService<AnalyticsSnapshotBackgroundService>();

// ============================================================
// REPORTING MODULE SERVICES
// ============================================================
builder.Services.AddScoped<IReportTemplateService, ReportTemplateService>();
builder.Services.AddScoped<IReportGeneratorService, ReportGeneratorService>();
builder.Services.AddScoped<IReportExportService, ReportExportService>();
builder.Services.AddScoped<IGeneratedReportService, GeneratedReportService>();
builder.Services.AddScoped<IReportRecipientService, ReportRecipientService>();
builder.Services.AddScoped<IScheduledReportService, ScheduledReportService>();

// ============================================================
// AI CHAT STREAMING
// ============================================================
builder.Services.AddScoped<IAIChatStreamingService, AIChatStreamingService>();

// File upload service (avatars, etc.)
builder.Services.AddScoped<IFileUploadService, FileUploadService>();

// ============================================================
// RATE LIMITER (Singleton — shared state across requests)
// ============================================================
builder.Services.AddSingleton<IRateLimitService, RateLimitService>();

// ============================================================
// SIGNALR AUDIT LOGGER
// ============================================================
builder.Services.AddScoped<ISignalRAuditService, SignalRAuditService>();

// Landing page service
builder.Services.AddScoped<ILandingPageService, LandingPageService>();

// Boredom feature service
builder.Services.AddScoped<IBoredomService, BoredomService>();

// ============================================================
// AUDIT SERVICE
// ============================================================
builder.Services.AddScoped<IAuditService, AuditService>();

// ============================================================
// GEMINI AI SERVICE
// ============================================================
builder.Services.AddHttpClient<IGeminiService, GeminiService>();

// Follow system service
builder.Services.AddScoped<IFollowService, FollowService>();

// ============================================================
// ASP.NET CORE IDENTITY
// ============================================================
builder.Services.AddIdentity<User, Role>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredUniqueChars = 1;

    options.User.RequireUniqueEmail = true;
    options.SignIn.RequireConfirmedEmail = false;
})
    .AddEntityFrameworkStores<MindoraDbContext>()
    .AddDefaultTokenProviders();

// ============================================================
// COOKIE CONFIGURATION + ROLE-BASED REDIRECT
// ============================================================
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
    // ⚠️ Role-based redirect is handled in Login.cshtml.cs (OnPostAsync)
});


// HEALTH CHECKS

builder.Services.AddHealthChecks();
var app = builder.Build();

// ============================================================
// SEED DATABASE
// ============================================================
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        await SeedData.InitializeAsync(services);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}

// ============================================================
// ERROR HANDLING
// ============================================================
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();


// ============================================================
// PUBLIC PROFILE ROUTE — /u/{id}
// ============================================================
app.MapControllerRoute(
    name: "public-profile",
    pattern: "u/{id:guid}",
    defaults: new { controller = "Profile", action = "Public" });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

// ============================================================
// SIGNALR HUB ENDPOINTS
// ============================================================
app.MapHub<NotificationHub>("/hubs/notifications");
app.MapHub<ChatHub>("/hubs/chat");
app.MapHub<PresenceHub>("/hubs/presence");
app.MapHealthChecks("/health");

// ============================================================
// TEST ENDPOINT (Step 4 verify — পরে সরাব)
// ============================================================
#if DEBUG
app.MapPost("/_dev/test-notify-me", async (
    HttpContext httpContext,
    INotificationDispatcher dispatcher,
    ILogger<Program> logger) =>
{
    var userIdClaim = httpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

    if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
    {
        return Results.Unauthorized();
    }

    var dto = new Mindora.Application.DTOs.Notification.RealtimeNotificationDto
    {
        Title = "🧪 Test Notification",
        Body = $"Step 4 working! {DateTime.UtcNow:HH:mm:ss}",
        Type = "System",
        Channel = "InApp",
        Severity = "Success",
        Icon = "fa-bell",
        ActionUrl = "/Notification",
        IsSoundEnabled = false
    };

    await dispatcher.DispatchAsync(userId, dto);
    logger.LogInformation("🧪 Test notification dispatched to {UserId}", userId);

    return Results.Ok(new
    {
        message = "Test notification dispatched (DB + SignalR)",
        userId = userId,
        sentAt = DateTime.UtcNow
    });
});
#endif

app.Run();