using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using jam_POS.Infrastructure.Data;
using jam_POS.Core.Entities;
using jam_POS.Application.DTOs.Requests;
using jam_POS.Application.DTOs.Responses;

namespace jam_POS.Application.Services
{
    public class RoleService : IRoleService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<RoleService> _logger;

        public RoleService(ApplicationDbContext context, ILogger<RoleService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<RoleResponse>> GetAllRolesAsync(bool includePermissions = false)
        {
            _logger.LogInformation("Recuperando todos los roles");

            var query = _context.Set<Role>()
                .Include(r => r.Users)
                .AsQueryable();

            if (includePermissions)
            {
                query = query.Include(r => r.RolePermissions)
                    .ThenInclude(rp => rp.Permission);
            }

            var roles = await query
                .OrderBy(r => r.Name)
                .ToListAsync();

            return roles.Select(r => MapToResponse(r, includePermissions));
        }

        public async Task<RoleResponse?> GetRoleByIdAsync(int id, bool includePermissions = true)
        {
            _logger.LogInformation("Recuperando rol con ID: {Id}", id);

            var query = _context.Set<Role>()
                .Include(r => r.Users)
                .AsQueryable();

            if (includePermissions)
            {
                query = query.Include(r => r.RolePermissions)
                    .ThenInclude(rp => rp.Permission);
            }

            var role = await query.FirstOrDefaultAsync(r => r.Id == id);
            
            if (role == null)
            {
                _logger.LogWarning("Rol no encontrado con ID: {Id}", id);
                return null;
            }

            return MapToResponse(role, includePermissions);
        }

        public async Task<RoleResponse> CreateRoleAsync(CreateRoleRequest request)
        {
            _logger.LogInformation("Creando nuevo rol: {Name}", request.Name);

            // Validar que el nombre no exista en el tenant actual
            // El query filter ya se encarga de filtrar por tenant
            var existingRole = await _context.Set<Role>()
                .FirstOrDefaultAsync(r => r.Name.ToLower() == request.Name.ToLower());

            if (existingRole != null)
            {
                throw new InvalidOperationException($"Ya existe un rol con el nombre '{request.Name}' en tu empresa");
            }

            var role = new Role
            {
                Name = request.Name,
                Description = request.Description,
                IsSystem = false, // Los roles creados por empresas NUNCA son de sistema
                Activo = request.Activo,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
                // EmpresaId se asigna automáticamente en SaveChanges
            };

            _context.Set<Role>().Add(role);
            await _context.SaveChangesAsync();

            // Asignar permisos
            if (request.PermissionIds.Any())
            {
                await AssignPermissionsToRole(role.Id, request.PermissionIds);
            }

            _logger.LogInformation("Rol creado exitosamente con ID: {Id}", role.Id);

            return (await GetRoleByIdAsync(role.Id))!;
        }

        public async Task<RoleResponse> UpdateRoleAsync(int id, UpdateRoleRequest request)
        {
            _logger.LogInformation("Actualizando rol con ID: {Id}", id);

            var role = await _context.Set<Role>()
                .Include(r => r.RolePermissions)
                .FirstOrDefaultAsync(r => r.Id == id);
            
            if (role == null)
            {
                _logger.LogWarning("Rol no encontrado para actualización con ID: {Id}", id);
                throw new KeyNotFoundException($"Rol con ID {id} no encontrado");
            }

            // No permitir editar roles de sistema
            if (role.IsSystem)
            {
                throw new InvalidOperationException("No se puede editar un rol de sistema");
            }

            // Validar nombre único en el tenant actual (excepto el mismo rol)
            var existingRole = await _context.Set<Role>()
                .FirstOrDefaultAsync(r => r.Name.ToLower() == request.Name.ToLower() && r.Id != id);

            if (existingRole != null)
            {
                throw new InvalidOperationException($"Ya existe un rol con el nombre '{request.Name}' en tu empresa");
            }

            role.Name = request.Name;
            role.Description = request.Description;
            role.Activo = request.Activo;
            role.UpdatedAt = DateTime.UtcNow;

            // Actualizar permisos
            // Eliminar permisos existentes
            var existingPermissions = role.RolePermissions.ToList();
            _context.Set<RolePermission>().RemoveRange(existingPermissions);

            await _context.SaveChangesAsync();

            // Asignar nuevos permisos
            if (request.PermissionIds.Any())
            {
                await AssignPermissionsToRole(role.Id, request.PermissionIds);
            }

            _logger.LogInformation("Rol actualizado exitosamente con ID: {Id}", id);

            return (await GetRoleByIdAsync(id))!;
        }

        public async Task<bool> DeleteRoleAsync(int id)
        {
            _logger.LogInformation("Eliminando rol con ID: {Id}", id);

            var role = await _context.Set<Role>()
                .Include(r => r.Users)
                .Include(r => r.RolePermissions)
                .FirstOrDefaultAsync(r => r.Id == id);
            
            if (role == null)
            {
                _logger.LogWarning("Rol no encontrado para eliminación con ID: {Id}", id);
                return false;
            }

            // No permitir eliminar roles de sistema
            if (role.IsSystem)
            {
                throw new InvalidOperationException("No se puede eliminar un rol de sistema");
            }

            // Verificar si tiene usuarios asignados
            if (role.Users.Any())
            {
                throw new InvalidOperationException($"No se puede eliminar el rol porque tiene {role.Users.Count} usuario(s) asignado(s)");
            }

            // Eliminar permisos asociados
            _context.Set<RolePermission>().RemoveRange(role.RolePermissions);
            
            // Eliminar rol
            _context.Set<Role>().Remove(role);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Rol eliminado exitosamente con ID: {Id}", id);

            return true;
        }

        public async Task<bool> RoleExistsAsync(int id)
        {
            return await _context.Set<Role>().AnyAsync(r => r.Id == id);
        }

        public async Task<IEnumerable<RoleResponse>> GetActiveRolesAsync()
        {
            // Obtener roles activos del sistema + roles activos del tenant
            // El query filter ya se encarga de filtrar: IsSystem=true OR EmpresaId=CurrentTenant
            var roles = await _context.Set<Role>()
                .Where(r => r.Activo)
                .Include(r => r.Users)
                .OrderBy(r => r.IsSystem ? 0 : 1) // Roles de sistema primero
                .ThenBy(r => r.Name)
                .ToListAsync();

            return roles.Select(r => MapToResponse(r, false));
        }

        private async Task AssignPermissionsToRole(int roleId, List<int> permissionIds)
        {
            var rolePermissions = permissionIds.Select(permissionId => new RolePermission
            {
                RoleId = roleId,
                PermissionId = permissionId,
                GrantedAt = DateTime.UtcNow
            }).ToList();

            _context.Set<RolePermission>().AddRange(rolePermissions);
            await _context.SaveChangesAsync();
        }

        private static RoleResponse MapToResponse(Role role, bool includePermissions)
        {
            var response = new RoleResponse
            {
                Id = role.Id,
                Name = role.Name,
                Description = role.Description,
                IsSystem = role.IsSystem,
                Activo = role.Activo,
                CreatedAt = role.CreatedAt,
                UpdatedAt = role.UpdatedAt,
                UsersCount = role.Users?.Count ?? 0,
                PermissionsCount = role.RolePermissions?.Count ?? 0
            };

            if (includePermissions && role.RolePermissions != null)
            {
                response.Permissions = role.RolePermissions
                    .Select(rp => new PermissionResponse
                    {
                        Id = rp.Permission.Id,
                        Name = rp.Permission.Name,
                        Module = rp.Permission.Module,
                        Description = rp.Permission.Description,
                        IsSystem = rp.Permission.IsSystem,
                        CreatedAt = rp.Permission.CreatedAt
                    })
                    .ToList();
            }

            return response;
        }
    }
}

