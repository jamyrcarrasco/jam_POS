using Amazon.S3;
using Amazon.S3.Model;
using jam_POS.Application.DTOs.Common;
using jam_POS.Application.DTOs.Responses;
using jam_POS.Application.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;

namespace jam_POS.Application.Services
{
    public class R2FileStorageService : IFileStorageService
    {
        private readonly IAmazonS3 _s3Client;
        private readonly R2StorageSettings _settings;
        private readonly ILogger<R2FileStorageService> _logger;

        public R2FileStorageService(
            IAmazonS3 s3Client,
            IOptions<R2StorageSettings> settings,
            ILogger<R2FileStorageService> logger)
        {
            _s3Client = s3Client;
            _settings = settings.Value;
            _logger = logger;
        }

        public async Task<FileUploadResponse> UploadImageAsync(Stream fileStream, string fileName, string contentType, int tenantId, string folder = "products")
        {
            try
            {
                // Validate file
                if (!ValidateFile(fileName, fileStream.Length, contentType))
                {
                    return new FileUploadResponse
                    {
                        Success = false,
                        ErrorMessage = "Invalid file type or size"
                    };
                }

                // Generate GUID-based filename
                var guidFileName = GenerateGuidFileName(fileName);
                var filePath = $"tenants/{tenantId}/{folder}/{guidFileName}";

                // Create a copy of the stream to avoid issues with the original stream
                using var memoryStream = new MemoryStream();
                await fileStream.CopyToAsync(memoryStream);
                memoryStream.Position = 0;

                // Process image if it's an image file
                Stream uploadStream;
                if (IsImageFile(contentType))
                {
                    // Create a completely independent stream for image processing
                    var processingStream = new MemoryStream();
                    await memoryStream.CopyToAsync(processingStream);
                    processingStream.Position = 0;
                    
                    uploadStream = await ProcessImageAsync(processingStream);
                }
                else
                {
                    // For non-image files, create a new independent stream
                    uploadStream = new MemoryStream();
                    await memoryStream.CopyToAsync(uploadStream);
                    uploadStream.Position = 0;
                }

                // Ensure the upload stream is at the beginning
                if (uploadStream.CanSeek)
                {
                    uploadStream.Position = 0;
                }

                // Upload to R2
                var request = new PutObjectRequest
                {
                    BucketName = _settings.BucketName,
                    Key = filePath,
                    InputStream = uploadStream,
                    ContentType = contentType,
                    DisablePayloadSigning = true,
                    DisableDefaultChecksumValidation = true,
                    ServerSideEncryptionMethod = ServerSideEncryptionMethod.AES256
                };

                var fileSize = uploadStream.Length;
                _logger.LogInformation("Uploading file to R2: {FilePath} (Size: {Size} bytes)", filePath, fileSize);
                var response = await _s3Client.PutObjectAsync(request);

                var publicUrl = GetPublicUrl(filePath);

                _logger.LogInformation("File uploaded successfully: {FilePath} for tenant {TenantId}", filePath, tenantId);

                // Dispose the upload stream after successful upload
                if (uploadStream is IDisposable disposableStream)
                {
                    disposableStream.Dispose();
                }

                return new FileUploadResponse
                {
                    Success = true,
                    FileName = guidFileName,
                    FileUrl = publicUrl,
                    FilePath = filePath,
                    FileSize = fileSize,
                    ContentType = contentType
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading file {FileName} for tenant {TenantId}", fileName, tenantId);
                return new FileUploadResponse
                {
                    Success = false,
                    ErrorMessage = "Error uploading file: " + ex.Message
                };
            }
        }

        public async Task<bool> DeleteFileAsync(string filePath, int tenantId)
        {
            try
            {
                // Validate that the file belongs to the tenant
                if (!filePath.StartsWith($"tenants/{tenantId}/"))
                {
                    _logger.LogWarning("Attempted to delete file {FilePath} not belonging to tenant {TenantId}", filePath, tenantId);
                    return false;
                }

                var request = new DeleteObjectRequest
                {
                    BucketName = _settings.BucketName,
                    Key = filePath
                };

                await _s3Client.DeleteObjectAsync(request);
                _logger.LogInformation("File deleted successfully: {FilePath}", filePath);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting file {FilePath}", filePath);
                return false;
            }
        }

        public string GetPublicUrl(string filePath)
        {
            return $"{_settings.PublicUrl.TrimEnd('/')}/{filePath}";
        }

        public bool ValidateFile(string fileName, long fileSize, string contentType)
        {
            // Check file size
            if (fileSize > _settings.MaxFileSize)
            {
                _logger.LogWarning("File {FileName} exceeds maximum size limit", fileName);
                return false;
            }

            // Check file extension
            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            if (!_settings.AllowedExtensions.Contains(extension))
            {
                _logger.LogWarning("File {FileName} has unsupported extension {Extension}", fileName, extension);
                return false;
            }

            // Check content type for images
            if (IsImageFile(contentType))
            {
                var allowedImageTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/webp", "image/gif" };
                if (!allowedImageTypes.Contains(contentType.ToLowerInvariant()))
                {
                    _logger.LogWarning("File {FileName} has unsupported content type {ContentType}", fileName, contentType);
                    return false;
                }
            }

            return true;
        }

        public string GenerateGuidFileName(string originalFileName)
        {
            var extension = Path.GetExtension(originalFileName);
            var guid = Guid.NewGuid().ToString("N");
            return $"{guid}{extension}";
        }

        private bool IsImageFile(string contentType)
        {
            return contentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase);
        }

        private async Task<Stream> ProcessImageAsync(Stream originalStream)
        {
            try
            {
                // Ensure stream is at the beginning
                if (originalStream.CanSeek)
                {
                    originalStream.Position = 0;
                }

                using var image = await Image.LoadAsync(originalStream);
                
                // Calculate new dimensions while maintaining aspect ratio
                var (newWidth, newHeight) = CalculateNewDimensions(image.Width, image.Height, _settings.ImageProcessing.MaxWidth, _settings.ImageProcessing.MaxHeight);
                
                // Resize if necessary
                if (newWidth != image.Width || newHeight != image.Height)
                {
                    image.Mutate(x => x.Resize(newWidth, newHeight));
                }

                // Convert to JPEG with specified quality
                var outputStream = new MemoryStream();
                var encoder = new JpegEncoder
                {
                    Quality = _settings.ImageProcessing.Quality
                };

                await image.SaveAsync(outputStream, encoder);
                outputStream.Position = 0;
                
                return outputStream;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing image");
                // Create a new MemoryStream from the original stream if processing fails
                var fallbackStream = new MemoryStream();
                if (originalStream.CanSeek)
                {
                    originalStream.Position = 0;
                }
                await originalStream.CopyToAsync(fallbackStream);
                fallbackStream.Position = 0;
                return fallbackStream;
            }
        }

        private static (int width, int height) CalculateNewDimensions(int originalWidth, int originalHeight, int maxWidth, int maxHeight)
        {
            if (originalWidth <= maxWidth && originalHeight <= maxHeight)
            {
                return (originalWidth, originalHeight);
            }

            var widthRatio = (double)maxWidth / originalWidth;
            var heightRatio = (double)maxHeight / originalHeight;
            var ratio = Math.Min(widthRatio, heightRatio);

            return (
                (int)(originalWidth * ratio),
                (int)(originalHeight * ratio)
            );
        }
    }
}