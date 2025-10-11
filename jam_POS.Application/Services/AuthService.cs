using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using jam_POS.Infrastructure.Data;
using jam_POS.Infrastructure.Services;
using jam_POS.Application.DTOs.Requests;
using jam_POS.Application.DTOs.Responses;
using BCrypt.Net;

namespace jam_POS.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly JwtService _jwtService;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            ApplicationDbContext context,
            JwtService jwtService,
            ILogger<AuthService> logger)
        {
            _context = context;
            _jwtService = jwtService;
            _logger = logger;
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            _logger.LogInformation("Login attempt for username: {Username}", request.Username);

            try
            {
                // Find user by username
                // IMPORTANTE: Usar IgnoreQueryFilters() porque en el momento del login
                // el usuario NO está autenticado y no tenemos TenantId en el contexto.
                // El Global Query Filter bloquearía el login de usuarios con empresa.
                var user = await _context.Users
                    .IgnoreQueryFilters() // ← CRÍTICO para permitir login
                    .Include(u => u.Role)
                    .Include(u => u.Empresa)
                    .FirstOrDefaultAsync(u => u.Username == request.Username && u.IsActive);

                if (user == null)
                {
                    _logger.LogWarning("Login failed: User not found - {Username}", request.Username);
                    throw new UnauthorizedAccessException("Invalid username or password");
                }

                // Verify password
                if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                {
                    _logger.LogWarning("Login failed: Invalid password - {Username}", request.Username);
                    throw new UnauthorizedAccessException("Invalid username or password");
                }

                // Update last login
                user.LastLoginAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                // Generate JWT token
                var token = _jwtService.GenerateToken(user);
                var expiresAt = _jwtService.GetTokenExpiry();

                _logger.LogInformation("Login successful for user: {Username}", request.Username);

                // Prepare empresa response if user has one
                EmpresaResponse? empresaResponse = null;
                if (user.Empresa != null)
                {
                    empresaResponse = new EmpresaResponse
                    {
                        Id = user.Empresa.Id,
                        Nombre = user.Empresa.Nombre,
                        NombreComercial = user.Empresa.NombreComercial,
                        RNC = user.Empresa.RNC,
                        Direccion = user.Empresa.Direccion,
                        Telefono = user.Empresa.Telefono,
                        Email = user.Empresa.Email,
                        LogoUrl = user.Empresa.LogoUrl,
                        Pais = user.Empresa.Pais,
                        Ciudad = user.Empresa.Ciudad,
                        CodigoPostal = user.Empresa.CodigoPostal,
                        Plan = user.Empresa.Plan,
                        FechaVencimientoPlan = user.Empresa.FechaVencimientoPlan,
                        Activo = user.Empresa.Activo,
                        CreatedAt = user.Empresa.CreatedAt
                    };
                }

                return new LoginResponse
                {
                    Token = token,
                    Username = user.Username,
                    Role = user.Role.Name,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    ExpiresAt = expiresAt,
                    Empresa = empresaResponse
                };
            }
            catch (UnauthorizedAccessException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during login for user: {Username}", request.Username);
                throw new InvalidOperationException("An error occurred during login", ex);
            }
        }

        public async Task LogoutAsync()
        {
            // In a stateless JWT implementation, logout is handled on the client side
            // by removing the token from storage
            await Task.CompletedTask;
        }

        public async Task<bool> ValidateTokenAsync(string token)
        {
            try
            {
                // Basic token validation - in a real implementation, you might want to
                // check if the token is blacklisted or validate additional claims
                return !string.IsNullOrEmpty(token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Token validation failed");
                return false;
            }
        }
    }
}
