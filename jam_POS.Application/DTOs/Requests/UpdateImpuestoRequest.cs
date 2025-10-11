using System.ComponentModel.DataAnnotations;

namespace jam_POS.Application.DTOs.Requests
{
    public class UpdateImpuestoRequest
    {
        [Required(ErrorMessage = "El ID es requerido")]
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre del impuesto es requerido")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "La descripción no puede exceder 500 caracteres")]
        public string? Descripcion { get; set; }

        [Required(ErrorMessage = "El tipo de impuesto es requerido")]
        [StringLength(50, ErrorMessage = "El tipo no puede exceder 50 caracteres")]
        public string Tipo { get; set; } = "PORCENTUAL";

        [Range(0, 100, ErrorMessage = "El porcentaje debe estar entre 0 y 100")]
        public decimal Porcentaje { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "El monto fijo no puede ser negativo")]
        public decimal? MontoFijo { get; set; }

        public bool AplicaPorDefecto { get; set; }

        public bool Activo { get; set; }
    }
}

