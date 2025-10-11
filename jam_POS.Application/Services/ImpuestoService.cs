using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using jam_POS.Infrastructure.Data;
using jam_POS.Core.Entities;
using jam_POS.Application.DTOs.Requests;
using jam_POS.Application.DTOs.Responses;

namespace jam_POS.Application.Services
{
    public class ImpuestoService : IImpuestoService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ImpuestoService> _logger;

        public ImpuestoService(ApplicationDbContext context, ILogger<ImpuestoService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<ImpuestoResponse>> GetAllImpuestosAsync()
        {
            _logger.LogInformation("Recuperando todos los impuestos");

            var impuestos = await _context.Set<Impuesto>()
                .OrderByDescending(i => i.AplicaPorDefecto)
                .ThenBy(i => i.Nombre)
                .ToListAsync();

            return impuestos.Select(MapToResponse);
        }

        public async Task<ImpuestoResponse?> GetImpuestoByIdAsync(int id)
        {
            _logger.LogInformation("Recuperando impuesto con ID: {Id}", id);

            var impuesto = await _context.Set<Impuesto>().FindAsync(id);
            
            if (impuesto == null)
            {
                _logger.LogWarning("Impuesto no encontrado con ID: {Id}", id);
                return null;
            }

            return MapToResponse(impuesto);
        }

        public async Task<ImpuestoResponse> CreateImpuestoAsync(CreateImpuestoRequest request)
        {
            _logger.LogInformation("Creando nuevo impuesto: {Nombre}", request.Nombre);

            // Si se marca como por defecto, desmarcar otros
            if (request.AplicaPorDefecto)
            {
                await DesmarcarImpuestosDefectoAsync();
            }

            var impuesto = new Impuesto
            {
                Nombre = request.Nombre,
                Descripcion = request.Descripcion,
                Tipo = request.Tipo,
                Porcentaje = request.Porcentaje,
                MontoFijo = request.MontoFijo,
                AplicaPorDefecto = request.AplicaPorDefecto,
                Activo = request.Activo,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
                // EmpresaId se asigna automáticamente en SaveChanges
            };

            _context.Set<Impuesto>().Add(impuesto);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Impuesto creado exitosamente con ID: {Id}", impuesto.Id);

            return MapToResponse(impuesto);
        }

        public async Task<ImpuestoResponse> UpdateImpuestoAsync(int id, UpdateImpuestoRequest request)
        {
            _logger.LogInformation("Actualizando impuesto con ID: {Id}", id);

            var impuesto = await _context.Set<Impuesto>().FindAsync(id);
            
            if (impuesto == null)
            {
                _logger.LogWarning("Impuesto no encontrado para actualización con ID: {Id}", id);
                throw new KeyNotFoundException($"Impuesto con ID {id} no encontrado");
            }

            // Si se marca como por defecto, desmarcar otros
            if (request.AplicaPorDefecto && !impuesto.AplicaPorDefecto)
            {
                await DesmarcarImpuestosDefectoAsync();
            }

            impuesto.Nombre = request.Nombre;
            impuesto.Descripcion = request.Descripcion;
            impuesto.Tipo = request.Tipo;
            impuesto.Porcentaje = request.Porcentaje;
            impuesto.MontoFijo = request.MontoFijo;
            impuesto.AplicaPorDefecto = request.AplicaPorDefecto;
            impuesto.Activo = request.Activo;
            impuesto.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Impuesto actualizado exitosamente con ID: {Id}", id);

            return MapToResponse(impuesto);
        }

        public async Task<bool> DeleteImpuestoAsync(int id)
        {
            _logger.LogInformation("Eliminando impuesto con ID: {Id}", id);

            var impuesto = await _context.Set<Impuesto>().FindAsync(id);
            
            if (impuesto == null)
            {
                _logger.LogWarning("Impuesto no encontrado para eliminación con ID: {Id}", id);
                return false;
            }

            _context.Set<Impuesto>().Remove(impuesto);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Impuesto eliminado exitosamente con ID: {Id}", id);

            return true;
        }

        public async Task<IEnumerable<ImpuestoResponse>> GetImpuestosActivosAsync()
        {
            var impuestos = await _context.Set<Impuesto>()
                .Where(i => i.Activo)
                .OrderByDescending(i => i.AplicaPorDefecto)
                .ThenBy(i => i.Nombre)
                .ToListAsync();

            return impuestos.Select(MapToResponse);
        }

        private async Task DesmarcarImpuestosDefectoAsync()
        {
            var impuestosDefecto = await _context.Set<Impuesto>()
                .Where(i => i.AplicaPorDefecto)
                .ToListAsync();

            foreach (var imp in impuestosDefecto)
            {
                imp.AplicaPorDefecto = false;
            }
        }

        private static ImpuestoResponse MapToResponse(Impuesto impuesto)
        {
            return new ImpuestoResponse
            {
                Id = impuesto.Id,
                Nombre = impuesto.Nombre,
                Descripcion = impuesto.Descripcion,
                Tipo = impuesto.Tipo,
                Porcentaje = impuesto.Porcentaje,
                MontoFijo = impuesto.MontoFijo,
                AplicaPorDefecto = impuesto.AplicaPorDefecto,
                Activo = impuesto.Activo,
                CreatedAt = impuesto.CreatedAt,
                UpdatedAt = impuesto.UpdatedAt
            };
        }
    }
}

