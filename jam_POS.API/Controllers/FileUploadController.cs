using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using jam_POS.Application.DTOs.Responses;
using jam_POS.Application.Interfaces;
using jam_POS.API.Middleware;
using System.Security.Claims;

namespace jam_POS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FileUploadController : ControllerBase
    {
        private readonly IFileStorageService _fileStorageService;
        private readonly ILogger<FileUploadController> _logger;

        public FileUploadController(
            IFileStorageService fileStorageService,
            ILogger<FileUploadController> logger)
        {
            _fileStorageService = fileStorageService;
            _logger = logger;
        }

        /// <summary>
        /// Uploads an image file for a product
        /// </summary>
        /// <param name="file">The image file to upload</param>
        /// <param name="folder">Optional folder name (default: products)</param>
        /// <returns>File upload response with URL and details</returns>
        [HttpPost("upload-image")]
        [ProducesResponseType(typeof(FileUploadResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<FileUploadResponse>> UploadImage(IFormFile file, string folder = "products")
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest(new ErrorResponse { Message = "No file provided" });
                }

                // Get tenant ID from claims
                var tenantIdClaim = User.FindFirst("TenantId")?.Value;
                if (!int.TryParse(tenantIdClaim, out var tenantId))
                {
                    return BadRequest(new ErrorResponse { Message = "Invalid tenant context" });
                }

                // Validate file
                if (!_fileStorageService.ValidateFile(file.FileName, file.Length, file.ContentType))
                {
                    return BadRequest(new ErrorResponse { Message = "Invalid file type or size" });
                }

                // Upload file
                using var stream = file.OpenReadStream();
                var result = await _fileStorageService.UploadImageAsync(
                    stream, 
                    file.FileName, 
                    file.ContentType, 
                    tenantId, 
                    folder
                );

                if (!result.Success)
                {
                    return BadRequest(new ErrorResponse { Message = result.ErrorMessage ?? "Upload failed" });
                }

                _logger.LogInformation("Image uploaded successfully for tenant {TenantId}: {FileName}", tenantId, result.FileName);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading image");
                return StatusCode(500, new ErrorResponse { Message = "Internal server error during upload" });
            }
        }

        /// <summary>
        /// Deletes an uploaded file
        /// </summary>
        /// <param name="filePath">The path of the file to delete</param>
        /// <returns>Success status</returns>
        [HttpDelete("delete")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteFile([FromQuery] string filePath)
        {
            try
            {
                if (string.IsNullOrEmpty(filePath))
                {
                    return BadRequest(new ErrorResponse { Message = "File path is required" });
                }

                // Get tenant ID from claims
                var tenantIdClaim = User.FindFirst("TenantId")?.Value;
                if (!int.TryParse(tenantIdClaim, out var tenantId))
                {
                    return BadRequest(new ErrorResponse { Message = "Invalid tenant context" });
                }

                var success = await _fileStorageService.DeleteFileAsync(filePath, tenantId);
                
                if (!success)
                {
                    return BadRequest(new ErrorResponse { Message = "Failed to delete file" });
                }

                _logger.LogInformation("File deleted successfully for tenant {TenantId}: {FilePath}", tenantId, filePath);
                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting file {FilePath}", filePath);
                return StatusCode(500, new ErrorResponse { Message = "Internal server error during deletion" });
            }
        }

        /// <summary>
        /// Gets the public URL for a file
        /// </summary>
        /// <param name="filePath">The file path</param>
        /// <returns>Public URL</returns>
        [HttpGet("url")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        public ActionResult<string> GetFileUrl([FromQuery] string filePath)
        {
            try
            {
                if (string.IsNullOrEmpty(filePath))
                {
                    return BadRequest(new ErrorResponse { Message = "File path is required" });
                }

                var publicUrl = _fileStorageService.GetPublicUrl(filePath);
                return Ok(publicUrl);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting file URL for {FilePath}", filePath);
                return StatusCode(500, new ErrorResponse { Message = "Internal server error" });
            }
        }

    }
}
