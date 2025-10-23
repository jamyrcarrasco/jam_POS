using jam_POS.API.DTOs;

namespace jam_POS.API.Services
{
    public interface IVentaService
    {
        Task<IEnumerable<VentaSummaryResponse>> GetVentasAsync(int page, int pageSize);
        Task<VentaResponse?> GetVentaByIdAsync(int id);
        Task<VentaResponse> CreateVentaAsync(CreateVentaRequest request);
        Task<bool> CancelarVentaAsync(int id, string motivo);
        Task<IEnumerable<VentaSummaryResponse>> GetVentasByUsuarioAsync(int usuarioId, int page, int pageSize);
        Task<IEnumerable<VentaSummaryResponse>> GetVentasByFechaAsync(DateTime fechaInicio, DateTime fechaFin, int page, int pageSize);
        Task<decimal> GetTotalVentasHoyAsync();
        Task<int> GetCantidadVentasHoyAsync();
    }
}
