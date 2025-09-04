using HRMS.Models;
using Microsoft.AspNetCore.Identity;

namespace HRMS.Repositories.Interfaces
{
    public interface IAuthRepository
    {
        Task<Employee> FindByEmailAsync(string email);
        Task<Employee> FindByIdAsync(string userId);
        Task<IdentityResult> ResetPasswordAsync(Employee user, string token, string newPassword);
        Task<string> GeneratePasswordResetTokenAsync(Employee user);
        Task<IdentityResult> AddToRolesAsync(Employee user, string[] roles);
        Task<IdentityResult> RemoveFromRolesAsync(Employee user, string[] roles);
        Task<string[]> GetRolesAsync(Employee user);
        Task<string> GenerateJwtToken(Employee user);
        Task<RefreshToken> CreateRefreshTokenAsync(Guid userId, string hashedRefreshToken);
    }
}
