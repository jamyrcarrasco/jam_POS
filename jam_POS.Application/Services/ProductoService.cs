using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using jam_POS.Infrastructure.Data;
using jam_POS.Core.Entities;
using jam_POS.Application.DTOs.Requests;
using jam_POS.Application.DTOs.Responses;
using jam_POS.Application.DTOs.Common;
using System.Linq.Expressions;

namespace jam_POS.Application.Services
{
    public class ProductoService : IProductoService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ProductoService> _logger;

        public ProductoService(ApplicationDbContext context, ILogger<ProductoService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<PagedResult<ProductoResponse>> GetAllProductosAsync(ProductFilterRequest filter)
        {
            _logger.LogInformation("Retrieving productos with filters: {@Filter}", filter);

            var query = _context.Productos.AsQueryable();

            // Apply filters
            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                var searchTerm = filter.SearchTerm.ToLower();
                query = query.Where(p => 
                    p.Nombre.ToLower().Contains(searchTerm) || 
                    (p.Descripcion != null && p.Descripcion.ToLower().Contains(searchTerm)) ||
                    (p.CodigoBarras != null && p.CodigoBarras.ToLower().Contains(searchTerm)));
            }

            if (!string.IsNullOrWhiteSpace(filter.Categoria))
            {
                query = query.Where(p => p.Categoria == filter.Categoria);
            }

            if (filter.PrecioMin.HasValue)
            {
                query = query.Where(p => p.Precio >= filter.PrecioMin.Value);
            }

            if (filter.PrecioMax.HasValue)
            {
                query = query.Where(p => p.Precio <= filter.PrecioMax.Value);
            }

            if (filter.StockBajo.HasValue && filter.StockBajo.Value)
            {
                query = query.Where(p => p.StockMinimo.HasValue && p.Stock <= p.StockMinimo.Value);
            }

            if (filter.Activo.HasValue)
            {
                query = query.Where(p => p.Activo == filter.Activo.Value);
            }

            // Get total count before pagination
            var totalCount = await query.CountAsync();

            // Apply ordering
            query = ApplyOrdering(query, filter.OrderBy, filter.OrderDescending);

            // Apply pagination
            var items = await query
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            var mappedItems = items.Select(MapToResponse).ToList();

            return new PagedResult<ProductoResponse>
            {
                Items = mappedItems,
                TotalCount = totalCount,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize
            };
        }

        public async Task<ProductoResponse?> GetProductoByIdAsync(int id)
        {
            _logger.LogInformation("Retrieving producto with ID: {Id}", id);

            var producto = await _context.Productos.FindAsync(id);
            
            if (producto == null)
            {
                _logger.LogWarning("Producto not found with ID: {Id}", id);
                return null;
            }

            return MapToResponse(producto);
        }

        public async Task<ProductoResponse> CreateProductoAsync(CreateProductoRequest request)
        {
            _logger.LogInformation("Creating new producto: {Nombre}", request.Nombre);

            var producto = new Producto
            {
                Nombre = request.Nombre,
                Descripcion = request.Descripcion,
                Precio = request.Precio,
                Stock = request.Stock,
                Categoria = request.Categoria,
                CodigoBarras = request.CodigoBarras,
                ImagenUrl = request.ImagenUrl,
                PrecioCompra = request.PrecioCompra,
                MargenGanancia = request.MargenGanancia,
                StockMinimo = request.StockMinimo,
                Activo = request.Activo,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Productos.Add(producto);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Producto created successfully with ID: {Id}", producto.Id);

            return MapToResponse(producto);
        }

        public async Task<ProductoResponse> UpdateProductoAsync(int id, UpdateProductoRequest request)
        {
            _logger.LogInformation("Updating producto with ID: {Id}", id);

            var producto = await _context.Productos.FindAsync(id);
            
            if (producto == null)
            {
                _logger.LogWarning("Producto not found for update with ID: {Id}", id);
                throw new KeyNotFoundException($"Producto with ID {id} not found");
            }

            producto.Nombre = request.Nombre;
            producto.Descripcion = request.Descripcion;
            producto.Precio = request.Precio;
            producto.Stock = request.Stock;
            producto.Categoria = request.Categoria;
            producto.CodigoBarras = request.CodigoBarras;
            producto.ImagenUrl = request.ImagenUrl;
            producto.PrecioCompra = request.PrecioCompra;
            producto.MargenGanancia = request.MargenGanancia;
            producto.StockMinimo = request.StockMinimo;
            producto.Activo = request.Activo;
            producto.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Producto updated successfully with ID: {Id}", id);

            return MapToResponse(producto);
        }

        public async Task<bool> DeleteProductoAsync(int id)
        {
            _logger.LogInformation("Deleting producto with ID: {Id}", id);

            var producto = await _context.Productos.FindAsync(id);
            
            if (producto == null)
            {
                _logger.LogWarning("Producto not found for deletion with ID: {Id}", id);
                return false;
            }

            _context.Productos.Remove(producto);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Producto deleted successfully with ID: {Id}", id);

            return true;
        }

        public async Task<bool> ProductoExistsAsync(int id)
        {
            return await _context.Productos.AnyAsync(e => e.Id == id);
        }

        public async Task<IEnumerable<string>> GetCategoriasAsync()
        {
            var categorias = await _context.Productos
                .Where(p => p.Categoria != null)
                .Select(p => p.Categoria!)
                .Distinct()
                .OrderBy(c => c)
                .ToListAsync();

            return categorias;
        }

        private static IQueryable<Producto> ApplyOrdering(IQueryable<Producto> query, string? orderBy, bool descending)
        {
            if (string.IsNullOrWhiteSpace(orderBy))
            {
                return descending ? query.OrderByDescending(p => p.Nombre) : query.OrderBy(p => p.Nombre);
            }

            Expression<Func<Producto, object>> keySelector = orderBy.ToLower() switch
            {
                "nombre" => p => p.Nombre,
                "precio" => p => p.Precio,
                "stock" => p => p.Stock,
                "categoria" => p => p.Categoria ?? string.Empty,
                "createdat" => p => p.CreatedAt,
                _ => p => p.Nombre
            };

            return descending ? query.OrderByDescending(keySelector) : query.OrderBy(keySelector);
        }

        private static ProductoResponse MapToResponse(Producto producto)
        {
            return new ProductoResponse
            {
                Id = producto.Id,
                Nombre = producto.Nombre,
                Descripcion = producto.Descripcion,
                Precio = producto.Precio,
                Stock = producto.Stock,
                Categoria = producto.Categoria,
                CodigoBarras = producto.CodigoBarras,
                ImagenUrl = producto.ImagenUrl,
                PrecioCompra = producto.PrecioCompra,
                MargenGanancia = producto.MargenGanancia,
                StockMinimo = producto.StockMinimo,
                Activo = producto.Activo,
                CreatedAt = producto.CreatedAt,
                UpdatedAt = producto.UpdatedAt
            };
        }
    }
}
