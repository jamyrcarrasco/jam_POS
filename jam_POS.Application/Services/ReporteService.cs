using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using jam_POS.Application.DTOs.Requests;
using jam_POS.Application.DTOs.Responses;
using jam_POS.Core.Entities;
using jam_POS.Infrastructure.Data;

namespace jam_POS.Application.Services
{
    public class ReporteService : IReporteService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ReporteService> _logger;

        public ReporteService(ApplicationDbContext context, ILogger<ReporteService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<SalesReportResponse> GetSalesReportAsync(SalesReportFilterRequest filter)
        {
            filter ??= new SalesReportFilterRequest();

            ValidateFilter(filter);

            var now = DateTime.UtcNow;

            int? month = filter.Month;
            int? year = filter.Year;
            DateTime? startDate = filter.StartDate?.Date;
            DateTime? endDate = filter.EndDate?.Date;

            if (!month.HasValue && !year.HasValue && !startDate.HasValue && !endDate.HasValue)
            {
                month = now.Month;
                year = now.Year;
            }

            if (month.HasValue && !year.HasValue)
            {
                year = now.Year;
            }

            _logger.LogInformation("Generando reporte de ventas con filtros {@Filter}", new
            {
                Month = month,
                Year = year,
                StartDate = startDate,
                EndDate = endDate,
                filter.ProductId,
                filter.CategoryId
            });

            var query = _context.Set<Venta>()
                .AsNoTracking()
                .Where(v => v.Estado != "CANCELADA")
                .Include(v => v.Usuario)
                .Include(v => v.VentaItems)
                    .ThenInclude(vi => vi.Producto)
                        .ThenInclude(p => p!.Categoria)
                .AsQueryable();

            if (year.HasValue)
            {
                query = query.Where(v => v.FechaVenta.Year == year.Value);
            }

            if (month.HasValue)
            {
                query = query.Where(v => v.FechaVenta.Month == month.Value);
            }

            if (startDate.HasValue)
            {
                query = query.Where(v => v.FechaVenta >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                var inclusiveEndDate = endDate.Value.AddDays(1).AddTicks(-1);
                query = query.Where(v => v.FechaVenta <= inclusiveEndDate);
            }

            if (filter.ProductId.HasValue)
            {
                query = query.Where(v => v.VentaItems.Any(vi => vi.ProductoId == filter.ProductId.Value));
            }

            if (filter.CategoryId.HasValue)
            {
                query = query.Where(v => v.VentaItems.Any(vi =>
                    vi.Producto != null && vi.Producto.CategoriaId == filter.CategoryId.Value));
            }

            var ventas = await query
                .OrderByDescending(v => v.FechaVenta)
                .ToListAsync();

            var totalVentas = ventas.Sum(v => v.Total);
            var totalImpuestos = ventas.Sum(v => v.TotalImpuestos);
            var totalDescuentos = ventas.Sum(v => v.TotalDescuentos);
            var cantidadVentas = ventas.Count;
            var totalItemsVendidos = ventas.Sum(v => v.VentaItems.Sum(item => item.Cantidad));

            var ventasDto = ventas.Select(v => new SalesReportVentaDto
            {
                Id = v.Id,
                NumeroVenta = v.NumeroVenta,
                FechaVenta = v.FechaVenta,
                Subtotal = v.Subtotal,
                TotalImpuestos = v.TotalImpuestos,
                TotalDescuentos = v.TotalDescuentos,
                Total = v.Total,
                Estado = v.Estado,
                UsuarioNombre = BuildUsuarioNombre(v.Usuario),
                CantidadItems = v.VentaItems.Sum(item => item.Cantidad)
            }).ToList();

            var productosDto = ventas
                .SelectMany(v => v.VentaItems)
                .GroupBy(item => new
                {
                    item.ProductoId,
                    item.ProductoNombre,
                    CategoriaNombre = item.Producto?.Categoria?.Nombre
                })
                .Select(group => new SalesReportProductoDto
                {
                    ProductoId = group.Key.ProductoId,
                    ProductoNombre = group.Key.ProductoNombre,
                    CategoriaNombre = group.Key.CategoriaNombre,
                    CantidadVendida = group.Sum(item => item.Cantidad),
                    TotalVendido = group.Sum(item => item.Total)
                })
                .OrderByDescending(p => p.TotalVendido)
                .ToList();

            var promedioVenta = cantidadVentas > 0 ? Math.Round(totalVentas / cantidadVentas, 2) : 0m;

            return new SalesReportResponse
            {
                TotalVentas = totalVentas,
                TotalImpuestos = totalImpuestos,
                TotalDescuentos = totalDescuentos,
                CantidadVentas = cantidadVentas,
                TotalItemsVendidos = totalItemsVendidos,
                PromedioVenta = promedioVenta,
                Mes = month,
                Anio = year,
                FechaInicio = startDate,
                FechaFin = endDate,
                PeriodoDescripcion = BuildPeriodoDescripcion(month, year, startDate, endDate),
                Ventas = ventasDto,
                Productos = productosDto
            };
        }

        private static void ValidateFilter(SalesReportFilterRequest filter)
        {
            if (filter.Month.HasValue && (filter.Month < 1 || filter.Month > 12))
            {
                throw new ArgumentException("El mes debe estar entre 1 y 12.");
            }

            if (filter.Year.HasValue && (filter.Year < 2000 || filter.Year > DateTime.UtcNow.Year + 5))
            {
                throw new ArgumentException("El año proporcionado no es válido.");
            }

            if (filter.StartDate.HasValue && filter.EndDate.HasValue && filter.EndDate.Value < filter.StartDate.Value)
            {
                throw new ArgumentException("La fecha final no puede ser menor que la fecha inicial.");
            }

            if (filter.ProductId.HasValue && filter.ProductId <= 0)
            {
                throw new ArgumentException("El identificador del producto no es válido.");
            }

            if (filter.CategoryId.HasValue && filter.CategoryId <= 0)
            {
                throw new ArgumentException("El identificador de la categoría no es válido.");
            }
        }

        private static string BuildUsuarioNombre(User? usuario)
        {
            if (usuario == null)
            {
                return string.Empty;
            }

            var fullName = $"{usuario.FirstName} {usuario.LastName}".Trim();
            return string.IsNullOrWhiteSpace(fullName) ? usuario.Username : fullName;
        }

        private static string BuildPeriodoDescripcion(int? month, int? year, DateTime? startDate, DateTime? endDate)
        {
            if (month.HasValue && year.HasValue)
            {
                var culture = new CultureInfo("es-ES");
                var date = new DateTime(year.Value, month.Value, 1);
                return culture.TextInfo.ToTitleCase(date.ToString("MMMM yyyy", culture));
            }

            if (year.HasValue)
            {
                return year.Value.ToString();
            }

            if (startDate.HasValue && endDate.HasValue)
            {
                var culture = new CultureInfo("es-ES");
                return $"{startDate.Value.ToString("dd 'de' MMMM yyyy", culture)} - {endDate.Value.ToString("dd 'de' MMMM yyyy", culture)}";
            }

            if (startDate.HasValue)
            {
                var culture = new CultureInfo("es-ES");
                return $"Desde {startDate.Value.ToString("dd 'de' MMMM yyyy", culture)}";
            }

            if (endDate.HasValue)
            {
                var culture = new CultureInfo("es-ES");
                return $"Hasta {endDate.Value.ToString("dd 'de' MMMM yyyy", culture)}";
            }

            return "Todo el periodo";
        }
    }
}
