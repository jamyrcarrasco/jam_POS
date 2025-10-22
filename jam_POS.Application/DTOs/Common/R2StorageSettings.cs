namespace jam_POS.Application.DTOs.Common
{
    public class R2StorageSettings
    {
        public string AccountId { get; set; } = string.Empty;
        public string AccessKeyId { get; set; } = string.Empty;
        public string SecretAccessKey { get; set; } = string.Empty;
        public string BucketName { get; set; } = string.Empty;
        public string PublicUrl { get; set; } = string.Empty;
        public long MaxFileSize { get; set; } = 10485760; // 10MB
        public string[] AllowedExtensions { get; set; } = { ".jpg", ".jpeg", ".png", ".webp", ".gif" };
        public ImageProcessingSettings ImageProcessing { get; set; } = new();
    }

    public class ImageProcessingSettings
    {
        public int MaxWidth { get; set; } = 1920;
        public int MaxHeight { get; set; } = 1080;
        public int Quality { get; set; } = 85;
    }
}
