using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using jam_POS.Infrastructure.Data;
using jam_POS.Core.Entities;
using jam_POS.Application.DTOs.Requests;
using jam_POS.Application.DTOs.Responses;

namespace jam_POS.Application.Services
{
    public class VentaService : IVentaService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<VentaService> _logger;
        private readonly IConfiguracionPOSService _configuracionPOSService;

        public VentaService(
            ApplicationDbContext context, 
            ILogger<VentaService> logger,
            IConfiguracionPOSService configuracionPOSService)
        {
            _context = context;
            _logger = logger;
            _configuracionPOSService = configuracionPOSService;
        }

        public async Task<IEnumerable<VentaSummaryResponse>> GetVentasAsync(int page = 1, int pageSize = 10)
        {
            _logger.LogInformation("Recuperando ventas - Página {Page}, Tamaño {PageSize}", page, pageSize);

            var ventas = await _context.Set<Venta>()
                .Include(v => v.Usuario)
                .Include(v => v.VentaItems)
                .OrderByDescending(v => v.FechaVenta)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return ventas.Select(MapToSummaryResponse);
        }

        public async Task<VentaResponse?> GetVentaByIdAsync(int id)
        {
            _logger.LogInformation("Recuperando venta con ID: {Id}", id);

            var venta = await _context.Set<Venta>()
                .Include(v => v.Usuario)
                .Include(v => v.Cliente)
                .Include(v => v.VentaItems)
                    .ThenInclude(vi => vi.Producto)
                .Include(v => v.Pagos)
                .FirstOrDefaultAsync(v => v.Id == id);

            if (venta == null)
            {
                _logger.LogWarning("Venta no encontrada con ID: {Id}", id);
                return null;
            }

            return MapToResponse(venta);
        }

        public async Task<VentaResponse> CreateVentaAsync(CreateVentaRequest request)
        {
            _logger.LogInformation("Creando nueva venta");

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Generar número de venta
                var numeroVenta = await GenerarNumeroVentaAsync();

                if (request.ClienteId.HasValue)
                {
                    var clienteExiste = await _context.Set<Cliente>()
                        .AnyAsync(c => c.Id == request.ClienteId.Value);

                    if (!clienteExiste)
                    {
                        throw new ArgumentException($"Cliente con ID {request.ClienteId.Value} no encontrado");
                    }
                }

                // Crear la venta
                var venta = new Venta
                {
                    NumeroVenta = numeroVenta,
                    FechaVenta = DateTime.UtcNow,
                    Notas = request.Notas,
                    ClienteId = request.ClienteId,
                    UsuarioId = GetCurrentUserId(), // Implementar según tu sistema de autenticación
                    Estado = "COMPLETADA",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                    // EmpresaId se asigna automáticamente en SaveChanges
                };

                _context.Set<Venta>().Add(venta);
                await _context.SaveChangesAsync();

                // Crear items de venta
                decimal subtotal = 0;
                decimal totalImpuestos = 0;
                decimal totalDescuentos = 0;

                foreach (var itemRequest in request.Items)
                {
                    var producto = await _context.Set<Producto>().FindAsync(itemRequest.ProductoId);
                    if (producto == null)
                    {
                        throw new ArgumentException($"Producto con ID {itemRequest.ProductoId} no encontrado");
                    }

                    // Calcular subtotal del item
                    var subtotalItem = itemRequest.Cantidad * itemRequest.PrecioUnitario;
                    
                    // Validar y calcular descuentos
                    var descuentoMonto = 0m;
                    if (itemRequest.DescuentoPorcentaje > 0)
                    {
                        // Validar límite máximo de descuento
                        var configuracion = await _configuracionPOSService.GetConfiguracionAsync();
                        if (configuracion?.DescuentoMaximoPorcentaje != null && 
                            itemRequest.DescuentoPorcentaje > configuracion.DescuentoMaximoPorcentaje)
                        {
                            throw new InvalidOperationException($"El descuento máximo permitido es {configuracion.DescuentoMaximoPorcentaje}%");
                        }

                        // Validar si se permiten descuentos manuales
                        if (configuracion?.PermitirDescuentos == false)
                        {
                            throw new InvalidOperationException("Los descuentos manuales no están permitidos");
                        }

                        descuentoMonto = subtotalItem * (itemRequest.DescuentoPorcentaje / 100);
                    }
                    else if (itemRequest.DescuentoMonto > 0)
                    {
                        descuentoMonto = itemRequest.DescuentoMonto;
                    }

                    var subtotalConDescuento = subtotalItem - descuentoMonto;

                    // Aplicar impuestos (usando configuración POS)
                    var impuestosItem = await CalcularImpuestosAsync(subtotalConDescuento);

                    var totalItem = subtotalConDescuento + impuestosItem;

                    var ventaItem = new VentaItem
                    {
                        VentaId = venta.Id,
                        ProductoId = itemRequest.ProductoId,
                        ProductoNombre = producto.Nombre,
                        ProductoCodigo = producto.CodigoBarras,
                        Cantidad = itemRequest.Cantidad,
                        PrecioUnitario = itemRequest.PrecioUnitario,
                        Subtotal = subtotalItem,
                        DescuentoPorcentaje = itemRequest.DescuentoPorcentaje,
                        DescuentoMonto = descuentoMonto,
                        TotalImpuestos = impuestosItem,
                        Total = totalItem,
                        Notas = itemRequest.Notas,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                        // EmpresaId se asigna automáticamente en SaveChanges
                    };

                    _context.Set<VentaItem>().Add(ventaItem);

                    subtotal += subtotalItem;
                    totalDescuentos += descuentoMonto;
                    totalImpuestos += impuestosItem;
                }

                // Crear pagos
                decimal totalPagado = 0;
                foreach (var pagoRequest in request.Pagos)
                {
                    var pago = new Pago
                    {
                        VentaId = venta.Id,
                        MetodoPago = pagoRequest.MetodoPago,
                        Monto = pagoRequest.Monto,
                        Referencia = pagoRequest.Referencia,
                        Banco = pagoRequest.Banco,
                        TipoTarjeta = pagoRequest.TipoTarjeta,
                        FechaPago = DateTime.UtcNow,
                        Notas = pagoRequest.Notas,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                        // EmpresaId se asigna automáticamente en SaveChanges
                    };

                    _context.Set<Pago>().Add(pago);
                    totalPagado += pagoRequest.Monto;
                }

                // Actualizar totales de la venta
                venta.Subtotal = subtotal;
                venta.TotalImpuestos = totalImpuestos;
                venta.TotalDescuentos = totalDescuentos;
                venta.Total = subtotal - totalDescuentos + totalImpuestos;

                // Verificar que el total pagado coincida con el total de la venta
                if (Math.Abs(totalPagado - venta.Total) > 0.01m)
                {
                    throw new InvalidOperationException($"El total pagado (${totalPagado:F2}) no coincide con el total de la venta (${venta.Total:F2})");
                }

                // Validar métodos de pago habilitados
                await ValidarMetodosPagoAsync(request.Pagos);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation("Venta creada exitosamente con ID: {Id}, Número: {Numero}", venta.Id, venta.NumeroVenta);

                return await GetVentaByIdAsync(venta.Id) ?? throw new InvalidOperationException("Error al recuperar la venta creada");
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> CancelarVentaAsync(int id, string motivo)
        {
            _logger.LogInformation("Cancelando venta con ID: {Id}", id);

            var venta = await _context.Set<Venta>().FindAsync(id);
            
            if (venta == null)
            {
                _logger.LogWarning("Venta no encontrada para cancelación con ID: {Id}", id);
                return false;
            }

            if (venta.Estado == "CANCELADA")
            {
                _logger.LogWarning("La venta con ID {Id} ya está cancelada", id);
                return false;
            }

            // Verificar tiempo límite para cancelación
            var configuracion = await _configuracionPOSService.GetConfiguracionAsync();
            if (configuracion != null)
            {
                var tiempoLimite = configuracion.TiempoLimiteAnulacionMinutos;
                var tiempoTranscurrido = DateTime.UtcNow - venta.FechaVenta;
                
                if (tiempoTranscurrido.TotalMinutes > tiempoLimite)
                {
                    throw new InvalidOperationException($"No se puede cancelar la venta. Tiempo límite excedido ({tiempoLimite} minutos)");
                }
            }

            venta.Estado = "CANCELADA";
            venta.FechaCancelacion = DateTime.UtcNow;
            venta.MotivoCancelacion = motivo;
            venta.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Venta cancelada exitosamente con ID: {Id}", id);

            return true;
        }

        public async Task<IEnumerable<VentaSummaryResponse>> GetVentasByUsuarioAsync(int usuarioId, int page = 1, int pageSize = 10)
        {
            var ventas = await _context.Set<Venta>()
                .Include(v => v.Usuario)
                .Include(v => v.VentaItems)
                .Where(v => v.UsuarioId == usuarioId)
                .OrderByDescending(v => v.FechaVenta)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return ventas.Select(MapToSummaryResponse);
        }

        public async Task<IEnumerable<VentaSummaryResponse>> GetVentasByFechaAsync(DateTime fechaInicio, DateTime fechaFin, int page = 1, int pageSize = 10)
        {
            var ventas = await _context.Set<Venta>()
                .Include(v => v.Usuario)
                .Include(v => v.VentaItems)
                .Where(v => v.FechaVenta >= fechaInicio && v.FechaVenta <= fechaFin)
                .OrderByDescending(v => v.FechaVenta)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return ventas.Select(MapToSummaryResponse);
        }

        public async Task<decimal> GetTotalVentasHoyAsync()
        {
            var hoy = DateTime.Today;
            var mañana = hoy.AddDays(1);

            return await _context.Set<Venta>()
                .Where(v => v.FechaVenta >= hoy && v.FechaVenta < mañana && v.Estado == "COMPLETADA")
                .SumAsync(v => v.Total);
        }

        public async Task<int> GetCantidadVentasHoyAsync()
        {
            var hoy = DateTime.Today;
            var mañana = hoy.AddDays(1);

            return await _context.Set<Venta>()
                .Where(v => v.FechaVenta >= hoy && v.FechaVenta < mañana && v.Estado == "COMPLETADA")
                .CountAsync();
        }

        private async Task<string> GenerarNumeroVentaAsync()
        {
            var configuracion = await _configuracionPOSService.GetConfiguracionAsync();
            if (configuracion != null)
            {
                var numero = await _configuracionPOSService.GetSiguienteNumeroReciboAsync();
                return numero;
            }
            
            // Fallback si no hay configuración
            var ultimaVenta = await _context.Set<Venta>()
                .OrderByDescending(v => v.Id)
                .FirstOrDefaultAsync();
            
            var siguienteNumero = ultimaVenta?.Id + 1 ?? 1;
            return $"VTA-{siguienteNumero:D6}";
        }

        private async Task<decimal> CalcularImpuestosAsync(decimal subtotal)
        {
            var configuracion = await _configuracionPOSService.GetConfiguracionAsync();
            if (configuracion?.ImpuestoPorDefectoId != null)
            {
                var impuesto = await _context.Set<Impuesto>().FindAsync(configuracion.ImpuestoPorDefectoId);
                if (impuesto != null && impuesto.Activo)
                {
                    if (impuesto.Tipo == "PORCENTUAL")
                    {
                        return subtotal * (impuesto.Porcentaje / 100);
                    }
                    else if (impuesto.Tipo == "FIJO")
                    {
                        return impuesto.MontoFijo ?? 0;
                    }
                }
            }
            
            return 0;
        }

        private int GetCurrentUserId()
        {
            // TODO: Implementar obtención del usuario actual desde el contexto de autenticación
            // Por ahora retornamos 1 como placeholder
            return 1;
        }

        private async Task ValidarMetodosPagoAsync(IEnumerable<CreatePagoRequest> pagos)
        {
            var configuracion = await _configuracionPOSService.GetConfiguracionAsync();
            if (configuracion == null) return;

            foreach (var pago in pagos)
            {
                switch (pago.MetodoPago.ToUpper())
                {
                    case "EFECTIVO":
                        if (!configuracion.EfectivoHabilitado)
                            throw new InvalidOperationException("El método de pago EFECTIVO no está habilitado");
                        break;
                    case "TARJETA":
                        if (!configuracion.TarjetaHabilitado)
                            throw new InvalidOperationException("El método de pago TARJETA no está habilitado");
                        break;
                    case "TRANSFERENCIA":
                        if (!configuracion.TransferenciaHabilitado)
                            throw new InvalidOperationException("El método de pago TRANSFERENCIA no está habilitado");
                        break;
                    case "CREDITO":
                        if (!configuracion.CreditoHabilitado)
                            throw new InvalidOperationException("El método de pago CREDITO no está habilitado");
                        break;
                    default:
                        throw new InvalidOperationException($"Método de pago no válido: {pago.MetodoPago}");
                }
            }
        }

        private static VentaSummaryResponse MapToSummaryResponse(Venta venta)
        {
            return new VentaSummaryResponse
            {
                Id = venta.Id,
                NumeroVenta = venta.NumeroVenta,
                FechaVenta = venta.FechaVenta,
                Total = venta.Total,
                Estado = venta.Estado,
                UsuarioNombre = venta.Usuario?.Username ?? "N/A",
                CantidadItems = venta.VentaItems.Count
            };
        }

        private static VentaResponse MapToResponse(Venta venta)
        {
            return new VentaResponse
            {
                Id = venta.Id,
                NumeroVenta = venta.NumeroVenta,
                FechaVenta = venta.FechaVenta,
                Notas = venta.Notas,
                Subtotal = venta.Subtotal,
                TotalImpuestos = venta.TotalImpuestos,
                TotalDescuentos = venta.TotalDescuentos,
                Total = venta.Total,
                Estado = venta.Estado,
                FechaCancelacion = venta.FechaCancelacion,
                MotivoCancelacion = venta.MotivoCancelacion,
                ClienteId = venta.ClienteId,
                ClienteNombre = venta.Cliente != null
                    ? string.Join(" ", new[] { venta.Cliente.Nombre, venta.Cliente.Apellido }
                        .Where(part => !string.IsNullOrWhiteSpace(part))).Trim()
                    : null,
                UsuarioId = venta.UsuarioId,
                UsuarioNombre = venta.Usuario?.Username ?? "N/A",
                CreatedAt = venta.CreatedAt,
                UpdatedAt = venta.UpdatedAt,
                Items = venta.VentaItems.Select(MapItemToResponse).ToList(),
                Pagos = venta.Pagos.Select(MapPagoToResponse).ToList()
            };
        }

        private static VentaItemResponse MapItemToResponse(VentaItem item)
        {
            return new VentaItemResponse
            {
                Id = item.Id,
                VentaId = item.VentaId,
                ProductoId = item.ProductoId,
                ProductoNombre = item.ProductoNombre,
                ProductoCodigo = item.ProductoCodigo,
                Cantidad = item.Cantidad,
                PrecioUnitario = item.PrecioUnitario,
                Subtotal = item.Subtotal,
                DescuentoPorcentaje = item.DescuentoPorcentaje,
                DescuentoMonto = item.DescuentoMonto,
                TotalImpuestos = item.TotalImpuestos,
                Total = item.Total,
                Notas = item.Notas,
                CreatedAt = item.CreatedAt
            };
        }

        private static PagoResponse MapPagoToResponse(Pago pago)
        {
            return new PagoResponse
            {
                Id = pago.Id,
                VentaId = pago.VentaId,
                MetodoPago = pago.MetodoPago,
                Monto = pago.Monto,
                Referencia = pago.Referencia,
                Banco = pago.Banco,
                TipoTarjeta = pago.TipoTarjeta,
                FechaPago = pago.FechaPago,
                Notas = pago.Notas,
                CreatedAt = pago.CreatedAt
            };
        }
    }
}

