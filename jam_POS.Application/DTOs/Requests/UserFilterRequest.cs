namespace jam_POS.Application.DTOs.Requests
{
    public class UserFilterRequest
    {
        public string? SearchTerm { get; set; }
        public int? RoleId { get; set; }
        public bool? IsActive { get; set; }
        public string? OrderBy { get; set; } = "username";
        public bool OrderDescending { get; set; } = false;
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}

