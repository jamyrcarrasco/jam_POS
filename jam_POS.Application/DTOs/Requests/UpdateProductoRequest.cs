using System.ComponentModel.DataAnnotations;

namespace jam_POS.Application.DTOs.Requests
{
    public class UpdateProductoRequest
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Product name is required")]
        [StringLength(200, ErrorMessage = "Product name cannot exceed 200 characters")]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string? Descripcion { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Precio { get; set; }

        [Required(ErrorMessage = "Stock is required")]
        [Range(0, int.MaxValue, ErrorMessage = "Stock cannot be negative")]
        public int Stock { get; set; }

        [StringLength(100, ErrorMessage = "Category cannot exceed 100 characters")]
        public string? Categoria { get; set; }

        [StringLength(50, ErrorMessage = "Barcode cannot exceed 50 characters")]
        public string? CodigoBarras { get; set; }

        [StringLength(500, ErrorMessage = "Image URL cannot exceed 500 characters")]
        public string? ImagenUrl { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Purchase price cannot be negative")]
        public decimal? PrecioCompra { get; set; }

        [Range(0, 100, ErrorMessage = "Profit margin must be between 0 and 100")]
        public decimal? MargenGanancia { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Minimum stock cannot be negative")]
        public int? StockMinimo { get; set; }

        public bool Activo { get; set; } = true;
    }
}
