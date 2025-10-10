using System.ComponentModel.DataAnnotations;

namespace jam_POS.Core.Entities
{
    public class Empresa
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string NombreComercial { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string RUC { get; set; } = string.Empty; // o NIT/RFC según país

        [StringLength(200)]
        public string? Direccion { get; set; }

        [StringLength(50)]
        public string? Telefono { get; set; }

        [StringLength(100)]
        public string? Email { get; set; }

        [StringLength(500)]
        public string? LogoUrl { get; set; }

        [StringLength(100)]
        public string? Pais { get; set; }

        [StringLength(100)]
        public string? Ciudad { get; set; }

        [StringLength(20)]
        public string? CodigoPostal { get; set; }

        // Plan/Subscription info
        [StringLength(50)]
        public string Plan { get; set; } = "BASICO"; // BASICO, PRO, ENTERPRISE

        public DateTime? FechaVencimientoPlan { get; set; }

        public bool Activo { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual ICollection<User> Users { get; set; } = new List<User>();
        public virtual ICollection<Producto> Productos { get; set; } = new List<Producto>();
        public virtual ICollection<Categoria> Categorias { get; set; } = new List<Categoria>();
    }
}

