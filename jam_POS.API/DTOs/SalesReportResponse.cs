namespace jam_POS.API.DTOs
{
    public class SalesReportResponse
    {
        public decimal TotalVentas { get; set; }
        public int CantidadVentas { get; set; }
        public decimal PromedioVenta { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public List<SalesReportItem> Detalles { get; set; } = new List<SalesReportItem>();
    }

    public class SalesReportItem
    {
        public DateTime Fecha { get; set; }
        public decimal Total { get; set; }
        public int Cantidad { get; set; }
        public string? Usuario { get; set; }
    }
}
