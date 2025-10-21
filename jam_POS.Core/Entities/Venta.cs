using System.ComponentModel.DataAnnotations;
using jam_POS.Core.Interfaces;

namespace jam_POS.Core.Entities
{
    public class Venta : ITenantEntity
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string NumeroVenta { get; set; } = string.Empty;

        public DateTime FechaVenta { get; set; } = DateTime.UtcNow;

        [StringLength(500)]
        public string? Notas { get; set; }

        // Totales
        [Required]
        public decimal Subtotal { get; set; }

        [Required]
        public decimal TotalImpuestos { get; set; }

        [Required]
        public decimal TotalDescuentos { get; set; }

        [Required]
        public decimal Total { get; set; }

        // Estado de la venta
        [Required]
        [StringLength(20)]
        public string Estado { get; set; } = "COMPLETADA"; // COMPLETADA, CANCELADA, PENDIENTE

        public DateTime? FechaCancelacion { get; set; }

        [StringLength(500)]
        public string? MotivoCancelacion { get; set; }

        // Cliente (opcional)
        public int? ClienteId { get; set; }

        // Usuario que realizó la venta
        [Required]
        public int UsuarioId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Foreign Keys (Multi-tenant)
        public int? EmpresaId { get; set; }

        // Navigation properties
        public virtual Empresa? Empresa { get; set; }
        public virtual User? Usuario { get; set; }
        public virtual Cliente? Cliente { get; set; }
        public virtual ICollection<VentaItem> VentaItems { get; set; } = new List<VentaItem>();
        public virtual ICollection<Pago> Pagos { get; set; } = new List<Pago>();
    }
}

