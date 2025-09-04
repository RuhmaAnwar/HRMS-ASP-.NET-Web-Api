using AutoMapper;
using HRMS.Data;
using HRMS.Dtos.RequestDtos;
using HRMS.Dtos.ResponseDtos;
using HRMS.Middleware;
using HRMS.Models;
using HRMS.Repositories.Interfaces;
using HRMS.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace HRMS.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly SignInManager<Employee> _signInManager;
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _context;

        public AuthService(IAuthRepository authRepository, SignInManager<Employee> signInManager, IMapper mapper, ApplicationDbContext context)
        {
            _authRepository = authRepository;
            _signInManager = signInManager;
            _mapper = mapper;
            _context = context;
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
            // Revoke all refresh tokens for the user
            var userId = _signInManager.Context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(userId) && Guid.TryParse(userId, out var employeeId))
            {
                var refreshTokens = await _context.RefreshTokens
                    .Where(rt => rt.EmployeeId == employeeId && !rt.IsRevoked)
                    .ToListAsync();
                foreach (var token in refreshTokens)
                {
                    token.IsRevoked = true;
                    token.RevokedAt = DateTime.UtcNow.AddHours(5);
                }
                await _context.SaveChangesAsync();
            }

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

        public string GenerateHashedRefreshToken()
        {
            var randomNumber = new byte[64];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                var token = Convert.ToBase64String(randomNumber);
                return token;
                //return HashToken(token);
            }
        }

        private string HashToken(string token)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(token));
            return Convert.ToBase64String(hashedBytes);
        }

        public async Task<TokenResponseDTO> CreateRefreshTokenAsync(Guid userId, string accessToken)
        {
            var hashedRefreshToken = GenerateHashedRefreshToken();
            var token = await _authRepository.CreateRefreshTokenAsync(userId, hashedRefreshToken);
            var response = _mapper.Map<TokenResponseDTO>(token);
            response.Token = accessToken;
            return response;
        }
    }
}