using System.ComponentModel.DataAnnotations;
using jam_POS.Core.Interfaces;

namespace jam_POS.Core.Entities
{
    public class Cliente : ITenantEntity
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(100)]
        public string? Apellido { get; set; }

        [StringLength(150)]
        [EmailAddress]
        public string? Email { get; set; }

        [StringLength(50)]
        public string? Telefono { get; set; }

        [StringLength(50)]
        public string? Documento { get; set; }

        [StringLength(300)]
        public string? Direccion { get; set; }

        [StringLength(500)]
        public string? Notas { get; set; }

        public DateTime? FechaNacimiento { get; set; }

        public bool Activo { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public int? EmpresaId { get; set; }

        public virtual Empresa? Empresa { get; set; }
        public virtual ICollection<Venta> Ventas { get; set; } = new List<Venta>();
    }
}
