using System.ComponentModel.DataAnnotations;
using jam_POS.Core.Interfaces;

namespace jam_POS.Core.Entities
{
    public class Producto : ITenantEntity
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Descripcion { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor a 0")]
        public decimal Precio { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "El stock no puede ser negativo")]
        public int Stock { get; set; }

        // Foreign Keys
        public int? CategoriaId { get; set; }
        public int? EmpresaId { get; set; }

        [StringLength(50)]
        public string? CodigoBarras { get; set; }
        
        // Navigation properties
        public virtual Categoria? Categoria { get; set; }
        public virtual Empresa? Empresa { get; set; }

        [StringLength(500)]
        public string? ImagenUrl { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "El precio de compra no puede ser negativo")]
        public decimal? PrecioCompra { get; set; }

        [Range(0, 100, ErrorMessage = "El margen de ganancia debe estar entre 0 y 100")]
        public decimal? MargenGanancia { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "El stock mínimo no puede ser negativo")]
        public int? StockMinimo { get; set; }

        public bool Activo { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
