using Mindora.Application.Interfaces;

namespace Mindora.Web.Services
{
    /// <summary>
    /// Local filesystem file upload service.
    /// Files stored in wwwroot/uploads/.
    /// </summary>
    public class FileUploadService : IFileUploadService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<FileUploadService> _logger;

        // Upload constraints
        private const long MAX_AVATAR_SIZE = 2 * 1024 * 1024;  // 2 MB
        private static readonly string[] ALLOWED_EXTENSIONS = { ".jpg", ".jpeg", ".png", ".webp" };
        private static readonly string[] ALLOWED_MIME_TYPES =
        {
            "image/jpeg",
            "image/png",
            "image/webp"
        };

        public FileUploadService(
            IWebHostEnvironment environment,
            ILogger<FileUploadService> logger)
        {
            _environment = environment;
            _logger = logger;
        }

        public async Task<string?> UploadAvatarAsync(
            Guid userId,
            Stream fileStream,
            string fileName,
            string contentType,
            long fileSize,
            CancellationToken cancellationToken = default)
        {
            var (isValid, error) = ValidateImage(fileName, contentType, fileSize);
            if (!isValid)
            {
                _logger.LogWarning("Avatar upload rejected for user {UserId}: {Error}",
                    userId, error);
                return null;
            }

            try
            {
                // Path: wwwroot/uploads/avatars/
                var uploadsFolder = Path.Combine(
                    _environment.WebRootPath,
                    "uploads",
                    "avatars");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                // Generate unique filename
                var extension = Path.GetExtension(fileName).ToLowerInvariant();
                var newFileName = $"user-{userId:N}-{Guid.NewGuid():N}{extension}";
                var filePath = Path.Combine(uploadsFolder, newFileName);

                // Save file
                using (var destination = new FileStream(filePath, FileMode.Create))
                {
                    await fileStream.CopyToAsync(destination, cancellationToken);
                }

                // Return relative URL
                var relativeUrl = $"/uploads/avatars/{newFileName}";

                _logger.LogInformation("Avatar uploaded for user {UserId}: {Url}",
                    userId, relativeUrl);

                return relativeUrl;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to upload avatar for user {UserId}", userId);
                return null;
            }
        }

        public Task<bool> DeleteFileAsync(string relativeUrl, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(relativeUrl))
                return Task.FromResult(false);

            try
            {
                // Sanitize: ensure it starts with /uploads/
                if (!relativeUrl.StartsWith("/uploads/", StringComparison.OrdinalIgnoreCase))
                {
                    _logger.LogWarning("Attempted to delete file outside /uploads/: {Url}", relativeUrl);
                    return Task.FromResult(false);
                }

                var relativePath = relativeUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
                var fullPath = Path.Combine(_environment.WebRootPath, relativePath);

                // Security: ensure path is still inside wwwroot/uploads
                var fullUploadsPath = Path.GetFullPath(Path.Combine(_environment.WebRootPath, "uploads"));
                var fullFilePath = Path.GetFullPath(fullPath);

                if (!fullFilePath.StartsWith(fullUploadsPath, StringComparison.OrdinalIgnoreCase))
                {
                    _logger.LogWarning("Path traversal attempt: {Url}", relativeUrl);
                    return Task.FromResult(false);
                }

                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                    _logger.LogInformation("File deleted: {Url}", relativeUrl);
                    return Task.FromResult(true);
                }

                return Task.FromResult(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete file: {Url}", relativeUrl);
                return Task.FromResult(false);
            }
        }

        public (bool IsValid, string? Error) ValidateImage(string fileName, string contentType, long fileSize)
        {
            if (fileSize <= 0)
                return (false, "No file provided.");

            if (fileSize > MAX_AVATAR_SIZE)
                return (false, $"File is too large. Maximum size is {MAX_AVATAR_SIZE / 1024 / 1024} MB.");

            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            if (!ALLOWED_EXTENSIONS.Contains(extension))
                return (false, $"Only {string.Join(", ", ALLOWED_EXTENSIONS)} files are allowed.");

            if (!ALLOWED_MIME_TYPES.Contains(contentType.ToLowerInvariant()))
                return (false, "Invalid file type. Please upload a valid image.");

            return (true, null);
        }
    }
}