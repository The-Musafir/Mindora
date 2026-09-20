using Microsoft.EntityFrameworkCore;
using Mindora.Application.DTOs.Reporting;
using Mindora.Application.Interfaces;
using Mindora.Domain.Entities;
using Mindora.Infrastructure.Persistence.DbContext;

namespace Mindora.Infrastructure.Services
{
    public class ReportTemplateService : IReportTemplateService
    {
        private readonly MindoraDbContext _context;

        public ReportTemplateService(MindoraDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<ReportTemplateDto>> GetAllTemplatesAsync()
        {
            var templates = await _context.ReportTemplates
                .OrderBy(t => t.Category)
                .ThenBy(t => t.Name)
                .ToListAsync();

            return templates.Select(MapToDto).ToList();
        }

        public async Task<IReadOnlyList<ReportTemplateDto>> GetActiveTemplatesAsync()
        {
            var templates = await _context.ReportTemplates
                .Where(t => t.IsActive)
                .OrderBy(t => t.Category)
                .ThenBy(t => t.Name)
                .ToListAsync();

            return templates.Select(MapToDto).ToList();
        }

        public async Task<ReportTemplateDto?> GetTemplateByIdAsync(Guid templateId)
        {
            var template = await _context.ReportTemplates.FindAsync(templateId);
            return template == null ? null : MapToDto(template);
        }

        public async Task<ReportTemplateDto?> GetTemplateByKeyAsync(string templateKey)
        {
            var template = await _context.ReportTemplates
                .FirstOrDefaultAsync(t => t.TemplateKey == templateKey);
            return template == null ? null : MapToDto(template);
        }

        public async Task<Guid> CreateTemplateAsync(CreateReportTemplateRequest request)
        {
            var template = new ReportTemplate
            {
                TemplateId = Guid.NewGuid(),
                TemplateKey = request.TemplateKey,
                Name = request.Name,
                Description = request.Description,
                Category = request.Category,
                DefaultFormat = request.DefaultFormat,
                IsActive = request.IsActive,
                CreatedAt = DateTime.UtcNow
            };

            _context.ReportTemplates.Add(template);
            await _context.SaveChangesAsync();
            return template.TemplateId;
        }

        public async Task<bool> UpdateTemplateAsync(ReportTemplateDto dto)
        {
            var template = await _context.ReportTemplates.FindAsync(dto.TemplateId);
            if (template == null) return false;

            template.Name = dto.Name;
            template.Description = dto.Description;
            template.Category = dto.Category;
            template.DefaultFormat = dto.DefaultFormat;
            template.IsActive = dto.IsActive;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ToggleTemplateActiveAsync(Guid templateId, bool isActive)
        {
            var template = await _context.ReportTemplates.FindAsync(templateId);
            if (template == null) return false;

            template.IsActive = isActive;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteTemplateAsync(Guid templateId)
        {
            var template = await _context.ReportTemplates.FindAsync(templateId);
            if (template == null) return false;

            _context.ReportTemplates.Remove(template);
            await _context.SaveChangesAsync();
            return true;
        }

        private ReportTemplateDto MapToDto(ReportTemplate t)
        {
            return new ReportTemplateDto
            {
                TemplateId = t.TemplateId,
                TemplateKey = t.TemplateKey,
                Name = t.Name,
                Description = t.Description,
                Category = t.Category,
                DefaultFormat = t.DefaultFormat,
                IsActive = t.IsActive,
                CreatedAt = t.CreatedAt
            };
        }
    }
}