using Microsoft.AspNetCore.Identity;
using UserMgt.BLL.DTOs.Request;
using UserMgt.BLL.DTOs.Response;

namespace UserMgt.BLL.Interface
{
    public interface IAuthService
    {
        Task<IdentityResult> RegisterUserAsync(RegisterDto register);
        Task<AuthResponse> UserLogin(LoginDto loginDto);
        Task<IdentityResult> ChangePasswordAsync(ChangePasswordDto changePassword);
        Task Logout();

    }
}
