using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using jam_POS.Infrastructure.Data;
using jam_POS.Core.Entities;
using jam_POS.Application.DTOs.Requests;
using jam_POS.Application.DTOs.Responses;

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

        public async Task<IEnumerable<ProductoResponse>> GetAllProductosAsync()
        {
            _logger.LogInformation("Retrieving all productos");

            var productos = await _context.Productos.ToListAsync();
            
            return productos.Select(MapToResponse);
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
                Precio = request.Precio,
                Stock = request.Stock
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
            producto.Precio = request.Precio;
            producto.Stock = request.Stock;

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

        private static ProductoResponse MapToResponse(Producto producto)
        {
            return new ProductoResponse
            {
                Id = producto.Id,
                Nombre = producto.Nombre,
                Precio = producto.Precio,
                Stock = producto.Stock,
                CreatedAt = DateTime.UtcNow, // You might want to add this to your Producto model
                UpdatedAt = DateTime.UtcNow  // You might want to add this to your Producto model
            };
        }
    }
}
