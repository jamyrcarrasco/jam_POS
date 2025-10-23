namespace jam_POS.API.DTOs
{
    public class SalesReportApiResponse
    {
        public decimal TotalVentas { get; set; }
        public decimal TotalImpuestos { get; set; }
        public decimal TotalDescuentos { get; set; }
        public int CantidadVentas { get; set; }
        public int TotalItemsVendidos { get; set; }
        public decimal PromedioVenta { get; set; }
        public int? Mes { get; set; }
        public int? Anio { get; set; }
        public string? FechaInicio { get; set; }
        public string? FechaFin { get; set; }
        public string PeriodoDescripcion { get; set; } = string.Empty;
        public List<SalesReportVentaApi> Ventas { get; set; } = new List<SalesReportVentaApi>();
        public List<SalesReportProductoApi> Productos { get; set; } = new List<SalesReportProductoApi>();
    }

    public class SalesReportVentaApi
    {
        public int Id { get; set; }
        public string NumeroVenta { get; set; } = string.Empty;
        public string FechaVenta { get; set; } = string.Empty;
        public decimal Subtotal { get; set; }
        public decimal TotalImpuestos { get; set; }
        public decimal TotalDescuentos { get; set; }
        public decimal Total { get; set; }
        public string Estado { get; set; } = string.Empty;
        public string UsuarioNombre { get; set; } = string.Empty;
        public int CantidadItems { get; set; }
    }

    public class SalesReportProductoApi
    {
        public int ProductoId { get; set; }
        public string ProductoNombre { get; set; } = string.Empty;
        public string? CategoriaNombre { get; set; }
        public string? CategoriaColor { get; set; }
        public int CantidadVendida { get; set; }
        public decimal TotalVendido { get; set; }
    }
}
