using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Mindora.Domain.Entities;
using Mindora.Infrastructure.Persistence.DbContext;

namespace Mindora.Infrastructure.Persistence
{
    public static class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<Role>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<User>>();
            var dbContext = serviceProvider.GetRequiredService<MindoraDbContext>();
            var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger("SeedData");

            await SeedRolesAsync(roleManager, logger);
            await SeedAdminUserAsync(userManager, logger);
            await SeedHabitCategoriesAsync(dbContext, logger);
            await SeedAssessmentQuestionnairesAsync(dbContext, logger);
            await SeedProviderSpecialtiesAsync(dbContext, logger);
            await SeedDemoProviderAsync(dbContext, userManager, logger);
            await SeedSubscriptionPlansAsync(dbContext, logger);
            await SeedBoredomActivitiesAsync(dbContext, logger);
        }

        private static async Task SeedRolesAsync(RoleManager<Role> roleManager, ILogger logger)
        {
            string[] roleNames = { "Admin", "User", "Professional" };

            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    var result = await roleManager.CreateAsync(new Role { Name = roleName });
                    if (result.Succeeded)
                        logger.LogInformation("Seeded role: {RoleName}", roleName);
                }
            }
        }

        private static async Task SeedAdminUserAsync(UserManager<User> userManager, ILogger logger)
        {
            const string adminEmail = "bithix29@gmail.com";
            const string adminPassword = "Bithix_29";

            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                var adminUser = new User
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true,
                    CreatedAt = DateTime.UtcNow
                };

                var createResult = await userManager.CreateAsync(adminUser, adminPassword);
                if (createResult.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                    logger.LogInformation("Seeded admin user: {Email}", adminEmail);
                }
                else
                {
                    foreach (var error in createResult.Errors)
                        logger.LogError("Admin user creation error: {Description}", error.Description);
                }
            }
        }

        private static async Task SeedHabitCategoriesAsync(MindoraDbContext dbContext, ILogger logger)
        {
            if (await dbContext.HabitCategories.AnyAsync())
            {
                logger.LogInformation("Habit categories already exist. Skipping.");
                return;
            }

            var categories = new List<HabitCategory>
            {
                new HabitCategory { HabitCategoryId = Guid.NewGuid(), Name = "Health", Description = "Physical and mental health related habits" },
                new HabitCategory { HabitCategoryId = Guid.NewGuid(), Name = "Fitness", Description = "Exercise, workout and physical activity" },
                new HabitCategory { HabitCategoryId = Guid.NewGuid(), Name = "Reading", Description = "Books, articles and learning" },
                new HabitCategory { HabitCategoryId = Guid.NewGuid(), Name = "Prayer", Description = "Spiritual and religious practice" },
                new HabitCategory { HabitCategoryId = Guid.NewGuid(), Name = "Meditation", Description = "Mindfulness and meditation" },
                new HabitCategory { HabitCategoryId = Guid.NewGuid(), Name = "Study", Description = "Academic and skill development" },
                new HabitCategory { HabitCategoryId = Guid.NewGuid(), Name = "Recovery", Description = "Bad habit recovery and addiction management" }
            };

            dbContext.HabitCategories.AddRange(categories);
            await dbContext.SaveChangesAsync();
            logger.LogInformation("Seeded {Count} habit categories.", categories.Count);
        }

        private static async Task SeedAssessmentQuestionnairesAsync(MindoraDbContext dbContext, ILogger logger)
        {
            if (await dbContext.AssessmentQuestionnaires.AnyAsync())
            {
                logger.LogInformation("Assessment questionnaires already exist. Skipping.");
                return;
            }

            var questionnaire = new AssessmentQuestionnaire
            {
                QuestionnaireId = Guid.NewGuid(),
                Title = "GAD-7 (Anxiety Assessment)",
                Description = "Generalized Anxiety Disorder 7-item scale",
                IsActive = true
            };

            var question1 = new AssessmentQuestion
            {
                QuestionId = Guid.NewGuid(),
                QuestionnaireId = questionnaire.QuestionnaireId,
                QuestionText = "Feeling nervous, anxious, or on edge?",
                OrderIndex = 1
            };

            question1.Options.Add(new AssessmentOption { OptionId = Guid.NewGuid(), QuestionId = question1.QuestionId, OptionText = "Not at all", ScoreValue = 0 });
            question1.Options.Add(new AssessmentOption { OptionId = Guid.NewGuid(), QuestionId = question1.QuestionId, OptionText = "Several days", ScoreValue = 1 });
            question1.Options.Add(new AssessmentOption { OptionId = Guid.NewGuid(), QuestionId = question1.QuestionId, OptionText = "More than half the days", ScoreValue = 2 });
            question1.Options.Add(new AssessmentOption { OptionId = Guid.NewGuid(), QuestionId = question1.QuestionId, OptionText = "Nearly every day", ScoreValue = 3 });

            var question2 = new AssessmentQuestion
            {
                QuestionId = Guid.NewGuid(),
                QuestionnaireId = questionnaire.QuestionnaireId,
                QuestionText = "Trouble relaxing?",
                OrderIndex = 2
            };

            question2.Options.Add(new AssessmentOption { OptionId = Guid.NewGuid(), QuestionId = question2.QuestionId, OptionText = "Not at all", ScoreValue = 0 });
            question2.Options.Add(new AssessmentOption { OptionId = Guid.NewGuid(), QuestionId = question2.QuestionId, OptionText = "Several days", ScoreValue = 1 });
            question2.Options.Add(new AssessmentOption { OptionId = Guid.NewGuid(), QuestionId = question2.QuestionId, OptionText = "More than half the days", ScoreValue = 2 });
            question2.Options.Add(new AssessmentOption { OptionId = Guid.NewGuid(), QuestionId = question2.QuestionId, OptionText = "Nearly every day", ScoreValue = 3 });

            questionnaire.Questions.Add(question1);
            questionnaire.Questions.Add(question2);

            dbContext.AssessmentQuestionnaires.Add(questionnaire);
            await dbContext.SaveChangesAsync();
            logger.LogInformation("Seeded assessment questionnaire with 2 questions.");
        }

        private static async Task SeedProviderSpecialtiesAsync(MindoraDbContext dbContext, ILogger logger)
        {
            if (await dbContext.ProviderSpecialties.AnyAsync())
            {
                logger.LogInformation("Provider specialties already exist. Skipping.");
                return;
            }

            var specialties = new List<ProviderSpecialty>
            {
                new ProviderSpecialty { SpecialtyId = Guid.NewGuid(), Name = "Anxiety", Description = "Anxiety disorders, panic attacks" },
                new ProviderSpecialty { SpecialtyId = Guid.NewGuid(), Name = "Depression", Description = "Mood disorders, persistent sadness" },
                new ProviderSpecialty { SpecialtyId = Guid.NewGuid(), Name = "Addiction Recovery", Description = "Substance and behavioral addiction" },
                new ProviderSpecialty { SpecialtyId = Guid.NewGuid(), Name = "Trauma & PTSD", Description = "Trauma recovery and post-traumatic stress" },
                new ProviderSpecialty { SpecialtyId = Guid.NewGuid(), Name = "Stress Management", Description = "Work-life balance, burnout" },
                new ProviderSpecialty { SpecialtyId = Guid.NewGuid(), Name = "Relationship Counseling", Description = "Couples and family therapy" },
                new ProviderSpecialty { SpecialtyId = Guid.NewGuid(), Name = "Child & Adolescent", Description = "Mental health for young people" }
            };

            dbContext.ProviderSpecialties.AddRange(specialties);
            await dbContext.SaveChangesAsync();
            logger.LogInformation("Seeded {Count} provider specialties.", specialties.Count);
        }

        private static async Task SeedDemoProviderAsync(MindoraDbContext dbContext, UserManager<User> userManager, ILogger logger)
        {
            if (await dbContext.ProfessionalProviders.AnyAsync())
            {
                logger.LogInformation("Professional providers already exist. Skipping demo provider.");
                return;
            }

            const string providerEmail = "provider@mindora.com";
            const string providerPassword = "Provider_123";

            var providerUser = await userManager.FindByEmailAsync(providerEmail);
            if (providerUser == null)
            {
                providerUser = new User
                {
                    UserName = providerEmail,
                    Email = providerEmail,
                    EmailConfirmed = true,
                    CreatedAt = DateTime.UtcNow
                };

                var userResult = await userManager.CreateAsync(providerUser, providerPassword);
                if (!userResult.Succeeded)
                {
                    foreach (var error in userResult.Errors)
                        logger.LogError("Demo provider user creation error: {Description}", error.Description);
                    return;
                }

                await userManager.AddToRoleAsync(providerUser, "Professional");
            }

            var anxietySpecialty = await dbContext.ProviderSpecialties
                .FirstOrDefaultAsync(s => s.Name == "Anxiety");
            if (anxietySpecialty == null)
            {
                anxietySpecialty = new ProviderSpecialty { SpecialtyId = Guid.NewGuid(), Name = "Anxiety" };
                dbContext.ProviderSpecialties.Add(anxietySpecialty);
                await dbContext.SaveChangesAsync();
            }

            var provider = new ProfessionalProvider
            {
                ProviderId = Guid.NewGuid(),
                UserId = providerUser.Id,
                Bio = "Experienced clinical psychologist specializing in anxiety and stress management.",
                LicenseNumber = "PSY123456",
                YearsOfExperience = 10,
                IsVerified = true,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            provider.SpecialtyMappings.Add(new ProviderSpecialtyMapping
            {
                ProviderId = provider.ProviderId,
                SpecialtyId = anxietySpecialty.SpecialtyId
            });

            var practice = new ProviderPracticeDetail
            {
                PracticeId = Guid.NewGuid(),
                ProviderId = provider.ProviderId,
                PracticeName = "Mindora Online Clinic",
                Address = "Chittagong, Bangladesh",
                Phone = "+8801608410224",
                IsVirtual = true
            };

            var service = new ProviderService
            {
                ServiceId = Guid.NewGuid(),
                PracticeId = practice.PracticeId,
                ServiceName = "One-on-One Therapy Session",
                DurationMinutes = 60,
                Fee = 1500
            };

            var slot1 = new ProviderAvailabilitySlot
            {
                SlotId = Guid.NewGuid(),
                PracticeId = practice.PracticeId,
                DayOfWeek = 1,
                StartTime = new TimeSpan(9, 0, 0),
                EndTime = new TimeSpan(10, 0, 0),
                IsRecurring = true,
                SpecificDate = null
            };

            var slot2 = new ProviderAvailabilitySlot
            {
                SlotId = Guid.NewGuid(),
                PracticeId = practice.PracticeId,
                DayOfWeek = 3,
                StartTime = new TimeSpan(14, 0, 0),
                EndTime = new TimeSpan(15, 0, 0),
                IsRecurring = true,
                SpecificDate = null
            };

            provider.PracticeDetails.Add(practice);
            practice.Services.Add(service);
            practice.AvailabilitySlots.Add(slot1);
            practice.AvailabilitySlots.Add(slot2);

            dbContext.ProfessionalProviders.Add(provider);
            await dbContext.SaveChangesAsync();
            logger.LogInformation("Seeded demo provider with practice, service and availability slots.");
        }

        private static async Task SeedSubscriptionPlansAsync(MindoraDbContext dbContext, ILogger logger)
        {
            if (await dbContext.SubscriptionPlans.AnyAsync())
            {
                logger.LogInformation("Subscription plans already exist. Skipping.");
                return;
            }

            var plans = new List<SubscriptionPlan>
            {
                new SubscriptionPlan { PlanId = Guid.NewGuid(), Name = "Free", Description = "Habit Tracking, Basic Journal, Basic Assessment", Price = 0, DurationDays = 36500, IsActive = true, CreatedAt = DateTime.UtcNow },
                new SubscriptionPlan { PlanId = Guid.NewGuid(), Name = "Premium", Description = "AI Insights, Advanced Assessment, Unlimited Journal, Mood Analytics", Price = 9.99m, DurationDays = 30, IsActive = true, CreatedAt = DateTime.UtcNow },
                new SubscriptionPlan { PlanId = Guid.NewGuid(), Name = "Professional Plus", Description = "Consultation Booking, Advanced AI Coach, Priority Support", Price = 19.99m, DurationDays = 30, IsActive = true, CreatedAt = DateTime.UtcNow }
            };

            dbContext.SubscriptionPlans.AddRange(plans);
            await dbContext.SaveChangesAsync();
            logger.LogInformation("Seeded {Count} subscription plans.", plans.Count);
        }

        private static async Task SeedBoredomActivitiesAsync(MindoraDbContext dbContext, ILogger logger)
        {
            if (await dbContext.BoredomRecoveryActivities.AnyAsync())
            {
                logger.LogInformation("Boredom activities already seeded. Skipping.");
                return;
            }

            var jsonPath = Path.Combine(AppContext.BaseDirectory, "SeedData", "boredom-activities.json");
            if (!File.Exists(jsonPath))
            {
                logger.LogWarning("Boredom activities JSON not found at {Path}. Skipping seed.", jsonPath);
                return;
            }

            var json = await File.ReadAllTextAsync(jsonPath);
            using var doc = System.Text.Json.JsonDocument.Parse(json);
            var list = new List<BoredomRecoveryActivity>();

            foreach (var el in doc.RootElement.EnumerateArray())
            {
                list.Add(new BoredomRecoveryActivity
                {
                    ActivityId = Guid.NewGuid(),
                    Title = el.GetProperty("title").GetString() ?? "Untitled",
                    Description = el.TryGetProperty("description", out var d) ? d.GetString() : null,
                    Category = el.GetProperty("category").GetString() ?? "General",
                    DurationMinutes = el.TryGetProperty("durationMinutes", out var dm) && dm.ValueKind != System.Text.Json.JsonValueKind.Null ? dm.GetInt32() : (int?)null,
                    Emoji = el.TryGetProperty("emoji", out var em) ? em.GetString() : null,
                    Trivia = el.TryGetProperty("trivia", out var tv) ? tv.GetString() : null,
                    IsActive = true
                });
            }

            dbContext.BoredomRecoveryActivities.AddRange(list);
            await dbContext.SaveChangesAsync();
            logger.LogInformation("Seeded {Count} boredom recovery activities.", list.Count);
        }
    }
}





