using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Mindora.Domain.Entities;
using Mindora.Infrastructure.Configurations;

namespace Mindora.Infrastructure.Persistence.DbContext
{
    public class MindoraDbContext : IdentityDbContext<User, Role, Guid>
    {
        public MindoraDbContext(DbContextOptions<MindoraDbContext> options) : base(options)
        {
        }

        // ========== IDENTITY (Identity নিজে ম্যানেজ করবে) ==========
        // User, Role, UserRoles ইত্যাদির DbSet আলাদা করে দেওয়ার দরকার নেই।

        // ========== APPLICATION SPECIFIC DbSets ==========
        public DbSet<UserProfile> UserProfiles { get; set; } = null!;
        public DbSet<UserSecuritySetting> UserSecuritySettings { get; set; } = null!;
        public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;
        public DbSet<DataPrivacyRequest> DataPrivacyRequests { get; set; } = null!;
        public DbSet<DashboardWidget> DashboardWidgets { get; set; } = null!;
        public DbSet<UserAnalyticsSnapshot> UserAnalyticsSnapshots { get; set; } = null!;
        public DbSet<BoredomRecoveryActivity> BoredomRecoveryActivities { get; set; } = null!;
        public DbSet<BoredomRecoverySession> BoredomRecoverySessions { get; set; } = null!;
        public DbSet<BoxBreathingSession> BoxBreathingSessions { get; set; } = null!;
        public DbSet<Habit> Habits { get; set; } = null!;
        public DbSet<HabitCategory> HabitCategories { get; set; } = null!;
        public DbSet<HabitGoal> HabitGoals { get; set; } = null!;
        public DbSet<HabitLog> HabitLogs { get; set; } = null!;
        public DbSet<HabitReminder> HabitReminders { get; set; } = null!;
        public DbSet<HabitMilestone> HabitMilestones { get; set; } = null!;
        public DbSet<RecoveryStreak> RecoveryStreaks { get; set; } = null!;
        public DbSet<RelapseLog> RelapseLogs { get; set; } = null!;
        public DbSet<SereneRecoveryResource> SereneRecoveryResources { get; set; } = null!;
        public DbSet<SereneRecoverySession> SereneRecoverySessions { get; set; } = null!;
        public DbSet<AssessmentQuestionnaire> AssessmentQuestionnaires { get; set; } = null!;
        public DbSet<AssessmentQuestion> AssessmentQuestions { get; set; } = null!;
        public DbSet<AssessmentOption> AssessmentOptions { get; set; } = null!;
        public DbSet<UserAssessment> UserAssessments { get; set; } = null!;
        public DbSet<UserAssessmentAnswer> UserAssessmentAnswers { get; set; } = null!;
        public DbSet<AssessmentResult> AssessmentResults { get; set; } = null!;
        public DbSet<AIWellnessCoachSession> AIWellnessCoachSessions { get; set; } = null!;
        public DbSet<AIChatMessage> AIChatMessages { get; set; } = null!;
        public DbSet<CommunityPost> CommunityPosts { get; set; } = null!;
        public DbSet<CommunityComment> CommunityComments { get; set; } = null!;
        public DbSet<CommunityReaction> CommunityReactions { get; set; } = null!;
        public DbSet<CommunityGroup> CommunityGroups { get; set; } = null!;
        public DbSet<CommunityGroupMember> CommunityGroupMembers { get; set; } = null!;
        public DbSet<CommunityModerationFlag> CommunityModerationFlags { get; set; } = null!;
        public DbSet<JournalEntry> JournalEntries { get; set; } = null!;
        public DbSet<JournalMood> JournalMoods { get; set; } = null!;
        public DbSet<JournalTag> JournalTags { get; set; } = null!;
        public DbSet<JournalEntryTag> JournalEntryTags { get; set; } = null!;
        public DbSet<ProfessionalProvider> ProfessionalProviders { get; set; } = null!;
        public DbSet<ProviderSpecialty> ProviderSpecialties { get; set; } = null!;
        public DbSet<ProviderSpecialtyMapping> ProviderSpecialtyMappings { get; set; } = null!;
        public DbSet<ProviderPracticeDetail> ProviderPracticeDetails { get; set; } = null!;
        public DbSet<ProviderService> ProviderServices { get; set; } = null!;
        public DbSet<ProviderAvailabilitySlot> ProviderAvailabilitySlots { get; set; } = null!;
        public DbSet<Appointment> Appointments { get; set; } = null!;
        public DbSet<AppointmentStatusHistory> AppointmentStatusHistories { get; set; } = null!;
        public DbSet<Review> Reviews { get; set; } = null!;
        public DbSet<ProviderVerificationDocument> ProviderVerificationDocuments { get; set; } = null!;
        public DbSet<NotificationTemplate> NotificationTemplates { get; set; } = null!;
        public DbSet<UserNotification> UserNotifications { get; set; } = null!;
        public DbSet<UserNotificationPreference> UserNotificationPreferences { get; set; } = null!;
        public DbSet<WellnessResource> WellnessResources { get; set; } = null!;
        public DbSet<WellnessResourceCategory> WellnessResourceCategories { get; set; } = null!;
        public DbSet<WellnessResourceCategoryMapping> WellnessResourceCategoryMappings { get; set; } = null!;
        public DbSet<AdminDashboardMetric> AdminDashboardMetrics { get; set; } = null!;
        public DbSet<ModerationAction> ModerationActions { get; set; } = null!;
        public DbSet<PlatformAnalyticsEvent> PlatformAnalyticsEvents { get; set; } = null!;
        public DbSet<PlatformAnalyticsSnapshot> PlatformAnalyticsSnapshots { get; set; } = null!;
        public DbSet<AuditLog> AuditLogs { get; set; } = null!;
        public DbSet<NotificationLog> NotificationLogs { get; set; } = null!;
        public DbSet<ConsultationSession> ConsultationSessions { get; set; } = null!;
        public DbSet<ConsultationNote> ConsultationNotes { get; set; } = null!;
        public DbSet<ConsultationPrescription> ConsultationPrescriptions { get; set; } = null!;
        public DbSet<ConsultationFeedback> ConsultationFeedbacks { get; set; } = null!;
        public DbSet<FollowUpPlan> FollowUpPlans { get; set; } = null!;
        public DbSet<ConsultationReminder> ConsultationReminders { get; set; } = null!;
        public DbSet<AIPromptTemplate> AIPromptTemplates { get; set; } = null!;
        public DbSet<AIFeedback> AIFeedbacks { get; set; } = null!;
        public DbSet<AIInteractionLog> AIInteractionLogs { get; set; } = null!;
        public DbSet<WellnessScore> WellnessScores { get; set; } = null!;
        public DbSet<RiskPrediction> RiskPredictions { get; set; } = null!;
        public DbSet<SubscriptionPlan> SubscriptionPlans { get; set; } = null!;
        public DbSet<UserSubscription> UserSubscriptions { get; set; } = null!;
        public DbSet<Payment> Payments { get; set; } = null!;
        public DbSet<ConsultationPayment> ConsultationPayments { get; set; } = null!;
        public DbSet<Invoice> Invoices { get; set; } = null!;
        public DbSet<Transaction> Transactions { get; set; } = null!;
        public DbSet<Coupon> Coupons { get; set; } = null!;
        public DbSet<RefundRequest> RefundRequests { get; set; } = null!;
        public DbSet<Payout> Payouts { get; set; } = null!;
        public DbSet<PlatformSetting> PlatformSettings { get; set; } = null!;
        public DbSet<UserLoginHistory> UserLoginHistories { get; set; } = null!;
        public DbSet<ReportRequest> ReportRequests { get; set; } = null!;
        public DbSet<ReportTemplate> ReportTemplates { get; set; } = null!;
        public DbSet<GeneratedReport> GeneratedReports { get; set; } = null!;
        public DbSet<ScheduledReport> ScheduledReports { get; set; } = null!;
        public DbSet<ReportRecipient> ReportRecipients { get; set; } = null!;
        public DbSet<UserFollow> UserFollows { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // Identity-র কনফিগারেশন

            // ========== নিজস্ব কনফিগারেশন (একবারই Apply হবে) ==========
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new RoleConfiguration());
            modelBuilder.ApplyConfiguration(new RefreshTokenConfiguration());
            modelBuilder.ApplyConfiguration(new UserProfileConfiguration());
            modelBuilder.ApplyConfiguration(new UserSecuritySettingConfiguration());
            modelBuilder.ApplyConfiguration(new DataPrivacyRequestConfiguration());
            modelBuilder.ApplyConfiguration(new DashboardWidgetConfiguration());
            modelBuilder.ApplyConfiguration(new UserAnalyticsSnapshotConfiguration());
            modelBuilder.ApplyConfiguration(new BoredomRecoveryActivityConfiguration());
            modelBuilder.ApplyConfiguration(new BoredomRecoverySessionConfiguration());
            modelBuilder.ApplyConfiguration(new BoxBreathingSessionConfiguration());
            modelBuilder.ApplyConfiguration(new HabitConfiguration());
            modelBuilder.ApplyConfiguration(new HabitCategoryConfiguration());
            modelBuilder.ApplyConfiguration(new HabitGoalConfiguration());
            modelBuilder.ApplyConfiguration(new HabitLogConfiguration());
            modelBuilder.ApplyConfiguration(new HabitReminderConfiguration());
            modelBuilder.ApplyConfiguration(new HabitMilestoneConfiguration());
            modelBuilder.ApplyConfiguration(new RecoveryStreakConfiguration());
            modelBuilder.ApplyConfiguration(new RelapseLogConfiguration());
            modelBuilder.ApplyConfiguration(new SereneRecoveryResourceConfiguration());
            modelBuilder.ApplyConfiguration(new SereneRecoverySessionConfiguration());
            modelBuilder.ApplyConfiguration(new AssessmentQuestionnaireConfiguration());
            modelBuilder.ApplyConfiguration(new AssessmentQuestionConfiguration());
            modelBuilder.ApplyConfiguration(new AssessmentOptionConfiguration());
            modelBuilder.ApplyConfiguration(new UserAssessmentConfiguration());
            modelBuilder.ApplyConfiguration(new UserAssessmentAnswerConfiguration());
            modelBuilder.ApplyConfiguration(new AssessmentResultConfiguration());
            modelBuilder.ApplyConfiguration(new AIWellnessCoachSessionConfiguration());
            modelBuilder.ApplyConfiguration(new AIChatMessageConfiguration());
            modelBuilder.ApplyConfiguration(new CommunityPostConfiguration());
            modelBuilder.ApplyConfiguration(new CommunityCommentConfiguration());
            modelBuilder.ApplyConfiguration(new CommunityReactionConfiguration());
            modelBuilder.ApplyConfiguration(new CommunityGroupConfiguration());
            modelBuilder.ApplyConfiguration(new CommunityGroupMemberConfiguration());
            modelBuilder.ApplyConfiguration(new CommunityModerationFlagConfiguration());
            modelBuilder.ApplyConfiguration(new JournalEntryConfiguration());
            modelBuilder.ApplyConfiguration(new JournalMoodConfiguration());
            modelBuilder.ApplyConfiguration(new JournalTagConfiguration());
            modelBuilder.ApplyConfiguration(new JournalEntryTagConfiguration());
            modelBuilder.ApplyConfiguration(new ProfessionalProviderConfiguration());
            modelBuilder.ApplyConfiguration(new ProviderSpecialtyConfiguration());
            modelBuilder.ApplyConfiguration(new ProviderSpecialtyMappingConfiguration());
            modelBuilder.ApplyConfiguration(new ProviderPracticeDetailConfiguration());
            modelBuilder.ApplyConfiguration(new ProviderServiceConfiguration());
            modelBuilder.ApplyConfiguration(new ProviderAvailabilitySlotConfiguration());
            modelBuilder.ApplyConfiguration(new AppointmentConfiguration());
            modelBuilder.ApplyConfiguration(new AppointmentStatusHistoryConfiguration());
            modelBuilder.ApplyConfiguration(new ReviewConfiguration());
            modelBuilder.ApplyConfiguration(new ProviderVerificationDocumentConfiguration());
            modelBuilder.ApplyConfiguration(new NotificationTemplateConfiguration());
            modelBuilder.ApplyConfiguration(new UserNotificationConfiguration());
            modelBuilder.ApplyConfiguration(new UserNotificationPreferenceConfiguration());
            modelBuilder.ApplyConfiguration(new WellnessResourceConfiguration());
            modelBuilder.ApplyConfiguration(new WellnessResourceCategoryConfiguration());
            modelBuilder.ApplyConfiguration(new WellnessResourceCategoryMappingConfiguration());
            modelBuilder.ApplyConfiguration(new AdminDashboardMetricConfiguration());
            modelBuilder.ApplyConfiguration(new ModerationActionConfiguration());
            modelBuilder.ApplyConfiguration(new PlatformAnalyticsEventConfiguration());
            modelBuilder.ApplyConfiguration(new PlatformAnalyticsSnapshotConfiguration());
            modelBuilder.ApplyConfiguration(new AuditLogConfiguration());
            modelBuilder.ApplyConfiguration(new NotificationTemplateConfiguration());
            modelBuilder.ApplyConfiguration(new UserNotificationConfiguration());
            modelBuilder.ApplyConfiguration(new UserNotificationPreferenceConfiguration());
            modelBuilder.ApplyConfiguration(new NotificationLogConfiguration());
            modelBuilder.ApplyConfiguration(new ConsultationSessionConfiguration());
            modelBuilder.ApplyConfiguration(new ConsultationNoteConfiguration());
            modelBuilder.ApplyConfiguration(new ConsultationPrescriptionConfiguration());
            modelBuilder.ApplyConfiguration(new ConsultationFeedbackConfiguration());
            modelBuilder.ApplyConfiguration(new FollowUpPlanConfiguration());
            modelBuilder.ApplyConfiguration(new ConsultationReminderConfiguration());
            modelBuilder.ApplyConfiguration(new AIWellnessCoachSessionConfiguration());
            modelBuilder.ApplyConfiguration(new AIChatMessageConfiguration());
            modelBuilder.ApplyConfiguration(new AIPromptTemplateConfiguration());
            modelBuilder.ApplyConfiguration(new AIFeedbackConfiguration());
            modelBuilder.ApplyConfiguration(new AIInteractionLogConfiguration());
            modelBuilder.ApplyConfiguration(new WellnessScoreConfiguration());
            modelBuilder.ApplyConfiguration(new RiskPredictionConfiguration());
            modelBuilder.ApplyConfiguration(new SubscriptionPlanConfiguration());
            modelBuilder.ApplyConfiguration(new UserSubscriptionConfiguration());
            modelBuilder.ApplyConfiguration(new PaymentConfiguration());
            modelBuilder.ApplyConfiguration(new ConsultationPaymentConfiguration());
            modelBuilder.ApplyConfiguration(new InvoiceConfiguration());
            modelBuilder.ApplyConfiguration(new TransactionConfiguration());
            modelBuilder.ApplyConfiguration(new CouponConfiguration());
            modelBuilder.ApplyConfiguration(new RefundRequestConfiguration());
            modelBuilder.ApplyConfiguration(new PayoutConfiguration());
            modelBuilder.ApplyConfiguration(new PlatformSettingConfiguration());
            modelBuilder.ApplyConfiguration(new UserLoginHistoryConfiguration());
            modelBuilder.ApplyConfiguration(new ReportRequestConfiguration());
            modelBuilder.ApplyConfiguration(new PlatformAnalyticsEventConfiguration());
            modelBuilder.ApplyConfiguration(new PlatformAnalyticsSnapshotConfiguration());
            modelBuilder.ApplyConfiguration(new UserAnalyticsSnapshotConfiguration());
            modelBuilder.ApplyConfiguration(new ReportTemplateConfiguration());
            modelBuilder.ApplyConfiguration(new GeneratedReportConfiguration());
            modelBuilder.ApplyConfiguration(new ScheduledReportConfiguration());
            modelBuilder.ApplyConfiguration(new ReportRecipientConfiguration());
            modelBuilder.ApplyConfiguration(new UserFollowConfiguration());
        }
    }
}