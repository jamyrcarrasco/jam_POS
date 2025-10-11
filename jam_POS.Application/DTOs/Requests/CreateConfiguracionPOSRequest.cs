using System.ComponentModel.DataAnnotations;

namespace jam_POS.Application.DTOs.Requests
{
    public class CreateConfiguracionPOSRequest
    {
        // Configuración de Recibos/Facturas
        [Required(ErrorMessage = "El prefijo de recibo es requerido")]
        [StringLength(10, ErrorMessage = "El prefijo no puede exceder 10 caracteres")]
        public string PrefijoRecibo { get; set; } = "REC";

        [Required(ErrorMessage = "El prefijo de factura es requerido")]
        [StringLength(10, ErrorMessage = "El prefijo no puede exceder 10 caracteres")]
        public string PrefijoFactura { get; set; } = "FAC";

        [Range(1, int.MaxValue, ErrorMessage = "El siguiente número debe ser mayor a 0")]
        public int SiguienteNumeroRecibo { get; set; } = 1;

        [Range(1, int.MaxValue, ErrorMessage = "El siguiente número debe ser mayor a 0")]
        public int SiguienteNumeroFactura { get; set; } = 1;

        [StringLength(500, ErrorMessage = "El mensaje no puede exceder 500 caracteres")]
        public string? MensajePieRecibo { get; set; }

        public bool IncluirLogoRecibo { get; set; } = true;

        // Comportamiento de Ventas
        public int? ImpuestoPorDefectoId { get; set; }

        public bool PermitirDescuentos { get; set; } = true;

        public bool PermitirDevoluciones { get; set; } = true;

        [Range(0, 1440, ErrorMessage = "El tiempo límite debe estar entre 0 y 1440 minutos")]
        public int TiempoLimiteAnulacionMinutos { get; set; } = 30;

        [Range(0, 100, ErrorMessage = "El descuento máximo debe estar entre 0 y 100%")]
        public decimal DescuentoMaximoPorcentaje { get; set; } = 100;

        // Configuración de Moneda
        [Required(ErrorMessage = "La moneda por defecto es requerida")]
        [StringLength(3, ErrorMessage = "El código de moneda debe tener 3 caracteres")]
        public string MonedaPorDefecto { get; set; } = "DOP";

        [Required(ErrorMessage = "El símbolo de moneda es requerido")]
        [StringLength(10, ErrorMessage = "El símbolo no puede exceder 10 caracteres")]
        public string SimboloMoneda { get; set; } = "$";

        [Range(0, 4, ErrorMessage = "Los decimales deben estar entre 0 y 4")]
        public int DecimalesMoneda { get; set; } = 2;

        // Configuración de Impresión
        [StringLength(50, ErrorMessage = "El formato de papel no puede exceder 50 caracteres")]
        public string FormatoPapel { get; set; } = "TERMICO_58";

        public bool ImprimirAutomaticamente { get; set; } = false;

        public bool ImprimirCopiaCliente { get; set; } = true;

        // Configuración de Métodos de Pago
        public bool EfectivoHabilitado { get; set; } = true;

        public bool TarjetaHabilitado { get; set; } = true;

        public bool TransferenciaHabilitado { get; set; } = false;

        public bool CreditoHabilitado { get; set; } = false;

        // Configuración de Modo Operación
        public bool ModoOfflineHabilitado { get; set; } = false;

        [Range(1, 1440, ErrorMessage = "El intervalo debe estar entre 1 y 1440 minutos")]
        public int IntervaloSincronizacionMinutos { get; set; } = 15;
    }
}

