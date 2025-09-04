
//using System.Security.Claims;
//using AutoMapper;
//using HRMS.Dtos.RequestDtos;
//using HRMS.Services.Interfaces;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;

//namespace HRMS.Controllers.v1
//{
//    [ApiController]
//    [Route("api/v1/auth")]
//    public class AuthControllerV1 : ControllerBase
//    {
//        private readonly IAuthService _authService;
//        private readonly IEmployeeService _employeeService;
//        private readonly IMapper _mapper;

//        public AuthControllerV1(
//            IAuthService authService,
//            IEmployeeService employeeService,
//            IMapper mapper)
//        {
//            _authService = authService;
//            _employeeService = employeeService;
//            _mapper = mapper;
//        }

//        [HttpPost("login")]
//        [AllowAnonymous]
//        public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
//        {
//            var response = await _authService.LoginAsync(dto);
//            return Ok(response);
//        }

//        [HttpPost("logout")]
//        public async Task<IActionResult> Logout()
//        {
//            await _authService.LogoutAsync();
//            return NoContent();
//        }

//        [HttpPost("register")]
//        [Authorize(Roles = "Admin,HR")]
//        public async Task<IActionResult> Register([FromBody] RegisterRequestDto dto)
//        {
//            var employee = await _employeeService.CreateAsync(
//                _mapper.Map<EmployeeCreateRequestDto>(dto));

//            return CreatedAtAction(
//               "GetEmployeeById",  
//                "Employees",              
//                new { id = employee.Id },
//                employee
//            );
//        }

//        [HttpPost("forgot-password")]
//        [AllowAnonymous]
//        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestDTO dto)
//        {
//            if (!ModelState.IsValid)
//                return BadRequest(ModelState);

//            var result = await _authService.ForgotPasswordAsync(dto);

//            if (result.Succeeded)
//                return Ok(result);

//            return BadRequest(result.Errors.Select(x => x.Description));
//        }

//        [HttpPost("roles/{Email}")]
//        [Authorize(Roles = "Admin")]
//        public async Task<IActionResult> AssignRoles(string email, [FromBody] RoleAssignmentRequestDto dto)
//        {
//            Guid? userId = _employeeService.GetIdByEmail(email);
//            var response = await _authService.AssignRolesAsync(userId.Value, dto);
//            return Ok(response);
//        }

//        [HttpGet("roles/{userId:Guid}")]
//        [Authorize(Roles = "Admin,HR")]
//        public async Task<IActionResult> GetRoles(Guid userId)
//        {
//            var response = await _authService.GetRolesAsync(userId);
//            return Ok(response);
//        }

//        [HttpGet("me")]
//        [Authorize]
//        public async Task<IActionResult> GetCurrentUser()
//        {
//            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
//            var response = await _authService.GetCurrentUserAsync(userId);
//            return Ok(response);
//        }
//    }
//}
