namespace jam_POS.Application.DTOs.Responses
{
    public class ProductoResponse
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public decimal Precio { get; set; }
        public int Stock { get; set; }
        public int? CategoriaId { get; set; }
        public string? CategoriaNombre { get; set; }
        public string? CodigoBarras { get; set; }
        public string? ImagenUrl { get; set; }
        public decimal? PrecioCompra { get; set; }
        public decimal? MargenGanancia { get; set; }
        public int? StockMinimo { get; set; }
        public bool Activo { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
