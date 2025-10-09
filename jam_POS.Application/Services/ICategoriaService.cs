using jam_POS.Application.DTOs.Requests;
using jam_POS.Application.DTOs.Responses;
using jam_POS.Application.DTOs.Common;

namespace jam_POS.Application.Services
{
    public interface ICategoriaService
    {
        Task<PagedResult<CategoriaResponse>> GetAllCategoriasAsync(CategoriaFilterRequest filter);
        Task<CategoriaResponse?> GetCategoriaByIdAsync(int id);
        Task<CategoriaResponse> CreateCategoriaAsync(CreateCategoriaRequest request);
        Task<CategoriaResponse> UpdateCategoriaAsync(int id, UpdateCategoriaRequest request);
        Task<bool> DeleteCategoriaAsync(int id);
        Task<bool> CategoriaExistsAsync(int id);
        Task<IEnumerable<CategoriaResponse>> GetCategoriasActivasAsync();
    }
}

