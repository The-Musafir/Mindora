namespace Mindora.Application.Interfaces
{
   
    public interface IFileUploadService
    {
        
        Task<string?> UploadAvatarAsync(
            Guid userId,
            Stream fileStream,
            string fileName,
            string contentType,
            long fileSize,
            CancellationToken cancellationToken = default);

        
        Task<bool> DeleteFileAsync(string relativeUrl, CancellationToken cancellationToken = default);

       
        (bool IsValid, string? Error) ValidateImage(string fileName, string contentType, long fileSize);
    }
}