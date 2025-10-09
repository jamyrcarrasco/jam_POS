using System.ComponentModel.DataAnnotations;

namespace jam_POS.Application.DTOs.Requests
{
    public class UpdateProductoRequest
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre del producto es requerido")]
        [StringLength(200, ErrorMessage = "El nombre del producto no puede exceder 200 caracteres")]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "La descripción no puede exceder 500 caracteres")]
        public string? Descripcion { get; set; }

        [Required(ErrorMessage = "El precio es requerido")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor que 0")]
        public decimal Precio { get; set; }

        [Required(ErrorMessage = "El stock es requerido")]
        [Range(0, int.MaxValue, ErrorMessage = "El stock no puede ser negativo")]
        public int Stock { get; set; }

        public int? CategoriaId { get; set; }

        [StringLength(50, ErrorMessage = "El código de barras no puede exceder 50 caracteres")]
        public string? CodigoBarras { get; set; }

        [StringLength(500, ErrorMessage = "La URL de imagen no puede exceder 500 caracteres")]
        public string? ImagenUrl { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "El precio de compra no puede ser negativo")]
        public decimal? PrecioCompra { get; set; }

        [Range(0, 100, ErrorMessage = "El margen de ganancia debe estar entre 0 y 100")]
        public decimal? MargenGanancia { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "El stock mínimo no puede ser negativo")]
        public int? StockMinimo { get; set; }

        public bool Activo { get; set; } = true;
    }
}
