using jam_POS.Application.DTOs.Requests;
using jam_POS.Application.DTOs.Responses;

namespace jam_POS.Application.Services
{
    public interface IRoleService
    {
        Task<IEnumerable<RoleResponse>> GetAllRolesAsync(bool includePermissions = false);
        Task<RoleResponse?> GetRoleByIdAsync(int id, bool includePermissions = true);
        Task<RoleResponse> CreateRoleAsync(CreateRoleRequest request);
        Task<RoleResponse> UpdateRoleAsync(int id, UpdateRoleRequest request);
        Task<bool> DeleteRoleAsync(int id);
        Task<bool> RoleExistsAsync(int id);
        Task<IEnumerable<RoleResponse>> GetActiveRolesAsync();
    }
}

