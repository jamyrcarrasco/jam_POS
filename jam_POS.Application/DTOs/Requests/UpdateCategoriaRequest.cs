using System.ComponentModel.DataAnnotations;

namespace jam_POS.Application.DTOs.Requests
{
    public class UpdateCategoriaRequest
    {
        [Required(ErrorMessage = "El ID es requerido")]
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre de la categoría es requerido")]
        [StringLength(100, ErrorMessage = "El nombre de la categoría no puede exceder 100 caracteres")]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "La descripción no puede exceder 500 caracteres")]
        public string? Descripcion { get; set; }

        [StringLength(50, ErrorMessage = "El color no puede exceder 50 caracteres")]
        public string? Color { get; set; }

        [StringLength(50, ErrorMessage = "El icono no puede exceder 50 caracteres")]
        public string? Icono { get; set; }

        public bool Activo { get; set; }
    }
}

