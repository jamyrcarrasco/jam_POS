using jam_POS.Application.DTOs.Requests;
using jam_POS.Application.DTOs.Responses;

namespace jam_POS.Application.Services
{
    public interface IEmpresaService
    {
        Task<EmpresaResponse> RegisterEmpresaAsync(RegisterEmpresaRequest request);
        Task<EmpresaResponse?> GetEmpresaByIdAsync(int id);
        Task<bool> RUCExistsAsync(string ruc);
    }
}

