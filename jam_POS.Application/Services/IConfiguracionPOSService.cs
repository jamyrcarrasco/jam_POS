using jam_POS.Application.DTOs.Requests;
using jam_POS.Application.DTOs.Responses;

namespace jam_POS.Application.Services
{
    public interface IConfiguracionPOSService
    {
        Task<ConfiguracionPOSResponse?> GetConfiguracionAsync();
        Task<ConfiguracionPOSResponse> CreateConfiguracionAsync(CreateConfiguracionPOSRequest request);
        Task<ConfiguracionPOSResponse> UpdateConfiguracionAsync(int id, UpdateConfiguracionPOSRequest request);
        Task<bool> DeleteConfiguracionAsync(int id);
        Task<string> GetSiguienteNumeroReciboAsync();
        Task<string> GetSiguienteNumeroFacturaAsync();
    }
}

