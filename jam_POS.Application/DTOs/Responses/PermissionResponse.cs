namespace jam_POS.Application.DTOs.Responses
{
    public class PermissionResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Module { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsSystem { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

