namespace jam_POS.Application.DTOs.Requests
{
    public class CategoriaFilterRequest
    {
        public string? SearchTerm { get; set; }
        public bool? Activo { get; set; }
        public string? OrderBy { get; set; } = "nombre";
        public bool OrderDescending { get; set; } = false;
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}

