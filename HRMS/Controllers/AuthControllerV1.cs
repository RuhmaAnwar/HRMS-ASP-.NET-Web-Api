using HRMS.Models;
using HRMS.Models.DTO;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/v1/auth")]
public class AuthControllerV1 : ControllerBase
{
    private readonly UserManager<Employee> _userManager;
    private readonly SignInManager<Employee> _signInManager;

    public AuthControllerV1(UserManager<Employee> userManager, SignInManager<Employee> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] EmployeeDTO dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var employee = new Employee
        {
            UserName = dto.Email,  
            Email = dto.Email,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            DepartmentId = dto.DepartmentId,
            Role = dto.Role
        };

        var result = await _userManager.CreateAsync(employee, dto.Password);
        if (!result.Succeeded)
            return BadRequest(result.Errors);

        // Assign the role from EmployeeDTO.Role to Identity role
        if (!string.IsNullOrEmpty(dto.Role))
        {
            var roleResult = await _userManager.AddToRoleAsync(employee, dto.Role);
            if (!roleResult.Succeeded)
                return BadRequest(new { Errors = roleResult.Errors.Select(e => e.Description) });
        }

        return Ok(new { Message = "Employee registered successfully" });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDTO login)  // created LoginDTO with Email and Password
    {
        var result = await _signInManager.PasswordSignInAsync(login.Email, login.Password, login.RememberMe, lockoutOnFailure: false);
        if (!result.Succeeded)
            return Unauthorized(new { Message = "Invalid login attempt" });

        // Cookie is automatically issued on successful sign-in
        return Ok(new { Message = "Logged in successfully" });
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return Ok(new { Message = "Logged out" });
    }
}