using System.ComponentModel.DataAnnotations;

namespace jam_POS.Application.DTOs.Requests
{
    public class CreateClienteRequest
    {
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
    }
}
