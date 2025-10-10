using jam_POS.Application.DTOs.Requests;
using jam_POS.Application.DTOs.Responses;
using jam_POS.Application.DTOs.Common;

namespace jam_POS.Application.Services
{
    public interface IUserService
    {
        Task<PagedResult<UserResponse>> GetAllUsersAsync(UserFilterRequest filter);
        Task<UserResponse?> GetUserByIdAsync(int id);
        Task<UserResponse> CreateUserAsync(CreateUserRequest request);
        Task<UserResponse> UpdateUserAsync(int id, UpdateUserRequest request);
        Task<bool> DeleteUserAsync(int id);
        Task<bool> UserExistsAsync(int id);
        Task<bool> UsernameExistsAsync(string username, int? excludeUserId = null);
        Task<bool> EmailExistsAsync(string email, int? excludeUserId = null);
    }
}

