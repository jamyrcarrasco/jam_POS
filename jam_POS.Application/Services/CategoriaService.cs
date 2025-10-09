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
    public class CategoriaService : ICategoriaService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CategoriaService> _logger;

        public CategoriaService(ApplicationDbContext context, ILogger<CategoriaService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<PagedResult<CategoriaResponse>> GetAllCategoriasAsync(CategoriaFilterRequest filter)
        {
            _logger.LogInformation("Recuperando categorías con filtros: {@Filter}", filter);

            var query = _context.Set<Categoria>().AsQueryable();

            // Apply filters
            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                var searchTerm = filter.SearchTerm.ToLower();
                query = query.Where(c => 
                    c.Nombre.ToLower().Contains(searchTerm) || 
                    (c.Descripcion != null && c.Descripcion.ToLower().Contains(searchTerm)));
            }

            if (filter.Activo.HasValue)
            {
                query = query.Where(c => c.Activo == filter.Activo.Value);
            }

            // Get total count before pagination
            var totalCount = await query.CountAsync();

            // Apply ordering
            query = ApplyOrdering(query, filter.OrderBy, filter.OrderDescending);

            // Apply pagination
            var items = await query
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .Include(c => c.Productos)
                .ToListAsync();

            var mappedItems = items.Select(MapToResponse).ToList();

            return new PagedResult<CategoriaResponse>
            {
                Items = mappedItems,
                TotalCount = totalCount,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize
            };
        }

        public async Task<CategoriaResponse?> GetCategoriaByIdAsync(int id)
        {
            _logger.LogInformation("Recuperando categoría con ID: {Id}", id);

            var categoria = await _context.Set<Categoria>()
                .Include(c => c.Productos)
                .FirstOrDefaultAsync(c => c.Id == id);
            
            if (categoria == null)
            {
                _logger.LogWarning("Categoría no encontrada con ID: {Id}", id);
                return null;
            }

            return MapToResponse(categoria);
        }

        public async Task<CategoriaResponse> CreateCategoriaAsync(CreateCategoriaRequest request)
        {
            _logger.LogInformation("Creando nueva categoría: {Nombre}", request.Nombre);

            var categoria = new Categoria
            {
                Nombre = request.Nombre,
                Descripcion = request.Descripcion,
                Color = request.Color,
                Icono = request.Icono,
                Activo = request.Activo,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Set<Categoria>().Add(categoria);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Categoría creada exitosamente con ID: {Id}", categoria.Id);

            return MapToResponse(categoria);
        }

        public async Task<CategoriaResponse> UpdateCategoriaAsync(int id, UpdateCategoriaRequest request)
        {
            _logger.LogInformation("Actualizando categoría con ID: {Id}", id);

            var categoria = await _context.Set<Categoria>().FindAsync(id);
            
            if (categoria == null)
            {
                _logger.LogWarning("Categoría no encontrada para actualización con ID: {Id}", id);
                throw new KeyNotFoundException($"Categoría con ID {id} no encontrada");
            }

            categoria.Nombre = request.Nombre;
            categoria.Descripcion = request.Descripcion;
            categoria.Color = request.Color;
            categoria.Icono = request.Icono;
            categoria.Activo = request.Activo;
            categoria.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Categoría actualizada exitosamente con ID: {Id}", id);

            return MapToResponse(categoria);
        }

        public async Task<bool> DeleteCategoriaAsync(int id)
        {
            _logger.LogInformation("Eliminando categoría con ID: {Id}", id);

            var categoria = await _context.Set<Categoria>()
                .Include(c => c.Productos)
                .FirstOrDefaultAsync(c => c.Id == id);
            
            if (categoria == null)
            {
                _logger.LogWarning("Categoría no encontrada para eliminación con ID: {Id}", id);
                return false;
            }

            // Check if category has products
            if (categoria.Productos.Any())
            {
                _logger.LogWarning("No se puede eliminar la categoría {Id} porque tiene productos asociados", id);
                throw new InvalidOperationException("No se puede eliminar una categoría que tiene productos asociados");
            }

            _context.Set<Categoria>().Remove(categoria);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Categoría eliminada exitosamente con ID: {Id}", id);

            return true;
        }

        public async Task<bool> CategoriaExistsAsync(int id)
        {
            return await _context.Set<Categoria>().AnyAsync(e => e.Id == id);
        }

        public async Task<IEnumerable<CategoriaResponse>> GetCategoriasActivasAsync()
        {
            var categorias = await _context.Set<Categoria>()
                .Where(c => c.Activo)
                .Include(c => c.Productos)
                .OrderBy(c => c.Nombre)
                .ToListAsync();

            return categorias.Select(MapToResponse).ToList();
        }

        private static IQueryable<Categoria> ApplyOrdering(IQueryable<Categoria> query, string? orderBy, bool descending)
        {
            if (string.IsNullOrWhiteSpace(orderBy))
            {
                return descending ? query.OrderByDescending(c => c.Nombre) : query.OrderBy(c => c.Nombre);
            }

            Expression<Func<Categoria, object>> keySelector = orderBy.ToLower() switch
            {
                "nombre" => c => c.Nombre,
                "createdat" => c => c.CreatedAt,
                _ => c => c.Nombre
            };

            return descending ? query.OrderByDescending(keySelector) : query.OrderBy(keySelector);
        }

        private static CategoriaResponse MapToResponse(Categoria categoria)
        {
            return new CategoriaResponse
            {
                Id = categoria.Id,
                Nombre = categoria.Nombre,
                Descripcion = categoria.Descripcion,
                Color = categoria.Color,
                Icono = categoria.Icono,
                Activo = categoria.Activo,
                CreatedAt = categoria.CreatedAt,
                UpdatedAt = categoria.UpdatedAt,
                ProductosCount = categoria.Productos?.Count ?? 0
            };
        }
    }
}

