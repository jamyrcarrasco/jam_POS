using System.ComponentModel.DataAnnotations;
using jam_POS.Core.Interfaces;

namespace jam_POS.Core.Entities
{
    public class Impuesto : ITenantEntity
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Descripcion { get; set; }

        [Required]
        [Range(0, 100, ErrorMessage = "El porcentaje debe estar entre 0 y 100")]
        public decimal Porcentaje { get; set; }

        [Required]
        [StringLength(50)]
        public string Tipo { get; set; } = "PORCENTUAL"; // PORCENTUAL, FIJO

        // Para impuestos fijos (ej: $1.00 por unidad)
        [Range(0, double.MaxValue, ErrorMessage = "El monto fijo no puede ser negativo")]
        public decimal? MontoFijo { get; set; }

        public bool AplicaPorDefecto { get; set; } = false;

        public bool Activo { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Foreign Key (Multi-tenant)
        public int? EmpresaId { get; set; }

        // Navigation property
        public virtual Empresa? Empresa { get; set; }
    }
}

