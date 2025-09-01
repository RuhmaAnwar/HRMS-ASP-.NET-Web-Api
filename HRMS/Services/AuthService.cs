using HRMS.Dtos.RequestDtos;
using HRMS.Dtos.ResponseDtos;
using HRMS.Middleware;
using HRMS.Models;
using HRMS.Repositories.Interfaces;
using HRMS.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace HRMS.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly SignInManager<Employee> _signInManager;

        public AuthService(IAuthRepository authRepository, SignInManager<Employee> signInManager)
        {
            _authRepository = authRepository;
            _signInManager = signInManager;
        }

        public async Task<AuthResponseDto> LoginAsync(LoginRequestDto dto)
        {
            var employee = await _authRepository.FindByEmailAsync(dto.Email);
            if (employee == null)
                throw new ValidationException("Invalid email or password.");

            var result = await _signInManager.PasswordSignInAsync(
                dto.Email,
                dto.Password,
                isPersistent: true,
                lockoutOnFailure: false
            );

            if (!result.Succeeded)
                throw new ValidationException("Invalid email or password.");

            var roles = await _authRepository.GetRolesAsync(employee);
            return new AuthResponseDto
            {
                Id = employee.Id,
                Email = employee.Email,
                Roles = roles
            };
        }

        public async Task<Employee> LoginJWT(LoginRequestDto dto)
        {
            var employee = await _authRepository.FindByEmailAsync(dto.Email);
            if (employee == null)
                throw new ValidationException("Invalid email or password.");

            var result = await _signInManager.PasswordSignInAsync(
                dto.Email,
                dto.Password,
                isPersistent: true,
                lockoutOnFailure: false
            );

            if (!result.Succeeded)
                throw new ValidationException("Invalid email or password.");

            return employee;
        }

        public async Task<string> GenerateJwtToken(Employee user)
        {
            return await _authRepository.GenerateJwtToken(user);
        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<AuthResponseDto> AssignRolesAsync(Guid userId, RoleAssignmentRequestDto dto)
        {
            var employee = await _authRepository.FindByIdAsync(userId.ToString());
            if (employee == null)
                throw new KeyNotFoundException($"Employee with ID {userId} not found.");

            var currentRoles = await _authRepository.GetRolesAsync(employee);
            await _authRepository.RemoveFromRolesAsync(employee, currentRoles);
            await _authRepository.AddToRolesAsync(employee, dto.Roles);

            var updatedRoles = await _authRepository.GetRolesAsync(employee);
            return new AuthResponseDto
            {
                Id = employee.Id,
                Email = employee.Email,
                Roles = updatedRoles
            };
        }

        public async Task<AuthResponseDto> GetRolesAsync(Guid userId)
        {
            var employee = await _authRepository.FindByIdAsync(userId.ToString());
            if (employee == null)
                throw new KeyNotFoundException($"Employee with ID {userId} not found.");

            var roles = await _authRepository.GetRolesAsync(employee);
            return new AuthResponseDto
            {
                Id = employee.Id,
                Email = employee.Email,
                Roles = roles
            };
        }

        public async Task<AuthResponseDto> GetCurrentUserAsync(string userId)
        {
            var employee = await _authRepository.FindByIdAsync(userId);
            if (employee == null)
                throw new KeyNotFoundException($"Employee with ID {userId} not found.");

            var roles = await _authRepository.GetRolesAsync(employee);
            return new AuthResponseDto
            {
                Id = employee.Id,
                Email = employee.Email,
                Roles = roles
            };
        }

        public async Task<IdentityResult> ForgotPasswordAsync(ForgotPasswordRequestDTO dto)
        {
            var user = await _authRepository.FindByEmailAsync(dto.Email);
            if (user == null)
                throw new KeyNotFoundException($"User with email {dto.Email} does not exist.");

            var token = await _authRepository.GeneratePasswordResetTokenAsync(user);
            return await _authRepository.ResetPasswordAsync(user, token, dto.newPassword);
        }
    }
}
