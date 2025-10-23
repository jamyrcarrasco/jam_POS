using jam_POS.API.DTOs;
using jam_POS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace jam_POS.API.Services
{
    public class ReporteService : IReporteService
    {
        private readonly ILogger<ReporteService> _logger;
        private readonly ApplicationDbContext _context;

        public ReporteService(ILogger<ReporteService> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<SalesReportApiResponse> GetSalesReportAsync(SalesReportFilterRequest filter)
        {
            try
            {
                // Build date range based on filter type
                var (startDate, endDate) = GetDateRange(filter);
                
                // Ensure all DateTime values are properly handled for PostgreSQL
                startDate = EnsureUtcDateTime(startDate);
                endDate = EnsureUtcDateTime(endDate);
                
                // Base query for sales
                var ventasQuery = _context.Ventas
                    .Include(v => v.Usuario)
                    .Include(v => v.Cliente)
                    .Include(v => v.VentaItems)
                        .ThenInclude(vi => vi.Producto)
                            .ThenInclude(p => p!.Categoria)
                    .Where(v => v.Estado == "COMPLETADA");

                // Apply date filters
                if (startDate.HasValue)
                {
                    ventasQuery = ventasQuery.Where(v => v.FechaVenta >= startDate.Value);
                }
                if (endDate.HasValue)
                {
                    ventasQuery = ventasQuery.Where(v => v.FechaVenta <= endDate.Value);
                }

                // Apply product filter
                if (filter.ProductId.HasValue)
                {
                    ventasQuery = ventasQuery.Where(v => v.VentaItems.Any(vi => vi.ProductoId == filter.ProductId.Value));
                }

                // Apply category filter
                if (filter.CategoryId.HasValue)
                {
                    ventasQuery = ventasQuery.Where(v => v.VentaItems.Any(vi => vi.Producto!.CategoriaId == filter.CategoryId.Value));
                }

                // Get sales data
                var ventas = await ventasQuery.ToListAsync();

                // Calculate summary statistics
                var totalVentas = ventas.Sum(v => v.Total);
                var totalImpuestos = ventas.Sum(v => v.TotalImpuestos);
                var totalDescuentos = ventas.Sum(v => v.TotalDescuentos);
                var cantidadVentas = ventas.Count;
                var totalItemsVendidos = (int)ventas.Sum(v => v.VentaItems.Sum(vi => vi.Cantidad));
                var promedioVenta = cantidadVentas > 0 ? totalVentas / cantidadVentas : 0;

                // Build sales details
                var ventasDetalle = ventas.Select(v => new SalesReportVentaApi
                {
                    Id = v.Id,
                    NumeroVenta = v.NumeroVenta,
                    FechaVenta = v.FechaVenta.ToString("yyyy-MM-dd HH:mm:ss"),
                    Subtotal = v.Subtotal,
                    TotalImpuestos = v.TotalImpuestos,
                    TotalDescuentos = v.TotalDescuentos,
                    Total = v.Total,
                    Estado = v.Estado,
                    UsuarioNombre = $"{v.Usuario?.FirstName} {v.Usuario?.LastName}".Trim(),
                    CantidadItems = (int)v.VentaItems.Sum(vi => vi.Cantidad)
                }).ToList();

                // Build product sales summary
                var productosVendidosQuery = ventas
                    .SelectMany(v => v.VentaItems)
                    .AsQueryable();

                // Apply category filter to product summary if specified
                if (filter.CategoryId.HasValue)
                {
                    productosVendidosQuery = productosVendidosQuery.Where(vi => vi.Producto!.CategoriaId == filter.CategoryId.Value);
                }

                // Apply product filter to product summary if specified
                if (filter.ProductId.HasValue)
                {
                    productosVendidosQuery = productosVendidosQuery.Where(vi => vi.ProductoId == filter.ProductId.Value);
                }

                var productosVendidos = productosVendidosQuery
                    .GroupBy(vi => new { vi.ProductoId, vi.ProductoNombre })
                    .Select(g => new SalesReportProductoApi
                    {
                        ProductoId = g.Key.ProductoId,
                        ProductoNombre = g.Key.ProductoNombre,
                        CategoriaNombre = g.First().Producto?.Categoria?.Nombre,
                        CantidadVendida = (int)g.Sum(vi => vi.Cantidad),
                        TotalVendido = g.Sum(vi => vi.Total)
                    })
                    .OrderByDescending(p => p.TotalVendido)
                    .ToList();

                // Build period description
                var periodoDescripcion = BuildPeriodDescription(filter, startDate, endDate);

                return new SalesReportApiResponse
                {
                    TotalVentas = totalVentas,
                    TotalImpuestos = totalImpuestos,
                    TotalDescuentos = totalDescuentos,
                    CantidadVentas = cantidadVentas,
                    TotalItemsVendidos = (int)totalItemsVendidos,
                    PromedioVenta = promedioVenta,
                    Mes = filter.Month,
                    Anio = filter.Year,
                    FechaInicio = startDate?.ToString("yyyy-MM-dd"),
                    FechaFin = endDate?.ToString("yyyy-MM-dd"),
                    PeriodoDescripcion = periodoDescripcion,
                    Ventas = ventasDetalle,
                    Productos = productosVendidos
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating sales report");
                throw;
            }
        }

        private (DateTime?, DateTime?) GetDateRange(SalesReportFilterRequest filter)
        {
            DateTime? startDate = null;
            DateTime? endDate = null;

            if (filter.StartDate.HasValue && filter.EndDate.HasValue)
            {
                // Date range filter - ensure UTC
                startDate = DateTime.SpecifyKind(filter.StartDate.Value.Date, DateTimeKind.Utc);
                endDate = DateTime.SpecifyKind(filter.EndDate.Value.Date.AddDays(1).AddTicks(-1), DateTimeKind.Utc); // End of day
            }
            else if (filter.Month.HasValue && filter.Year.HasValue)
            {
                // Month filter - ensure UTC
                startDate = DateTime.SpecifyKind(new DateTime(filter.Year.Value, filter.Month.Value, 1), DateTimeKind.Utc);
                endDate = DateTime.SpecifyKind(startDate.Value.AddMonths(1).AddDays(-1).AddHours(23).AddMinutes(59).AddSeconds(59), DateTimeKind.Utc);
            }
            else if (filter.Year.HasValue)
            {
                // Year filter - ensure UTC
                startDate = DateTime.SpecifyKind(new DateTime(filter.Year.Value, 1, 1), DateTimeKind.Utc);
                endDate = DateTime.SpecifyKind(new DateTime(filter.Year.Value, 12, 31, 23, 59, 59), DateTimeKind.Utc);
            }

            return (startDate, endDate);
        }

        private string BuildPeriodDescription(SalesReportFilterRequest filter, DateTime? startDate, DateTime? endDate)
        {
            if (filter.StartDate.HasValue && filter.EndDate.HasValue)
            {
                return $"Reporte del {filter.StartDate.Value:dd/MM/yyyy} al {filter.EndDate.Value:dd/MM/yyyy}";
            }
            else if (filter.Month.HasValue && filter.Year.HasValue)
            {
                var monthNames = new[] { "", "Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio",
                    "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre" };
                return $"Reporte de {monthNames[filter.Month.Value]} {filter.Year.Value}";
            }
            else if (filter.Year.HasValue)
            {
                return $"Reporte del año {filter.Year.Value}";
            }
            
            return "Reporte de ventas";
        }

        private DateTime? EnsureUtcDateTime(DateTime? dateTime)
        {
            if (!dateTime.HasValue)
                return null;

            // If the DateTime is already UTC, return as is
            if (dateTime.Value.Kind == DateTimeKind.Utc)
                return dateTime;

            // If it's Local or Unspecified, convert to UTC
            if (dateTime.Value.Kind == DateTimeKind.Local)
                return dateTime.Value.ToUniversalTime();

            // If it's Unspecified, assume it's UTC and specify the kind
            return DateTime.SpecifyKind(dateTime.Value, DateTimeKind.Utc);
        }
    }
}
