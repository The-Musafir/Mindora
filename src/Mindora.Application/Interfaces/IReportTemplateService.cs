using Mindora.Application.DTOs.Reporting;

namespace Mindora.Application.Interfaces
{
    public interface IReportTemplateService
    {
        Task<IReadOnlyList<ReportTemplateDto>> GetAllTemplatesAsync();
        Task<IReadOnlyList<ReportTemplateDto>> GetActiveTemplatesAsync();
        Task<ReportTemplateDto?> GetTemplateByIdAsync(Guid templateId);
        Task<ReportTemplateDto?> GetTemplateByKeyAsync(string templateKey);
        Task<Guid> CreateTemplateAsync(CreateReportTemplateRequest request);
        Task<bool> UpdateTemplateAsync(ReportTemplateDto dto);
        Task<bool> ToggleTemplateActiveAsync(Guid templateId, bool isActive);
        Task<bool> DeleteTemplateAsync(Guid templateId);
    }
}