namespace jam_POS.API.DTOs
{
    public class VentaResponse
    {
        public int Id { get; set; }
        public string NumeroVenta { get; set; } = string.Empty;
        public DateTime Fecha { get; set; }
        public decimal Total { get; set; }
        public decimal Descuento { get; set; }
        public decimal Impuesto { get; set; }
        public string Estado { get; set; } = string.Empty;
        public string? Observaciones { get; set; }
        public int UsuarioId { get; set; }
        public string UsuarioNombre { get; set; } = string.Empty;
        public List<VentaItemResponse> Items { get; set; } = new List<VentaItemResponse>();
    }

    public class VentaItemResponse
    {
        public int Id { get; set; }
        public int ProductoId { get; set; }
        public string ProductoNombre { get; set; } = string.Empty;
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal DescuentoPorcentaje { get; set; }
        public decimal DescuentoMonto { get; set; }
        public decimal TotalImpuestos { get; set; }
        public decimal Total { get; set; }
        public string? Notas { get; set; }
    }

    public class VentaSummaryResponse
    {
        public int Id { get; set; }
        public string NumeroVenta { get; set; } = string.Empty;
        public DateTime Fecha { get; set; }
        public decimal Total { get; set; }
        public string Estado { get; set; } = string.Empty;
        public string UsuarioNombre { get; set; } = string.Empty;
    }

    public class CreateVentaRequest
    {
        public List<CreateVentaItemRequest> Items { get; set; } = new List<CreateVentaItemRequest>();
        public List<CreatePagoRequest> Pagos { get; set; } = new List<CreatePagoRequest>();
        public decimal Descuento { get; set; }
        public decimal Impuesto { get; set; }
        public string? Observaciones { get; set; }
        public int UsuarioId { get; set; }
    }

    public class CreatePagoRequest
    {
        public string MetodoPago { get; set; } = string.Empty;
        public decimal Monto { get; set; }
        public decimal? MontoRecibido { get; set; }
        public decimal? CambioDevolver { get; set; }
        public string? Referencia { get; set; }
        public string? Banco { get; set; }
        public string? TipoTarjeta { get; set; }
        public string? Notas { get; set; }
    }

    public class CreateVentaItemRequest
    {
        public int ProductoId { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal DescuentoPorcentaje { get; set; }
        public decimal DescuentoMonto { get; set; }
        public decimal TotalImpuestos { get; set; }
        public string? Notas { get; set; }
    }
}
