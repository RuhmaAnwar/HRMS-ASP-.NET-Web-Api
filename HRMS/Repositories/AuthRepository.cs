
using HRMS.Models;
using HRMS.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace HRMS.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly UserManager<Employee> _userManager;

        public AuthRepository(UserManager<Employee> userManager)
        {
            _userManager = userManager;
        }

        public async Task<Employee> FindByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task<Employee> FindByIdAsync(string userId)
        {
            return await _userManager.FindByIdAsync(userId);
        }

        public async Task<IdentityResult> ResetPasswordAsync(Employee user, string token, string newPassword)
        {
            return await _userManager.ResetPasswordAsync(user, token, newPassword);
        }

        public async Task<string> GeneratePasswordResetTokenAsync(Employee user)
        {
            return await _userManager.GeneratePasswordResetTokenAsync(user);
        }

        public async Task<IdentityResult> AddToRolesAsync(Employee user, string[] roles)
        {
            return await _userManager.AddToRolesAsync(user, roles);
        }

        public async Task<IdentityResult> RemoveFromRolesAsync(Employee user, string[] roles)
        {
            return await _userManager.RemoveFromRolesAsync(user, roles);
        }

        public async Task<string[]> GetRolesAsync(Employee user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            return roles.ToArray();
        }
    }
}
