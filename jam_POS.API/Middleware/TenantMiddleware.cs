using System.Security.Claims;
using jam_POS.Infrastructure.Services;

namespace jam_POS.API.Middleware
{
    /// <summary>
    /// Middleware que extrae el TenantId del JWT y lo establece en el TenantProvider
    /// Esto asegura que todas las consultas y operaciones estén aisladas por empresa
    /// </summary>
    public class TenantMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<TenantMiddleware> _logger;

        public TenantMiddleware(RequestDelegate next, ILogger<TenantMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, ITenantProvider tenantProvider)
        {
            // Extraer el TenantId del claim del usuario autenticado
            var tenantIdClaim = context.User?.FindFirst("TenantId")?.Value 
                             ?? context.User?.FindFirst("EmpresaId")?.Value;

            if (!string.IsNullOrEmpty(tenantIdClaim) && int.TryParse(tenantIdClaim, out var tenantId))
            {
                tenantProvider.SetTenantId(tenantId);
                _logger.LogDebug("TenantId establecido: {TenantId} para el usuario: {Username}", 
                    tenantId, context.User?.Identity?.Name);
            }
            else
            {
                // Si no hay tenant (ej: endpoints públicos), establecer null
                tenantProvider.SetTenantId(null);
                _logger.LogDebug("No se encontró TenantId en el token");
            }

            await _next(context);
        }
    }

    /// <summary>
    /// Extension method para registrar el middleware fácilmente
    /// </summary>
    public static class TenantMiddlewareExtensions
    {
        public static IApplicationBuilder UseTenantMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<TenantMiddleware>();
        }
    }
}

