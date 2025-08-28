
using HRMS.Dtos.RequestDtos;
using HRMS.Dtos.ResponseDtos;
using Microsoft.AspNetCore.Identity;

namespace HRMS.Services.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto> LoginAsync(LoginRequestDto dto);
        Task LogoutAsync();
        Task<AuthResponseDto> AssignRolesAsync(Guid userId, RoleAssignmentRequestDto dto);
        Task<AuthResponseDto> GetRolesAsync(Guid userId);
        Task<AuthResponseDto> GetCurrentUserAsync(string userId);
        Task<IdentityResult> ForgotPasswordAsync(ForgotPasswordRequestDTO dto);
    }
}
