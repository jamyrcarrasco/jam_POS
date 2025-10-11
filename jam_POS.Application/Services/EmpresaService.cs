using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using jam_POS.Infrastructure.Data;
using jam_POS.Infrastructure.Services;
using jam_POS.Core.Entities;
using jam_POS.Application.DTOs.Requests;
using jam_POS.Application.DTOs.Responses;

namespace jam_POS.Application.Services
{
    public class EmpresaService : IEmpresaService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<EmpresaService> _logger;
        private readonly IEmailService _emailService;

        public EmpresaService(
            ApplicationDbContext context, 
            ILogger<EmpresaService> logger,
            IEmailService emailService)
        {
            _context = context;
            _logger = logger;
            _emailService = emailService;
        }

        public async Task<EmpresaResponse> RegisterEmpresaAsync(RegisterEmpresaRequest request)
        {
            _logger.LogInformation("Registrando nueva empresa: {Nombre}", request.NombreEmpresa);

            // Validar que el RNC no exista (solo si se proporcionó)
            if (!string.IsNullOrWhiteSpace(request.RNC) && await RNCExistsAsync(request.RNC))
            {
                throw new InvalidOperationException($"Ya existe una empresa registrada con el RNC {request.RNC}");
            }

            // Validar que el username no exista
            var usernameExists = await _context.Users.AnyAsync(u => u.Username.ToLower() == request.AdminUsername.ToLower());
            if (usernameExists)
            {
                throw new InvalidOperationException($"El nombre de usuario '{request.AdminUsername}' ya está en uso");
            }

            // Validar que el email no exista
            var emailExists = await _context.Users.AnyAsync(u => u.Email.ToLower() == request.AdminEmail.ToLower());
            if (emailExists)
            {
                throw new InvalidOperationException($"El email '{request.AdminEmail}' ya está en uso");
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            
            try
            {
                // Crear empresa
                var empresa = new Empresa
                {
                    Nombre = request.NombreEmpresa,
                    NombreComercial = request.NombreComercial,
                    RNC = request.RNC,
                    Email = request.EmailEmpresa,
                    Telefono = request.Telefono,
                    Plan = "BASICO",
                    FechaVencimientoPlan = DateTime.UtcNow.AddDays(30), // 30 días de prueba
                    Activo = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.Set<Empresa>().Add(empresa);
                await _context.SaveChangesAsync();

                // Crear usuario administrador de la empresa
                // Por defecto, el primer usuario será SuperAdmin de su empresa
                var adminUser = new User
                {
                    Username = request.AdminUsername,
                    Email = request.AdminEmail,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.AdminPassword),
                    FirstName = request.AdminFirstName,
                    LastName = request.AdminLastName,
                    RoleId = 1, // SuperAdmin
                    EmpresaId = empresa.Id,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Users.Add(adminUser);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                _logger.LogInformation("Empresa registrada exitosamente con ID: {Id}", empresa.Id);

                // Enviar email de bienvenida de forma asíncrona (no bloquear el registro)
                _ = Task.Run(async () =>
                {
                    try
                    {
                        await _emailService.SendWelcomeEmailAsync(
                            request.AdminEmail,
                            request.NombreEmpresa,
                            $"{request.AdminFirstName} {request.AdminLastName}",
                            request.AdminUsername
                        );
                        _logger.LogInformation("Email de bienvenida enviado a: {Email}", request.AdminEmail);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error al enviar email de bienvenida");
                        // No fallar el registro si el email falla
                    }
                });

                return await GetEmpresaByIdAsync(empresa.Id) ?? throw new Exception("Error al recuperar la empresa creada");
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<EmpresaResponse?> GetEmpresaByIdAsync(int id)
        {
            var empresa = await _context.Set<Empresa>()
                .Include(e => e.Users)
                .Include(e => e.Productos)
                .FirstOrDefaultAsync(e => e.Id == id);
            
            if (empresa == null) return null;

            return new EmpresaResponse
            {
                Id = empresa.Id,
                Nombre = empresa.Nombre,
                NombreComercial = empresa.NombreComercial,
                RNC = empresa.RNC,
                Direccion = empresa.Direccion,
                Telefono = empresa.Telefono,
                Email = empresa.Email,
                LogoUrl = empresa.LogoUrl,
                Pais = empresa.Pais,
                Ciudad = empresa.Ciudad,
                CodigoPostal = empresa.CodigoPostal,
                Plan = empresa.Plan,
                FechaVencimientoPlan = empresa.FechaVencimientoPlan,
                Activo = empresa.Activo,
                CreatedAt = empresa.CreatedAt,
                UsersCount = empresa.Users?.Count ?? 0,
                ProductsCount = empresa.Productos?.Count ?? 0
            };
        }

        public async Task<bool> RNCExistsAsync(string rnc)
        {
            if (string.IsNullOrWhiteSpace(rnc)) return false;
            return await _context.Set<Empresa>().AnyAsync(e => e.RNC != null && e.RNC.ToLower() == rnc.ToLower());
        }
    }
}

