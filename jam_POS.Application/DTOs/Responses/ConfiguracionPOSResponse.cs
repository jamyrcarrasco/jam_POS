namespace jam_POS.Application.DTOs.Responses
{
    public class ConfiguracionPOSResponse
    {
        public int Id { get; set; }

        // Configuración de Recibos/Facturas
        public string PrefijoRecibo { get; set; } = string.Empty;
        public string PrefijoFactura { get; set; } = string.Empty;
        public int SiguienteNumeroRecibo { get; set; }
        public int SiguienteNumeroFactura { get; set; }
        public string? MensajePieRecibo { get; set; }
        public bool IncluirLogoRecibo { get; set; }

        // Comportamiento de Ventas
        public int? ImpuestoPorDefectoId { get; set; }
        public string? ImpuestoPorDefectoNombre { get; set; }
        public bool PermitirDescuentos { get; set; }
        public bool PermitirDevoluciones { get; set; }
        public int TiempoLimiteAnulacionMinutos { get; set; }
        public decimal DescuentoMaximoPorcentaje { get; set; }

        // Configuración de Moneda
        public string MonedaPorDefecto { get; set; } = string.Empty;
        public string SimboloMoneda { get; set; } = string.Empty;
        public int DecimalesMoneda { get; set; }

        // Configuración de Impresión
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
        public int IntervaloSincronizacionMinutos { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}

