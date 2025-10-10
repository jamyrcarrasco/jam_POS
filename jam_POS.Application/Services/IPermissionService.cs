using jam_POS.Application.DTOs.Responses;

namespace jam_POS.Application.Services
{
    public interface IPermissionService
    {
        Task<IEnumerable<PermissionResponse>> GetAllPermissionsAsync();
        Task<Dictionary<string, List<PermissionResponse>>> GetPermissionsByModuleAsync();
        Task<PermissionResponse?> GetPermissionByIdAsync(int id);
    }
}

