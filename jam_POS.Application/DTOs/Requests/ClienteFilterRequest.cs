namespace jam_POS.Application.DTOs.Requests
{
    public class ClienteFilterRequest
    {
        public string? SearchTerm { get; set; }
        public bool? Activo { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? OrderBy { get; set; } = "Nombre";
        public bool OrderDescending { get; set; } = false;
    }
}
