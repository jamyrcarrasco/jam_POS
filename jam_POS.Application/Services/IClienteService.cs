using jam_POS.Application.DTOs.Common;
using jam_POS.Application.DTOs.Requests;
using jam_POS.Application.DTOs.Responses;

namespace jam_POS.Application.Services
{
    public interface IClienteService
    {
        Task<PagedResult<ClienteResponse>> GetClientesAsync(ClienteFilterRequest filter);
        Task<ClienteResponse?> GetClienteByIdAsync(int id);
        Task<IEnumerable<ClienteResponse>> GetClientesActivosAsync();
        Task<ClienteResponse> CreateClienteAsync(CreateClienteRequest request);
        Task<ClienteResponse> UpdateClienteAsync(int id, UpdateClienteRequest request);
        Task<bool> DeleteClienteAsync(int id);
    }
}
