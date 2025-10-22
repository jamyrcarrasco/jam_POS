namespace jam_POS.Application.DTOs.Responses
{
    public class SalesReportResponse
    {
        public decimal TotalVentas { get; set; }
        public decimal TotalImpuestos { get; set; }
        public decimal TotalDescuentos { get; set; }
        public int CantidadVentas { get; set; }
        public decimal TotalItemsVendidos { get; set; }
        public decimal PromedioVenta { get; set; }
        public int? Mes { get; set; }
        public int? Anio { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public string PeriodoDescripcion { get; set; } = string.Empty;
        public List<SalesReportVentaDto> Ventas { get; set; } = new();
        public List<SalesReportProductoDto> Productos { get; set; } = new();
    }

    public class SalesReportVentaDto
    {
        public int Id { get; set; }
        public string NumeroVenta { get; set; } = string.Empty;
        public DateTime FechaVenta { get; set; }
        public decimal Subtotal { get; set; }
        public decimal TotalImpuestos { get; set; }
        public decimal TotalDescuentos { get; set; }
        public decimal Total { get; set; }
        public string Estado { get; set; } = string.Empty;
        public string UsuarioNombre { get; set; } = string.Empty;
        public decimal CantidadItems { get; set; }
    }

    public class SalesReportProductoDto
    {
        public int ProductoId { get; set; }
        public string ProductoNombre { get; set; } = string.Empty;
        public string? CategoriaNombre { get; set; }
        public decimal CantidadVendida { get; set; }
        public decimal TotalVendido { get; set; }
    }
}
