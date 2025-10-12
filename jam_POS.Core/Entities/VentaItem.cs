using System.ComponentModel.DataAnnotations;
using jam_POS.Core.Interfaces;

namespace jam_POS.Core.Entities
{
    public class VentaItem : ITenantEntity
    {
        public int Id { get; set; }

        [Required]
        public int VentaId { get; set; }

        [Required]
        public int ProductoId { get; set; }

        [Required]
        [StringLength(200)]
        public string ProductoNombre { get; set; } = string.Empty;

        [StringLength(50)]
        public string? ProductoCodigo { get; set; }

        [Required]
        public decimal Cantidad { get; set; }

        [Required]
        public decimal PrecioUnitario { get; set; }

        [Required]
        public decimal Subtotal { get; set; }

        // Descuento aplicado a este item
        public decimal DescuentoPorcentaje { get; set; } = 0;

        public decimal DescuentoMonto { get; set; } = 0;

        // Impuestos aplicados a este item
        public decimal TotalImpuestos { get; set; } = 0;

        [Required]
        public decimal Total { get; set; }

        // Notas específicas del item
        [StringLength(500)]
        public string? Notas { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Foreign Keys (Multi-tenant)
        public int? EmpresaId { get; set; }

        // Navigation properties
        public virtual Empresa? Empresa { get; set; }
        public virtual Venta? Venta { get; set; }
        public virtual Producto? Producto { get; set; }
    }
}

