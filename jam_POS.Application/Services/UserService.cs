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
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UserService> _logger;

        public UserService(ApplicationDbContext context, ILogger<UserService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<PagedResult<UserResponse>> GetAllUsersAsync(UserFilterRequest filter)
        {
            _logger.LogInformation("Recuperando usuarios con filtros: {@Filter}", filter);

            var query = _context.Users
                .Include(u => u.Role)
                .AsQueryable();

            // Apply filters
            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                var searchTerm = filter.SearchTerm.ToLower();
                query = query.Where(u => 
                    u.Username.ToLower().Contains(searchTerm) || 
                    u.Email.ToLower().Contains(searchTerm) ||
                    u.FirstName.ToLower().Contains(searchTerm) ||
                    u.LastName.ToLower().Contains(searchTerm));
            }

            if (filter.RoleId.HasValue)
            {
                query = query.Where(u => u.RoleId == filter.RoleId.Value);
            }

            if (filter.IsActive.HasValue)
            {
                query = query.Where(u => u.IsActive == filter.IsActive.Value);
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

            return new PagedResult<UserResponse>
            {
                Items = mappedItems,
                TotalCount = totalCount,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize
            };
        }

        public async Task<UserResponse?> GetUserByIdAsync(int id)
        {
            _logger.LogInformation("Recuperando usuario con ID: {Id}", id);

            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Id == id);
            
            if (user == null)
            {
                _logger.LogWarning("Usuario no encontrado con ID: {Id}", id);
                return null;
            }

            return MapToResponse(user);
        }

        public async Task<UserResponse> CreateUserAsync(CreateUserRequest request)
        {
            _logger.LogInformation("Creando nuevo usuario: {Username}", request.Username);

            // Validar que el username no exista
            if (await UsernameExistsAsync(request.Username))
            {
                throw new InvalidOperationException($"Ya existe un usuario con el nombre '{request.Username}'");
            }

            // Validar que el email no exista
            if (await EmailExistsAsync(request.Email))
            {
                throw new InvalidOperationException($"Ya existe un usuario con el email '{request.Email}'");
            }

            // Validar que el rol exista
            var roleExists = await _context.Set<Role>().AnyAsync(r => r.Id == request.RoleId);
            if (!roleExists)
            {
                throw new InvalidOperationException($"El rol con ID {request.RoleId} no existe");
            }

            var user = new User
            {
                Username = request.Username,
                Email = request.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                FirstName = request.FirstName,
                LastName = request.LastName,
                RoleId = request.RoleId,
                IsActive = request.IsActive,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Usuario creado exitosamente con ID: {Id}", user.Id);

            return (await GetUserByIdAsync(user.Id))!;
        }

        public async Task<UserResponse> UpdateUserAsync(int id, UpdateUserRequest request)
        {
            _logger.LogInformation("Actualizando usuario con ID: {Id}", id);

            var user = await _context.Users.FindAsync(id);
            
            if (user == null)
            {
                _logger.LogWarning("Usuario no encontrado para actualización con ID: {Id}", id);
                throw new KeyNotFoundException($"Usuario con ID {id} no encontrado");
            }

            // Validar username único (excepto el mismo usuario)
            if (await UsernameExistsAsync(request.Username, id))
            {
                throw new InvalidOperationException($"Ya existe un usuario con el nombre '{request.Username}'");
            }

            // Validar email único (excepto el mismo usuario)
            if (await EmailExistsAsync(request.Email, id))
            {
                throw new InvalidOperationException($"Ya existe un usuario con el email '{request.Email}'");
            }

            // Validar que el rol exista
            var roleExists = await _context.Set<Role>().AnyAsync(r => r.Id == request.RoleId);
            if (!roleExists)
            {
                throw new InvalidOperationException($"El rol con ID {request.RoleId} no existe");
            }

            user.Username = request.Username;
            user.Email = request.Email;
            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.RoleId = request.RoleId;
            user.IsActive = request.IsActive;

            // Si se proporciona una nueva contraseña, actualizarla
            if (!string.IsNullOrWhiteSpace(request.Password))
            {
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
            }

            await _context.SaveChangesAsync();

            _logger.LogInformation("Usuario actualizado exitosamente con ID: {Id}", id);

            return (await GetUserByIdAsync(id))!;
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            _logger.LogInformation("Eliminando usuario con ID: {Id}", id);

            var user = await _context.Users.FindAsync(id);
            
            if (user == null)
            {
                _logger.LogWarning("Usuario no encontrado para eliminación con ID: {Id}", id);
                return false;
            }

            // No permitir eliminar el usuario admin principal
            if (user.Id == 1)
            {
                throw new InvalidOperationException("No se puede eliminar el usuario administrador principal");
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Usuario eliminado exitosamente con ID: {Id}", id);

            return true;
        }

        public async Task<bool> UserExistsAsync(int id)
        {
            return await _context.Users.AnyAsync(u => u.Id == id);
        }

        public async Task<bool> UsernameExistsAsync(string username, int? excludeUserId = null)
        {
            var query = _context.Users.Where(u => u.Username.ToLower() == username.ToLower());
            
            if (excludeUserId.HasValue)
            {
                query = query.Where(u => u.Id != excludeUserId.Value);
            }

            return await query.AnyAsync();
        }

        public async Task<bool> EmailExistsAsync(string email, int? excludeUserId = null)
        {
            var query = _context.Users.Where(u => u.Email.ToLower() == email.ToLower());
            
            if (excludeUserId.HasValue)
            {
                query = query.Where(u => u.Id != excludeUserId.Value);
            }

            return await query.AnyAsync();
        }

        private static IQueryable<User> ApplyOrdering(IQueryable<User> query, string? orderBy, bool descending)
        {
            if (string.IsNullOrWhiteSpace(orderBy))
            {
                return descending ? query.OrderByDescending(u => u.Username) : query.OrderBy(u => u.Username);
            }

            Expression<Func<User, object>> keySelector = orderBy.ToLower() switch
            {
                "username" => u => u.Username,
                "email" => u => u.Email,
                "firstname" => u => u.FirstName,
                "lastname" => u => u.LastName,
                "role" => u => u.Role.Name,
                "createdat" => u => u.CreatedAt,
                _ => u => u.Username
            };

            return descending ? query.OrderByDescending(keySelector) : query.OrderBy(keySelector);
        }

        private static UserResponse MapToResponse(User user)
        {
            return new UserResponse
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                RoleId = user.RoleId,
                RoleName = user.Role?.Name ?? string.Empty,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt,
                LastLoginAt = user.LastLoginAt
            };
        }
    }
}

