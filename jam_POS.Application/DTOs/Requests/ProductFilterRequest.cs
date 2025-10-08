namespace jam_POS.Application.DTOs.Requests
{
    public class ProductFilterRequest
    {
        public string? SearchTerm { get; set; }
        public string? Categoria { get; set; }
        public decimal? PrecioMin { get; set; }
        public decimal? PrecioMax { get; set; }
        public bool? StockBajo { get; set; }
        public bool? Activo { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? OrderBy { get; set; } = "Nombre";
        public bool OrderDescending { get; set; } = false;
    }
}

