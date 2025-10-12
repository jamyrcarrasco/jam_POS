namespace jam_POS.Application.DTOs.Responses
{
    public class VentaResponse
    {
        public int Id { get; set; }
        public string NumeroVenta { get; set; } = string.Empty;
        public DateTime FechaVenta { get; set; }
        public string? Notas { get; set; }
        public decimal Subtotal { get; set; }
        public decimal TotalImpuestos { get; set; }
        public decimal TotalDescuentos { get; set; }
        public decimal Total { get; set; }
        public string Estado { get; set; } = string.Empty;
        public DateTime? FechaCancelacion { get; set; }
        public string? MotivoCancelacion { get; set; }
        public int? ClienteId { get; set; }
        public string? ClienteNombre { get; set; }
        public int UsuarioId { get; set; }
        public string UsuarioNombre { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<VentaItemResponse> Items { get; set; } = new();
        public List<PagoResponse> Pagos { get; set; } = new();
    }

    public class VentaItemResponse
    {
        public int Id { get; set; }
        public int VentaId { get; set; }
        public int ProductoId { get; set; }
        public string ProductoNombre { get; set; } = string.Empty;
        public string? ProductoCodigo { get; set; }
        public decimal Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal { get; set; }
        public decimal DescuentoPorcentaje { get; set; }
        public decimal DescuentoMonto { get; set; }
        public decimal TotalImpuestos { get; set; }
        public decimal Total { get; set; }
        public string? Notas { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class PagoResponse
    {
        public int Id { get; set; }
        public int VentaId { get; set; }
        public string MetodoPago { get; set; } = string.Empty;
        public decimal Monto { get; set; }
        public string? Referencia { get; set; }
        public string? Banco { get; set; }
        public string? TipoTarjeta { get; set; }
        public DateTime FechaPago { get; set; }
        public string? Notas { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class VentaSummaryResponse
    {
        public int Id { get; set; }
        public string NumeroVenta { get; set; } = string.Empty;
        public DateTime FechaVenta { get; set; }
        public decimal Total { get; set; }
        public string Estado { get; set; } = string.Empty;
        public string UsuarioNombre { get; set; } = string.Empty;
        public int CantidadItems { get; set; }
    }
}

