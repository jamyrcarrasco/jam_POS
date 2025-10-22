using jam_POS.Application.DTOs.Responses;

namespace jam_POS.Application.Interfaces
{
    public interface IFileStorageService
    {
        /// <summary>
        /// Uploads an image file to R2 storage with tenant-specific folder structure
        /// </summary>
        /// <param name="fileStream">The file stream to upload</param>
        /// <param name="fileName">Original file name</param>
        /// <param name="contentType">MIME type of the file</param>
        /// <param name="tenantId">Tenant ID for folder organization</param>
        /// <param name="folder">Subfolder within tenant (e.g., "products", "temp")</param>
        /// <returns>FileUploadResponse with upload details</returns>
        Task<FileUploadResponse> UploadImageAsync(Stream fileStream, string fileName, string contentType, int tenantId, string folder = "products");

        /// <summary>
        /// Deletes a file from R2 storage
        /// </summary>
        /// <param name="filePath">The full path to the file in R2</param>
        /// <param name="tenantId">Tenant ID for validation</param>
        /// <returns>True if deletion was successful</returns>
        Task<bool> DeleteFileAsync(string filePath, int tenantId);

        /// <summary>
        /// Generates a public URL for a file
        /// </summary>
        /// <param name="filePath">The file path in R2</param>
        /// <returns>Public URL to access the file</returns>
        string GetPublicUrl(string filePath);

        /// <summary>
        /// Validates if a file is allowed to be uploaded
        /// </summary>
        /// <param name="fileName">Name of the file</param>
        /// <param name="fileSize">Size of the file in bytes</param>
        /// <param name="contentType">MIME type</param>
        /// <returns>True if file is valid</returns>
        bool ValidateFile(string fileName, long fileSize, string contentType);

        /// <summary>
        /// Generates a GUID-based filename while preserving extension
        /// </summary>
        /// <param name="originalFileName">Original file name</param>
        /// <returns>GUID-based filename with extension</returns>
        string GenerateGuidFileName(string originalFileName);
    }
}
