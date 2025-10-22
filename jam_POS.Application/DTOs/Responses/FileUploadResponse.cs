namespace jam_POS.Application.DTOs.Responses
{
    public class FileUploadResponse
    {
        public bool Success { get; set; }
        public string? FileName { get; set; }
        public string? FileUrl { get; set; }
        public string? FilePath { get; set; }
        public long FileSize { get; set; }
        public string? ContentType { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
