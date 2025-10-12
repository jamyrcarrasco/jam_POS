using System.ComponentModel.DataAnnotations;
using jam_POS.Core.Interfaces;

namespace jam_POS.Core.Entities
{
    public class Pago : ITenantEntity
    {
        public int Id { get; set; }

        [Required]
        public int VentaId { get; set; }

        [Required]
        [StringLength(50)]
        public string MetodoPago { get; set; } = string.Empty; // EFECTIVO, TARJETA, TRANSFERENCIA, CREDITO

        [Required]
        public decimal Monto { get; set; }

        [StringLength(100)]
        public string? Referencia { get; set; } // Número de tarjeta (parcial), transferencia, etc.

        [StringLength(100)]
        public string? Banco { get; set; } // Para transferencias

        [StringLength(50)]
        public string? TipoTarjeta { get; set; } // VISA, MASTERCARD, AMEX

        public DateTime FechaPago { get; set; } = DateTime.UtcNow;

        [StringLength(500)]
        public string? Notas { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Foreign Keys (Multi-tenant)
        public int? EmpresaId { get; set; }

        // Navigation properties
        public virtual Empresa? Empresa { get; set; }
        public virtual Venta? Venta { get; set; }
    }
}

