using jam_POS.Application.DTOs.Requests;
using jam_POS.Application.DTOs.Responses;
using jam_POS.Application.DTOs.Common;

namespace jam_POS.Application.Services
{
    public interface IProductoService
    {
        Task<PagedResult<ProductoResponse>> GetAllProductosAsync(ProductFilterRequest filter);
        Task<ProductoResponse?> GetProductoByIdAsync(int id);
        Task<ProductoResponse> CreateProductoAsync(CreateProductoRequest request);
        Task<ProductoResponse> UpdateProductoAsync(int id, UpdateProductoRequest request);
        Task<bool> DeleteProductoAsync(int id);
        Task<bool> ProductoExistsAsync(int id);
        Task<IEnumerable<string>> GetCategoriasAsync();
    }
}
