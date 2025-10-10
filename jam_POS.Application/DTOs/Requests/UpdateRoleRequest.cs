using System.ComponentModel.DataAnnotations;

namespace jam_POS.Application.DTOs.Requests
{
    public class UpdateRoleRequest
    {
        [Required(ErrorMessage = "El ID es requerido")]
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre del rol es requerido")]
        [StringLength(50, ErrorMessage = "El nombre del rol no puede exceder 50 caracteres")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "La descripción es requerida")]
        [StringLength(200, ErrorMessage = "La descripción no puede exceder 200 caracteres")]
        public string Description { get; set; } = string.Empty;

        public bool Activo { get; set; }

        public List<int> PermissionIds { get; set; } = new List<int>();
    }
}

