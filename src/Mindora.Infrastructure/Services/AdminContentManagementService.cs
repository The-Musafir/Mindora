using Microsoft.EntityFrameworkCore;
using Mindora.Application.DTOs.AdminDashboard;
using Mindora.Application.Interfaces;
using Mindora.Domain.Entities;
using Mindora.Infrastructure.Persistence.DbContext;

namespace Mindora.Infrastructure.Services
{
    public class AdminContentManagementService : IAdminContentManagementService
    {
        private readonly MindoraDbContext _context;

        public AdminContentManagementService(MindoraDbContext context)
        {
            _context = context;
        }

        // ============ WELLNESS RESOURCES ============
        public async Task<IReadOnlyList<AdminWellnessResourceDto>> GetAllWellnessResourcesAsync()
        {
            var resources = await _context.WellnessResources
                .OrderByDescending(r => r.ResourceId)
                .ToListAsync();

            return resources.Select(r => new AdminWellnessResourceDto
            {
                ResourceId = r.ResourceId,
                Title = r.Title,
                ContentType = r.ContentType,
                Url = r.Url,
                IsPublished = r.IsPublished
            }).ToList();
        }

        public async Task<Guid> CreateWellnessResourceAsync(AdminWellnessResourceDto dto)
        {
            var resource = new WellnessResource
            {
                ResourceId = Guid.NewGuid(),
                Title = dto.Title,
                Description = null,
                ContentType = dto.ContentType,
                Url = dto.Url,
                IsPublished = dto.IsPublished
            };

            _context.WellnessResources.Add(resource);
            await _context.SaveChangesAsync();
            return resource.ResourceId;
        }

        public async Task<bool> ToggleWellnessResourceAsync(Guid resourceId, bool isPublished)
        {
            var resource = await _context.WellnessResources.FindAsync(resourceId);
            if (resource == null) return false;

            resource.IsPublished = isPublished;
            await _context.SaveChangesAsync();
            return true;
        }

        // ============ ASSESSMENTS ============
        public async Task<IReadOnlyList<AdminAssessmentQuestionnaireDto>> GetAllAssessmentsAsync()
        {
            var assessments = await _context.AssessmentQuestionnaires
                .OrderByDescending(q => q.QuestionnaireId)
                .ToListAsync();

            return assessments.Select(q => new AdminAssessmentQuestionnaireDto
            {
                QuestionnaireId = q.QuestionnaireId,
                Title = q.Title,
                Description = q.Description,
                IsActive = q.IsActive
            }).ToList();
        }

        public async Task<Guid> CreateAssessmentAsync(AdminAssessmentQuestionnaireDto dto)
        {
            var questionnaire = new AssessmentQuestionnaire
            {
                QuestionnaireId = Guid.NewGuid(),
                Title = dto.Title,
                Description = dto.Description,
                IsActive = dto.IsActive
            };

            _context.AssessmentQuestionnaires.Add(questionnaire);
            await _context.SaveChangesAsync();
            return questionnaire.QuestionnaireId;
        }

        public async Task<bool> ToggleAssessmentAsync(Guid questionnaireId, bool isActive)
        {
            var questionnaire = await _context.AssessmentQuestionnaires.FindAsync(questionnaireId);
            if (questionnaire == null) return false;

            questionnaire.IsActive = isActive;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}