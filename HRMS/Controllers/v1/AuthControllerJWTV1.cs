using AutoMapper;
using HRMS.Dtos.RequestDtos;
using HRMS.Dtos.ResponseDtos;
using HRMS.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using HRMS.Models;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using HRMS.Data;

namespace HRMS.Controllers.v1
{
    [ApiController]
    [Route("api/v1/auth")]
    public class AuthControllerJWTV1 : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IEmployeeService _employeeService;
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _context;

        public AuthControllerJWTV1(
            IAuthService authService,
            IEmployeeService employeeService,
            IMapper mapper,
            ApplicationDbContext context)
        {
            _authService = authService;
            _employeeService = employeeService;
            _mapper = mapper;
            _context = context;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
        {
            var response = await _authService.LoginJWT(dto);
            string token = await _authService.GenerateJwtToken(response);
            var refreshToken = await _authService.CreateRefreshTokenAsync(response.Id, token);
            return Ok(refreshToken);
        }

        [HttpPost("refresh-token")]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDTO dto)
        {
            if (dto == null || string.IsNullOrEmpty(dto.RefreshToken))
            {
                return BadRequest("Refresh token is required.");
            }

            // Find the refresh token in the database
            var refreshToken = await _context.RefreshTokens
                .Include(rt => rt.Employee)
                .FirstOrDefaultAsync(rt => rt.Token == dto.RefreshToken);

            if (refreshToken == null)
            {
                return BadRequest("Invalid refresh token.");
            }

            // Check if the token is expired or revoked
            if (refreshToken.IsRevoked || refreshToken.ExpiresAt < DateTime.UtcNow)
            {
                return BadRequest("Refresh token is expired or revoked.");
            }

            // Generate a new JWT for the user
            var employee = refreshToken.Employee;
            var newAccessToken = await _authService.GenerateJwtToken(employee);

            // Optionally generate a new refresh token (token rotation)
            var newRefreshToken = await _authService.CreateRefreshTokenAsync(employee.Id, newAccessToken);

            // Revoke the old refresh token
            refreshToken.IsRevoked = true;
            refreshToken.RevokedAt = DateTime.UtcNow.AddHours(5);
            await _context.SaveChangesAsync();

            return Ok(new TokenResponseDTO
            {
                Token = newAccessToken,
                RefreshToken = newRefreshToken.RefreshToken
            });
        }

        [HttpGet("protected-endpoint")]
        [Authorize]
        public IActionResult ProtectedEndpoint()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Ok($"Hello, user {userId}! This is a protected endpoint.");
        }
    }
}