using System.ComponentModel.DataAnnotations;

namespace jam_POS.Application.DTOs.Requests
{
    public class RegisterEmpresaRequest
    {
        // Datos de la Empresa
        [Required(ErrorMessage = "El nombre de la empresa es requerido")]
        [StringLength(200, ErrorMessage = "El nombre no puede exceder 200 caracteres")]
        public string NombreEmpresa { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre comercial es requerido")]
        [StringLength(100, ErrorMessage = "El nombre comercial no puede exceder 100 caracteres")]
        public string NombreComercial { get; set; } = string.Empty;

        [Required(ErrorMessage = "El RUC/NIT es requerido")]
        [StringLength(50, ErrorMessage = "El RUC/NIT no puede exceder 50 caracteres")]
        public string RUC { get; set; } = string.Empty;

        [StringLength(100, ErrorMessage = "El email no puede exceder 100 caracteres")]
        [EmailAddress(ErrorMessage = "Email no válido")]
        public string? EmailEmpresa { get; set; }

        [StringLength(50)]
        public string? Telefono { get; set; }

        // Datos del Usuario Administrador
        [Required(ErrorMessage = "El nombre del administrador es requerido")]
        [StringLength(100)]
        public string AdminFirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "El apellido del administrador es requerido")]
        [StringLength(100)]
        public string AdminLastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "El email del administrador es requerido")]
        [EmailAddress(ErrorMessage = "Email no válido")]
        [StringLength(100)]
        public string AdminEmail { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre de usuario es requerido")]
        [StringLength(50)]
        public string AdminUsername { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es requerida")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "La contraseña debe tener entre 6 y 100 caracteres")]
        public string AdminPassword { get; set; } = string.Empty;
    }
}

