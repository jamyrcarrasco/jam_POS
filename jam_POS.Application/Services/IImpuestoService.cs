using jam_POS.Application.DTOs.Requests;
using jam_POS.Application.DTOs.Responses;

namespace jam_POS.Application.Services
{
    public interface IImpuestoService
    {
        Task<IEnumerable<ImpuestoResponse>> GetAllImpuestosAsync();
        Task<ImpuestoResponse?> GetImpuestoByIdAsync(int id);
        Task<ImpuestoResponse> CreateImpuestoAsync(CreateImpuestoRequest request);
        Task<ImpuestoResponse> UpdateImpuestoAsync(int id, UpdateImpuestoRequest request);
        Task<bool> DeleteImpuestoAsync(int id);
        Task<IEnumerable<ImpuestoResponse>> GetImpuestosActivosAsync();
    }
}

