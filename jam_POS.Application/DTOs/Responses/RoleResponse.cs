namespace jam_POS.Application.DTOs.Responses
{
    public class RoleResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsSystem { get; set; }
        public bool Activo { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int UsersCount { get; set; }
        public int PermissionsCount { get; set; }
        public List<PermissionResponse>? Permissions { get; set; }
    }
}

