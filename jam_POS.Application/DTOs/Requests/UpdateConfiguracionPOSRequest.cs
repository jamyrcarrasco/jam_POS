using System.ComponentModel.DataAnnotations;

namespace jam_POS.Application.DTOs.Requests
{
    public class UpdateConfiguracionPOSRequest
    {
        [Required(ErrorMessage = "El ID es requerido")]
        public int Id { get; set; }

        // Configuración de Recibos/Facturas
        [Required(ErrorMessage = "El prefijo de recibo es requerido")]
        [StringLength(10, ErrorMessage = "El prefijo no puede exceder 10 caracteres")]
        public string PrefijoRecibo { get; set; } = string.Empty;

        [Required(ErrorMessage = "El prefijo de factura es requerido")]
        [StringLength(10, ErrorMessage = "El prefijo no puede exceder 10 caracteres")]
        public string PrefijoFactura { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "El siguiente número debe ser mayor a 0")]
        public int SiguienteNumeroRecibo { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "El siguiente número debe ser mayor a 0")]
        public int SiguienteNumeroFactura { get; set; }

        [StringLength(500, ErrorMessage = "El mensaje no puede exceder 500 caracteres")]
        public string? MensajePieRecibo { get; set; }

        public bool IncluirLogoRecibo { get; set; }

        // Comportamiento de Ventas
        public int? ImpuestoPorDefectoId { get; set; }

        public bool PermitirDescuentos { get; set; }

        public bool PermitirDevoluciones { get; set; }

        [Range(0, 1440, ErrorMessage = "El tiempo límite debe estar entre 0 y 1440 minutos")]
        public int TiempoLimiteAnulacionMinutos { get; set; }

        [Range(0, 100, ErrorMessage = "El descuento máximo debe estar entre 0 y 100%")]
        public decimal DescuentoMaximoPorcentaje { get; set; }

        // Configuración de Moneda
        [Required(ErrorMessage = "La moneda por defecto es requerida")]
        [StringLength(3, ErrorMessage = "El código de moneda debe tener 3 caracteres")]
        public string MonedaPorDefecto { get; set; } = string.Empty;

        [Required(ErrorMessage = "El símbolo de moneda es requerido")]
        [StringLength(10, ErrorMessage = "El símbolo no puede exceder 10 caracteres")]
        public string SimboloMoneda { get; set; } = string.Empty;

        [Range(0, 4, ErrorMessage = "Los decimales deben estar entre 0 y 4")]
        public int DecimalesMoneda { get; set; }

        // Configuración de Impresión
        [StringLength(50, ErrorMessage = "El formato de papel no puede exceder 50 caracteres")]
        public string FormatoPapel { get; set; } = string.Empty;

        public bool ImprimirAutomaticamente { get; set; }

        public bool ImprimirCopiaCliente { get; set; }

        // Configuración de Métodos de Pago
        public bool EfectivoHabilitado { get; set; }

        public bool TarjetaHabilitado { get; set; }

        public bool TransferenciaHabilitado { get; set; }

        public bool CreditoHabilitado { get; set; }

        // Configuración de Modo Operación
        public bool ModoOfflineHabilitado { get; set; }

        [Range(1, 1440, ErrorMessage = "El intervalo debe estar entre 1 y 1440 minutos")]
        public int IntervaloSincronizacionMinutos { get; set; }
    }
}

