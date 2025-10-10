using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using jam_POS.Infrastructure.Data;
using jam_POS.Core.Entities;
using jam_POS.Application.DTOs.Responses;

namespace jam_POS.Application.Services
{
    public class PermissionService : IPermissionService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<PermissionService> _logger;

        public PermissionService(ApplicationDbContext context, ILogger<PermissionService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<PermissionResponse>> GetAllPermissionsAsync()
        {
            _logger.LogInformation("Recuperando todos los permisos");

            var permissions = await _context.Set<Permission>()
                .OrderBy(p => p.Module)
                .ThenBy(p => p.Name)
                .ToListAsync();

            return permissions.Select(MapToResponse);
        }

        public async Task<Dictionary<string, List<PermissionResponse>>> GetPermissionsByModuleAsync()
        {
            _logger.LogInformation("Recuperando permisos agrupados por módulo");

            var permissions = await _context.Set<Permission>()
                .OrderBy(p => p.Module)
                .ThenBy(p => p.Name)
                .ToListAsync();

            return permissions
                .GroupBy(p => p.Module)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(MapToResponse).ToList()
                );
        }

        public async Task<PermissionResponse?> GetPermissionByIdAsync(int id)
        {
            _logger.LogInformation("Recuperando permiso con ID: {Id}", id);

            var permission = await _context.Set<Permission>().FindAsync(id);
            
            if (permission == null)
            {
                _logger.LogWarning("Permiso no encontrado con ID: {Id}", id);
                return null;
            }

            return MapToResponse(permission);
        }

        private static PermissionResponse MapToResponse(Permission permission)
        {
            return new PermissionResponse
            {
                Id = permission.Id,
                Name = permission.Name,
                Module = permission.Module,
                Description = permission.Description,
                IsSystem = permission.IsSystem,
                CreatedAt = permission.CreatedAt
            };
        }
    }
}

