using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using jam_POS.Application.DTOs.Common;
using jam_POS.Application.DTOs.Requests;
using jam_POS.Application.DTOs.Responses;
using jam_POS.Core.Entities;
using jam_POS.Infrastructure.Data;
using System.Linq.Expressions;

namespace jam_POS.Application.Services
{
    public class ClienteService : IClienteService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ClienteService> _logger;

        public ClienteService(ApplicationDbContext context, ILogger<ClienteService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<PagedResult<ClienteResponse>> GetClientesAsync(ClienteFilterRequest filter)
        {
            _logger.LogInformation("Recuperando clientes con filtros: {@Filter}", filter);

            var query = _context.Clientes.AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                var searchTerm = filter.SearchTerm.Trim().ToLower();
                query = query.Where(c =>
                    c.Nombre.ToLower().Contains(searchTerm) ||
                    (c.Apellido != null && c.Apellido.ToLower().Contains(searchTerm)) ||
                    (c.Email != null && c.Email.ToLower().Contains(searchTerm)) ||
                    (c.Telefono != null && c.Telefono.ToLower().Contains(searchTerm)) ||
                    (c.Documento != null && c.Documento.ToLower().Contains(searchTerm)));
            }

            if (filter.Activo.HasValue)
            {
                query = query.Where(c => c.Activo == filter.Activo.Value);
            }

            var totalCount = await query.CountAsync();

            query = ApplyOrdering(query, filter.OrderBy, filter.OrderDescending);

            var items = await query
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            var mappedItems = items.Select(MapToResponse).ToList();

            return new PagedResult<ClienteResponse>
            {
                Items = mappedItems,
                TotalCount = totalCount,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize
            };
        }

        public async Task<ClienteResponse?> GetClienteByIdAsync(int id)
        {
            _logger.LogInformation("Recuperando cliente con ID: {Id}", id);

            var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.Id == id);

            if (cliente == null)
            {
                _logger.LogWarning("Cliente no encontrado con ID: {Id}", id);
                return null;
            }

            return MapToResponse(cliente);
        }

        public async Task<IEnumerable<ClienteResponse>> GetClientesActivosAsync()
        {
            _logger.LogInformation("Recuperando clientes activos");

            var clientes = await _context.Clientes
                .Where(c => c.Activo)
                .OrderBy(c => c.Nombre)
                .ThenBy(c => c.Apellido)
                .ToListAsync();

            return clientes.Select(MapToResponse);
        }

        public async Task<ClienteResponse> CreateClienteAsync(CreateClienteRequest request)
        {
            _logger.LogInformation("Creando nuevo cliente: {Nombre} {Apellido}", request.Nombre, request.Apellido);

            await ValidateUniqueFieldsAsync(request.Email, request.Documento);

            var cliente = new Cliente
            {
                Nombre = request.Nombre.Trim(),
                Apellido = string.IsNullOrWhiteSpace(request.Apellido) ? null : request.Apellido.Trim(),
                Email = string.IsNullOrWhiteSpace(request.Email) ? null : request.Email.Trim(),
                Telefono = string.IsNullOrWhiteSpace(request.Telefono) ? null : request.Telefono.Trim(),
                Documento = string.IsNullOrWhiteSpace(request.Documento) ? null : request.Documento.Trim(),
                Direccion = string.IsNullOrWhiteSpace(request.Direccion) ? null : request.Direccion.Trim(),
                Notas = string.IsNullOrWhiteSpace(request.Notas) ? null : request.Notas.Trim(),
                FechaNacimiento = request.FechaNacimiento,
                Activo = request.Activo,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Clientes.Add(cliente);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Cliente creado exitosamente con ID: {Id}", cliente.Id);

            return MapToResponse(cliente);
        }

        public async Task<ClienteResponse> UpdateClienteAsync(int id, UpdateClienteRequest request)
        {
            _logger.LogInformation("Actualizando cliente con ID: {Id}", id);

            var cliente = await _context.Clientes.FindAsync(id);

            if (cliente == null)
            {
                _logger.LogWarning("Cliente no encontrado para actualización con ID: {Id}", id);
                throw new KeyNotFoundException($"Cliente con ID {id} no encontrado");
            }

            await ValidateUniqueFieldsAsync(request.Email, request.Documento, id);

            cliente.Nombre = request.Nombre.Trim();
            cliente.Apellido = string.IsNullOrWhiteSpace(request.Apellido) ? null : request.Apellido.Trim();
            cliente.Email = string.IsNullOrWhiteSpace(request.Email) ? null : request.Email.Trim();
            cliente.Telefono = string.IsNullOrWhiteSpace(request.Telefono) ? null : request.Telefono.Trim();
            cliente.Documento = string.IsNullOrWhiteSpace(request.Documento) ? null : request.Documento.Trim();
            cliente.Direccion = string.IsNullOrWhiteSpace(request.Direccion) ? null : request.Direccion.Trim();
            cliente.Notas = string.IsNullOrWhiteSpace(request.Notas) ? null : request.Notas.Trim();
            cliente.FechaNacimiento = request.FechaNacimiento;
            cliente.Activo = request.Activo;
            cliente.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Cliente actualizado exitosamente con ID: {Id}", id);

            return MapToResponse(cliente);
        }

        public async Task<bool> DeleteClienteAsync(int id)
        {
            _logger.LogInformation("Eliminando cliente con ID: {Id}", id);

            var cliente = await _context.Clientes.FindAsync(id);

            if (cliente == null)
            {
                _logger.LogWarning("Cliente no encontrado para eliminación con ID: {Id}", id);
                return false;
            }

            _context.Clientes.Remove(cliente);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Cliente eliminado exitosamente con ID: {Id}", id);

            return true;
        }

        private async Task ValidateUniqueFieldsAsync(string? email, string? documento, int? excludeId = null)
        {
            if (!string.IsNullOrWhiteSpace(email))
            {
                var normalizedEmail = email.Trim().ToLower();
                var emailExists = await _context.Clientes.AnyAsync(c =>
                    c.Email != null && c.Email.ToLower() == normalizedEmail && (!excludeId.HasValue || c.Id != excludeId.Value));

                if (emailExists)
                {
                    throw new InvalidOperationException($"Ya existe un cliente con el email '{email.Trim()}'");
                }
            }

            if (!string.IsNullOrWhiteSpace(documento))
            {
                var normalizedDocumento = documento.Trim().ToLower();
                var documentoExists = await _context.Clientes.AnyAsync(c =>
                    c.Documento != null && c.Documento.ToLower() == normalizedDocumento && (!excludeId.HasValue || c.Id != excludeId.Value));

                if (documentoExists)
                {
                    throw new InvalidOperationException($"Ya existe un cliente con el documento '{documento.Trim()}'");
                }
            }
        }

        private static IQueryable<Cliente> ApplyOrdering(IQueryable<Cliente> query, string? orderBy, bool descending)
        {
            Expression<Func<Cliente, object>> keySelector = orderBy?.ToLower() switch
            {
                "email" => c => c.Email ?? string.Empty,
                "telefono" => c => c.Telefono ?? string.Empty,
                "documento" => c => c.Documento ?? string.Empty,
                "fechanacimiento" => c => c.FechaNacimiento ?? DateTime.MinValue,
                "activo" => c => c.Activo,
                "createdat" => c => c.CreatedAt,
                "updatedat" => c => c.UpdatedAt,
                _ => c => c.Nombre
            };

            return descending ? query.OrderByDescending(keySelector) : query.OrderBy(keySelector);
        }

        private static ClienteResponse MapToResponse(Cliente cliente)
        {
            var nombreCompleto = string.Join(" ", new[] { cliente.Nombre, cliente.Apellido }
                .Where(part => !string.IsNullOrWhiteSpace(part)));

            return new ClienteResponse
            {
                Id = cliente.Id,
                Nombre = cliente.Nombre,
                Apellido = cliente.Apellido,
                NombreCompleto = string.IsNullOrWhiteSpace(nombreCompleto) ? cliente.Nombre : nombreCompleto,
                Email = cliente.Email,
                Telefono = cliente.Telefono,
                Documento = cliente.Documento,
                Direccion = cliente.Direccion,
                Notas = cliente.Notas,
                FechaNacimiento = cliente.FechaNacimiento,
                Activo = cliente.Activo,
                CreatedAt = cliente.CreatedAt,
                UpdatedAt = cliente.UpdatedAt
            };
        }
    }
}
