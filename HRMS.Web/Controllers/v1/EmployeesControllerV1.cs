
using HRMS.Dtos.RequestDtos;
using HRMS.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HRMS.Controllers.v1
{
    [ApiController]
    [Route("api/v1/employees")]
    [Authorize]
    public class EmployeesControllerV1 : ControllerBase
    {
        private readonly IEmployeeService _employeeService;

        public EmployeesControllerV1(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,HR,Manager")]
        public async Task<IActionResult> GetEmployees(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] Guid? departmentId = null,
            [FromQuery] Guid? managerId = null,
            [FromQuery] string? search = null)
        {
            
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var userRoles = User.FindAll(ClaimTypes.Role).Select(r => r.Value).ToArray();

            var (employees, totalCount) = await _employeeService.GetAllAsync(page, pageSize, departmentId, managerId, search, userId, userRoles);

            return Ok(new { totalCount, employees });
        }

        [HttpGet("{id:Guid}")]
        public async Task<IActionResult> GetEmployeeById(Guid id)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var userRoles = User.FindAll(ClaimTypes.Role).Select(r => r.Value).ToArray();

            var employee = await _employeeService.GetByIdAsync(id, userId, userRoles);

            return Ok(employee);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,HR")]
        public async Task<IActionResult> CreateEmployee([FromBody] EmployeeCreateRequestDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { Errors = GetModelStateErrors() }); ///////////////////////

            var employee = await _employeeService.CreateAsync(dto);

            return CreatedAtAction(nameof(GetEmployeeById),new { id = employee.Id }, employee);
        }

        [HttpPut("{id:Guid}")]
        public async Task<IActionResult> UpdateEmployee(Guid id, [FromBody] EmployeeUpdateRequestDto dto)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var userRoles = User.FindAll(ClaimTypes.Role).Select(r => r.Value).ToArray();

            var employee = await _employeeService.UpdateAsync(id, dto, userId, userRoles);

            return Ok(employee);
        }

        [HttpDelete("{id:Guid}")]
        [Authorize(Roles = "Admin,HR")]
        public async Task<IActionResult> DeleteEmployee(Guid id)
        {
            await _employeeService.DeleteAsync(id);

            return NoContent();
        }

        [HttpGet("{id:Guid}/leave-balance")]
        public async Task<IActionResult> GetLeaveBalance(Guid id)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var userRoles = User.FindAll(ClaimTypes.Role).Select(r => r.Value).ToArray();

            var leaveBalance = await _employeeService.GetLeaveBalanceAsync(id, userId, userRoles);

            return Ok(leaveBalance);
        }

        [HttpGet("{id:Guid}/subordinates")]
        [Authorize(Roles = "Admin,HR,Manager")]
        public async Task<IActionResult> GetSubordinates(
            Guid id,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var userRoles = User.FindAll(ClaimTypes.Role).Select(r => r.Value).ToArray();

            var (subordinates, totalCount) = await _employeeService.GetSubordinatesAsync(id, page, pageSize, userId, userRoles);

            return Ok(new { totalCount, subordinates });
        }

        private List<string> GetModelStateErrors()
        {
            return ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();
        }
    }
}
