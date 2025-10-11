using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using jam_POS.Infrastructure.Data;
using jam_POS.Core.Entities;
using jam_POS.Application.DTOs.Requests;
using jam_POS.Application.DTOs.Responses;

namespace jam_POS.Application.Services
{
    public class ConfiguracionPOSService : IConfiguracionPOSService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ConfiguracionPOSService> _logger;

        public ConfiguracionPOSService(ApplicationDbContext context, ILogger<ConfiguracionPOSService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ConfiguracionPOSResponse?> GetConfiguracionAsync()
        {
            _logger.LogInformation("Recuperando configuración POS");

            var configuracion = await _context.Set<ConfiguracionPOS>()
                .Include(c => c.ImpuestoPorDefecto)
                .FirstOrDefaultAsync();

            if (configuracion == null)
            {
                _logger.LogInformation("No se encontró configuración POS, se usará la configuración por defecto");
                return null;
            }

            return MapToResponse(configuracion);
        }

        public async Task<ConfiguracionPOSResponse> CreateConfiguracionAsync(CreateConfiguracionPOSRequest request)
        {
            _logger.LogInformation("Creando nueva configuración POS");

            // Verificar si ya existe una configuración
            var existingConfig = await _context.Set<ConfiguracionPOS>().FirstOrDefaultAsync();
            if (existingConfig != null)
            {
                throw new InvalidOperationException("Ya existe una configuración POS. Use la función de actualización.");
            }

            var configuracion = new ConfiguracionPOS
            {
                PrefijoRecibo = request.PrefijoRecibo,
                PrefijoFactura = request.PrefijoFactura,
                SiguienteNumeroRecibo = request.SiguienteNumeroRecibo,
                SiguienteNumeroFactura = request.SiguienteNumeroFactura,
                MensajePieRecibo = request.MensajePieRecibo,
                IncluirLogoRecibo = request.IncluirLogoRecibo,
                ImpuestoPorDefectoId = request.ImpuestoPorDefectoId,
                PermitirDescuentos = request.PermitirDescuentos,
                PermitirDevoluciones = request.PermitirDevoluciones,
                TiempoLimiteAnulacionMinutos = request.TiempoLimiteAnulacionMinutos,
                DescuentoMaximoPorcentaje = request.DescuentoMaximoPorcentaje,
                MonedaPorDefecto = request.MonedaPorDefecto,
                SimboloMoneda = request.SimboloMoneda,
                DecimalesMoneda = request.DecimalesMoneda,
                FormatoPapel = request.FormatoPapel,
                ImprimirAutomaticamente = request.ImprimirAutomaticamente,
                ImprimirCopiaCliente = request.ImprimirCopiaCliente,
                EfectivoHabilitado = request.EfectivoHabilitado,
                TarjetaHabilitado = request.TarjetaHabilitado,
                TransferenciaHabilitado = request.TransferenciaHabilitado,
                CreditoHabilitado = request.CreditoHabilitado,
                ModoOfflineHabilitado = request.ModoOfflineHabilitado,
                IntervaloSincronizacionMinutos = request.IntervaloSincronizacionMinutos,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
                // EmpresaId se asigna automáticamente en SaveChanges
            };

            _context.Set<ConfiguracionPOS>().Add(configuracion);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Configuración POS creada exitosamente con ID: {Id}", configuracion.Id);

            return await GetConfiguracionAsync() ?? throw new InvalidOperationException("Error al recuperar la configuración creada");
        }

        public async Task<ConfiguracionPOSResponse> UpdateConfiguracionAsync(int id, UpdateConfiguracionPOSRequest request)
        {
            _logger.LogInformation("Actualizando configuración POS con ID: {Id}", id);

            var configuracion = await _context.Set<ConfiguracionPOS>()
                .Include(c => c.ImpuestoPorDefecto)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (configuracion == null)
            {
                _logger.LogWarning("Configuración POS no encontrada para actualización con ID: {Id}", id);
                throw new KeyNotFoundException($"Configuración POS con ID {id} no encontrada");
            }

            configuracion.PrefijoRecibo = request.PrefijoRecibo;
            configuracion.PrefijoFactura = request.PrefijoFactura;
            configuracion.SiguienteNumeroRecibo = request.SiguienteNumeroRecibo;
            configuracion.SiguienteNumeroFactura = request.SiguienteNumeroFactura;
            configuracion.MensajePieRecibo = request.MensajePieRecibo;
            configuracion.IncluirLogoRecibo = request.IncluirLogoRecibo;
            configuracion.ImpuestoPorDefectoId = request.ImpuestoPorDefectoId;
            configuracion.PermitirDescuentos = request.PermitirDescuentos;
            configuracion.PermitirDevoluciones = request.PermitirDevoluciones;
            configuracion.TiempoLimiteAnulacionMinutos = request.TiempoLimiteAnulacionMinutos;
            configuracion.DescuentoMaximoPorcentaje = request.DescuentoMaximoPorcentaje;
            configuracion.MonedaPorDefecto = request.MonedaPorDefecto;
            configuracion.SimboloMoneda = request.SimboloMoneda;
            configuracion.DecimalesMoneda = request.DecimalesMoneda;
            configuracion.FormatoPapel = request.FormatoPapel;
            configuracion.ImprimirAutomaticamente = request.ImprimirAutomaticamente;
            configuracion.ImprimirCopiaCliente = request.ImprimirCopiaCliente;
            configuracion.EfectivoHabilitado = request.EfectivoHabilitado;
            configuracion.TarjetaHabilitado = request.TarjetaHabilitado;
            configuracion.TransferenciaHabilitado = request.TransferenciaHabilitado;
            configuracion.CreditoHabilitado = request.CreditoHabilitado;
            configuracion.ModoOfflineHabilitado = request.ModoOfflineHabilitado;
            configuracion.IntervaloSincronizacionMinutos = request.IntervaloSincronizacionMinutos;
            configuracion.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Configuración POS actualizada exitosamente con ID: {Id}", id);

            return MapToResponse(configuracion);
        }

        public async Task<bool> DeleteConfiguracionAsync(int id)
        {
            _logger.LogInformation("Eliminando configuración POS con ID: {Id}", id);

            var configuracion = await _context.Set<ConfiguracionPOS>().FindAsync(id);
            
            if (configuracion == null)
            {
                _logger.LogWarning("Configuración POS no encontrada para eliminación con ID: {Id}", id);
                return false;
            }

            _context.Set<ConfiguracionPOS>().Remove(configuracion);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Configuración POS eliminada exitosamente con ID: {Id}", id);

            return true;
        }

        public async Task<string> GetSiguienteNumeroReciboAsync()
        {
            var configuracion = await GetConfiguracionAsync();
            if (configuracion == null)
            {
                return "REC-000001";
            }

            var siguienteNumero = configuracion.SiguienteNumeroRecibo;
            var numeroFormateado = siguienteNumero.ToString("D6");
            
            // Incrementar el número para la próxima vez
            await IncrementarNumeroReciboAsync(configuracion.Id);
            
            return $"{configuracion.PrefijoRecibo}-{numeroFormateado}";
        }

        public async Task<string> GetSiguienteNumeroFacturaAsync()
        {
            var configuracion = await GetConfiguracionAsync();
            if (configuracion == null)
            {
                return "FAC-000001";
            }

            var siguienteNumero = configuracion.SiguienteNumeroFactura;
            var numeroFormateado = siguienteNumero.ToString("D6");
            
            // Incrementar el número para la próxima vez
            await IncrementarNumeroFacturaAsync(configuracion.Id);
            
            return $"{configuracion.PrefijoFactura}-{numeroFormateado}";
        }

        private async Task IncrementarNumeroReciboAsync(int configId)
        {
            var configuracion = await _context.Set<ConfiguracionPOS>().FindAsync(configId);
            if (configuracion != null)
            {
                configuracion.SiguienteNumeroRecibo++;
                configuracion.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        private async Task IncrementarNumeroFacturaAsync(int configId)
        {
            var configuracion = await _context.Set<ConfiguracionPOS>().FindAsync(configId);
            if (configuracion != null)
            {
                configuracion.SiguienteNumeroFactura++;
                configuracion.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        private static ConfiguracionPOSResponse MapToResponse(ConfiguracionPOS configuracion)
        {
            return new ConfiguracionPOSResponse
            {
                Id = configuracion.Id,
                PrefijoRecibo = configuracion.PrefijoRecibo,
                PrefijoFactura = configuracion.PrefijoFactura,
                SiguienteNumeroRecibo = configuracion.SiguienteNumeroRecibo,
                SiguienteNumeroFactura = configuracion.SiguienteNumeroFactura,
                MensajePieRecibo = configuracion.MensajePieRecibo,
                IncluirLogoRecibo = configuracion.IncluirLogoRecibo,
                ImpuestoPorDefectoId = configuracion.ImpuestoPorDefectoId,
                ImpuestoPorDefectoNombre = configuracion.ImpuestoPorDefecto?.Nombre,
                PermitirDescuentos = configuracion.PermitirDescuentos,
                PermitirDevoluciones = configuracion.PermitirDevoluciones,
                TiempoLimiteAnulacionMinutos = configuracion.TiempoLimiteAnulacionMinutos,
                DescuentoMaximoPorcentaje = configuracion.DescuentoMaximoPorcentaje,
                MonedaPorDefecto = configuracion.MonedaPorDefecto,
                SimboloMoneda = configuracion.SimboloMoneda,
                DecimalesMoneda = configuracion.DecimalesMoneda,
                FormatoPapel = configuracion.FormatoPapel,
                ImprimirAutomaticamente = configuracion.ImprimirAutomaticamente,
                ImprimirCopiaCliente = configuracion.ImprimirCopiaCliente,
                EfectivoHabilitado = configuracion.EfectivoHabilitado,
                TarjetaHabilitado = configuracion.TarjetaHabilitado,
                TransferenciaHabilitado = configuracion.TransferenciaHabilitado,
                CreditoHabilitado = configuracion.CreditoHabilitado,
                ModoOfflineHabilitado = configuracion.ModoOfflineHabilitado,
                IntervaloSincronizacionMinutos = configuracion.IntervaloSincronizacionMinutos,
                CreatedAt = configuracion.CreatedAt,
                UpdatedAt = configuracion.UpdatedAt
            };
        }
    }
}

