using System.ComponentModel.DataAnnotations;
using jam_POS.Core.Interfaces;

namespace jam_POS.Core.Entities
{
    public class Categoria : ITenantEntity
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Descripcion { get; set; }

        [StringLength(50)]
        public string? Color { get; set; }

        [StringLength(50)]
        public string? Icono { get; set; }

        public bool Activo { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Foreign Key
        public int? EmpresaId { get; set; }

        // Navigation properties
        public virtual ICollection<Producto> Productos { get; set; } = new List<Producto>();
        public virtual Empresa? Empresa { get; set; }
    }
}

