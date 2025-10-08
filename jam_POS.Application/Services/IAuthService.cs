using jam_POS.Application.DTOs.Requests;
using jam_POS.Application.DTOs.Responses;

namespace jam_POS.Application.Services
{
    public interface IAuthService
    {
        Task<LoginResponse> LoginAsync(LoginRequest request);
        Task LogoutAsync();
        Task<bool> ValidateTokenAsync(string token);
    }
}
