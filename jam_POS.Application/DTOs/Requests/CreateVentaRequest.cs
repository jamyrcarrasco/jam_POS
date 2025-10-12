using System.ComponentModel.DataAnnotations;

namespace jam_POS.Application.DTOs.Requests
{
    public class CreateVentaRequest
    {
        [StringLength(500, ErrorMessage = "Las notas no pueden exceder 500 caracteres")]
        public string? Notas { get; set; }

        public int? ClienteId { get; set; }

        [Required(ErrorMessage = "Los items de la venta son requeridos")]
        [MinLength(1, ErrorMessage = "Debe incluir al menos un producto")]
        public List<CreateVentaItemRequest> Items { get; set; } = new();

        [Required(ErrorMessage = "Los pagos son requeridos")]
        [MinLength(1, ErrorMessage = "Debe incluir al menos un método de pago")]
        public List<CreatePagoRequest> Pagos { get; set; } = new();
    }

    public class CreateVentaItemRequest
    {
        [Required(ErrorMessage = "El ID del producto es requerido")]
        public int ProductoId { get; set; }

        [Required(ErrorMessage = "La cantidad es requerida")]
        [Range(0.01, double.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0")]
        public decimal Cantidad { get; set; }

        [Required(ErrorMessage = "El precio unitario es requerido")]
        [Range(0, double.MaxValue, ErrorMessage = "El precio no puede ser negativo")]
        public decimal PrecioUnitario { get; set; }

        [Range(0, 100, ErrorMessage = "El descuento debe estar entre 0 y 100%")]
        public decimal DescuentoPorcentaje { get; set; } = 0;

        [Range(0, double.MaxValue, ErrorMessage = "El descuento no puede ser negativo")]
        public decimal DescuentoMonto { get; set; } = 0;

        [StringLength(500, ErrorMessage = "Las notas no pueden exceder 500 caracteres")]
        public string? Notas { get; set; }
    }

    public class CreatePagoRequest
    {
        [Required(ErrorMessage = "El método de pago es requerido")]
        [StringLength(50, ErrorMessage = "El método de pago no puede exceder 50 caracteres")]
        public string MetodoPago { get; set; } = string.Empty;

        [Required(ErrorMessage = "El monto es requerido")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El monto debe ser mayor a 0")]
        public decimal Monto { get; set; }

        [StringLength(100, ErrorMessage = "La referencia no puede exceder 100 caracteres")]
        public string? Referencia { get; set; }

        [StringLength(100, ErrorMessage = "El banco no puede exceder 100 caracteres")]
        public string? Banco { get; set; }

        [StringLength(50, ErrorMessage = "El tipo de tarjeta no puede exceder 50 caracteres")]
        public string? TipoTarjeta { get; set; }

        [StringLength(500, ErrorMessage = "Las notas no pueden exceder 500 caracteres")]
        public string? Notas { get; set; }
    }
}

