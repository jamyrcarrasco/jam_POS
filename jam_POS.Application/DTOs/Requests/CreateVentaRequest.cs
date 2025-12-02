using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace jam_POS.Application.DTOs.Requests
{
    public class CreateVentaRequest : IValidatableObject
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

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            foreach (var pago in Pagos)
            {
                if (string.Equals(pago.MetodoPago, "CREDITO", StringComparison.OrdinalIgnoreCase)
                    && !ClienteId.HasValue
                    && string.IsNullOrWhiteSpace(pago.Referencia))
                {
                    yield return new ValidationResult(
                        "Para pagos a crédito se requiere especificar el cliente o una referencia de crédito.",
                        new[] { nameof(Pagos), nameof(ClienteId) });
                }
            }
        }
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

        [Range(0, double.MaxValue, ErrorMessage = "Los impuestos no pueden ser negativos")]
        public decimal TotalImpuestos { get; set; } = 0; // Impuestos ya calculados por el frontend

        [StringLength(500, ErrorMessage = "Las notas no pueden exceder 500 caracteres")]
        public string? Notas { get; set; }
    }

    public class CreatePagoRequest : IValidatableObject
    {
        [Required(ErrorMessage = "El método de pago es requerido")]
        [StringLength(50, ErrorMessage = "El método de pago no puede exceder 50 caracteres")]
        public string MetodoPago { get; set; } = string.Empty;

        [Required(ErrorMessage = "El monto es requerido")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El monto debe ser mayor a 0")]
        public decimal Monto { get; set; }

        // Campos para pago en efectivo con cambio
        [Range(0, double.MaxValue, ErrorMessage = "El monto recibido no puede ser negativo")]
        public decimal? MontoRecibido { get; set; } // Monto que entregó el cliente en efectivo

        [Range(0, double.MaxValue, ErrorMessage = "El cambio no puede ser negativo")]
        public decimal? CambioDevolver { get; set; } // Cambio que se devolvió al cliente

        [StringLength(100, ErrorMessage = "La referencia no puede exceder 100 caracteres")]
        public string? Referencia { get; set; }

        [StringLength(100, ErrorMessage = "El banco no puede exceder 100 caracteres")]
        public string? Banco { get; set; }

        [StringLength(50, ErrorMessage = "El tipo de tarjeta no puede exceder 50 caracteres")]
        public string? TipoTarjeta { get; set; }

        [StringLength(500, ErrorMessage = "Las notas no pueden exceder 500 caracteres")]
        public string? Notas { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var metodo = MetodoPago?.ToUpperInvariant() ?? string.Empty;

            if (metodo == "TARJETA")
            {
                if (string.IsNullOrWhiteSpace(TipoTarjeta))
                {
                    yield return new ValidationResult(
                        "El tipo de tarjeta es requerido para pagos con TARJETA.",
                        new[] { nameof(TipoTarjeta) });
                }

                if (string.IsNullOrWhiteSpace(Referencia))
                {
                    yield return new ValidationResult(
                        "La referencia es requerida para pagos con TARJETA.",
                        new[] { nameof(Referencia) });
                }
            }

            if (metodo == "TRANSFERENCIA")
            {
                if (string.IsNullOrWhiteSpace(Banco))
                {
                    yield return new ValidationResult(
                        "El banco es requerido para pagos con TRANSFERENCIA.",
                        new[] { nameof(Banco) });
                }

                if (string.IsNullOrWhiteSpace(Referencia))
                {
                    yield return new ValidationResult(
                        "La referencia es requerida para pagos con TRANSFERENCIA.",
                        new[] { nameof(Referencia) });
                }
            }
        }
    }
}

