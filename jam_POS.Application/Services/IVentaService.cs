using jam_POS.Application.DTOs.Requests;
using jam_POS.Application.DTOs.Responses;

namespace jam_POS.Application.Services
{
    public interface IVentaService
    {
        Task<IEnumerable<VentaSummaryResponse>> GetVentasAsync(int page = 1, int pageSize = 10);
        Task<VentaResponse?> GetVentaByIdAsync(int id);
        Task<VentaResponse> CreateVentaAsync(CreateVentaRequest request);
        Task<bool> CancelarVentaAsync(int id, string motivo);
        Task<IEnumerable<VentaSummaryResponse>> GetVentasByUsuarioAsync(int usuarioId, int page = 1, int pageSize = 10);
        Task<IEnumerable<VentaSummaryResponse>> GetVentasByFechaAsync(DateTime fechaInicio, DateTime fechaFin, int page = 1, int pageSize = 10);
        Task<decimal> GetTotalVentasHoyAsync();
        Task<int> GetCantidadVentasHoyAsync();
    }
}

