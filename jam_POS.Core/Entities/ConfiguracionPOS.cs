using System.ComponentModel.DataAnnotations;
using jam_POS.Core.Interfaces;

namespace jam_POS.Core.Entities
{
    public class ConfiguracionPOS : ITenantEntity
    {
        public int Id { get; set; }

        // Configuración de Recibos/Facturas
        [Required]
        [StringLength(10)]
        public string PrefijoRecibo { get; set; } = "REC";

        [Required]
        [StringLength(10)]
        public string PrefijoFactura { get; set; } = "FAC";

        public int SiguienteNumeroRecibo { get; set; } = 1;

        public int SiguienteNumeroFactura { get; set; } = 1;

        [StringLength(500)]
        public string? MensajePieRecibo { get; set; }

        public bool IncluirLogoRecibo { get; set; } = true;

        // Comportamiento de Ventas
        public int? ImpuestoPorDefectoId { get; set; }

        public bool PermitirDescuentos { get; set; } = true;

        public bool PermitirDevoluciones { get; set; } = true;

        public int TiempoLimiteAnulacionMinutos { get; set; } = 30;

        public decimal DescuentoMaximoPorcentaje { get; set; } = 100;

        // Configuración de Moneda
        [Required]
        [StringLength(3)]
        public string MonedaPorDefecto { get; set; } = "DOP";

        [Required]
        [StringLength(10)]
        public string SimboloMoneda { get; set; } = "$";

        public int DecimalesMoneda { get; set; } = 2;

        // Configuración de Impresión
        [StringLength(50)]
        public string FormatoPapel { get; set; } = "TERMICO_58";

        public bool ImprimirAutomaticamente { get; set; } = false;

        public bool ImprimirCopiaCliente { get; set; } = true;

        // Configuración de Métodos de Pago por Defecto
        public bool EfectivoHabilitado { get; set; } = true;

        public bool TarjetaHabilitado { get; set; } = true;

        public bool TransferenciaHabilitado { get; set; } = false;

        public bool CreditoHabilitado { get; set; } = false;

        // Configuración de Modo Operación
        public bool ModoOfflineHabilitado { get; set; } = false;

        public int IntervaloSincronizacionMinutos { get; set; } = 15;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Foreign Keys (Multi-tenant)
        public int? EmpresaId { get; set; }

        // Navigation properties
        public virtual Empresa? Empresa { get; set; }
        public virtual Impuesto? ImpuestoPorDefecto { get; set; }
    }
}

