using jam_POS.API.DTOs;
using jam_POS.Application.Services;
using jam_POS.Application.DTOs.Requests;
using jam_POS.Application.DTOs.Responses;

namespace jam_POS.API.Services
{
    public class VentaService : IVentaService
    {
        private readonly ILogger<VentaService> _logger;
        private readonly jam_POS.Application.Services.IVentaService _appVentaService;

        public VentaService(
            ILogger<VentaService> logger,
            jam_POS.Application.Services.IVentaService appVentaService)
        {
            _logger = logger;
            _appVentaService = appVentaService;
        }

        public async Task<IEnumerable<jam_POS.API.DTOs.VentaSummaryResponse>> GetVentasAsync(int page, int pageSize)
        {
            var appVentas = await _appVentaService.GetVentasAsync(page, pageSize);
            return appVentas.Select(MapToApiSummary);
        }

        public async Task<jam_POS.API.DTOs.VentaResponse?> GetVentaByIdAsync(int id)
        {
            var appVenta = await _appVentaService.GetVentaByIdAsync(id);
            return appVenta != null ? MapToApiResponse(appVenta) : null;
        }

        public async Task<jam_POS.API.DTOs.VentaResponse> CreateVentaAsync(jam_POS.API.DTOs.CreateVentaRequest request)
        {
            var appRequest = MapToAppRequest(request);
            var appResponse = await _appVentaService.CreateVentaAsync(appRequest);
            return MapToApiResponse(appResponse);
        }

        public async Task<bool> CancelarVentaAsync(int id, string motivo)
        {
            return await _appVentaService.CancelarVentaAsync(id, motivo);
        }

        public async Task<IEnumerable<jam_POS.API.DTOs.VentaSummaryResponse>> GetVentasByUsuarioAsync(int usuarioId, int page, int pageSize)
        {
            var appVentas = await _appVentaService.GetVentasByUsuarioAsync(usuarioId, page, pageSize);
            return appVentas.Select(MapToApiSummary);
        }

        public async Task<IEnumerable<jam_POS.API.DTOs.VentaSummaryResponse>> GetVentasByFechaAsync(DateTime fechaInicio, DateTime fechaFin, int page, int pageSize)
        {
            var appVentas = await _appVentaService.GetVentasByFechaAsync(fechaInicio, fechaFin, page, pageSize);
            return appVentas.Select(MapToApiSummary);
        }

        public async Task<decimal> GetTotalVentasHoyAsync()
        {
            return await _appVentaService.GetTotalVentasHoyAsync();
        }

        public async Task<int> GetCantidadVentasHoyAsync()
        {
            return await _appVentaService.GetCantidadVentasHoyAsync();
        }

        private static jam_POS.API.DTOs.VentaSummaryResponse MapToApiSummary(jam_POS.Application.DTOs.Responses.VentaSummaryResponse appVenta)
        {
            return new jam_POS.API.DTOs.VentaSummaryResponse
            {
                Id = appVenta.Id,
                NumeroVenta = appVenta.NumeroVenta,
                Fecha = appVenta.FechaVenta,
                Total = appVenta.Total,
                Estado = appVenta.Estado,
                UsuarioNombre = appVenta.UsuarioNombre
            };
        }

        private static jam_POS.API.DTOs.VentaResponse MapToApiResponse(jam_POS.Application.DTOs.Responses.VentaResponse appVenta)
        {
            return new jam_POS.API.DTOs.VentaResponse
            {
                Id = appVenta.Id,
                NumeroVenta = appVenta.NumeroVenta,
                Fecha = appVenta.FechaVenta,
                Total = appVenta.Total,
                Descuento = appVenta.TotalDescuentos,
                Impuesto = appVenta.TotalImpuestos,
                Estado = appVenta.Estado,
                Observaciones = appVenta.Notas,
                UsuarioId = appVenta.UsuarioId,
                UsuarioNombre = appVenta.UsuarioNombre,
                Items = appVenta.Items.Select(MapItemToApi).ToList()
            };
        }

        private static jam_POS.API.DTOs.VentaItemResponse MapItemToApi(jam_POS.Application.DTOs.Responses.VentaItemResponse appItem)
        {
            return new jam_POS.API.DTOs.VentaItemResponse
            {
                Id = appItem.Id,
                ProductoId = appItem.ProductoId,
                ProductoNombre = appItem.ProductoNombre,
                Cantidad = (int)appItem.Cantidad,
                PrecioUnitario = appItem.PrecioUnitario,
                DescuentoPorcentaje = appItem.DescuentoPorcentaje,
                DescuentoMonto = appItem.DescuentoMonto,
                TotalImpuestos = appItem.TotalImpuestos,
                Total = appItem.Total,
                Notas = appItem.Notas
            };
        }

        private static jam_POS.Application.DTOs.Requests.CreateVentaRequest MapToAppRequest(jam_POS.API.DTOs.CreateVentaRequest apiRequest)
        {
            return new jam_POS.Application.DTOs.Requests.CreateVentaRequest
            {
                Notas = apiRequest.Observaciones,
                ClienteId = apiRequest.ClienteId,
                Items = apiRequest.Items.Select(MapItemToApp).ToList(),
                Pagos = apiRequest.Pagos.Select(MapPagoToApp).ToList()
            };
        }

        private static jam_POS.Application.DTOs.Requests.CreatePagoRequest MapPagoToApp(jam_POS.API.DTOs.CreatePagoRequest apiPago)
        {
            return new jam_POS.Application.DTOs.Requests.CreatePagoRequest
            {
                MetodoPago = apiPago.MetodoPago,
                Monto = apiPago.Monto,
                MontoRecibido = apiPago.MontoRecibido,
                CambioDevolver = apiPago.CambioDevolver,
                Referencia = apiPago.Referencia,
                Banco = apiPago.Banco,
                TipoTarjeta = apiPago.TipoTarjeta,
                Notas = apiPago.Notas
            };
        }

        private static jam_POS.Application.DTOs.Requests.CreateVentaItemRequest MapItemToApp(jam_POS.API.DTOs.CreateVentaItemRequest apiItem)
        {
            return new jam_POS.Application.DTOs.Requests.CreateVentaItemRequest
            {
                ProductoId = apiItem.ProductoId,
                Cantidad = (decimal)apiItem.Cantidad,
                PrecioUnitario = apiItem.PrecioUnitario,
                DescuentoPorcentaje = apiItem.DescuentoPorcentaje,
                DescuentoMonto = apiItem.DescuentoMonto,
                TotalImpuestos = apiItem.TotalImpuestos,
                Notas = apiItem.Notas
            };
        }
    }
}
