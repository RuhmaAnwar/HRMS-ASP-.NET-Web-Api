using HRMS.Dtos.RequestDtos;
using HRMS.Services;
using HRMS.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HRMS.Controllers.v1
{
    [Route("api/v1/departments")]
    [ApiController]
    [Authorize]
    public class DepartmentsControllerV1 : ControllerBase
    {
        private readonly IDepartmentService _departmentService;

        public DepartmentsControllerV1(IDepartmentService departmentService)
        {
            _departmentService = departmentService;
        }

        [HttpGet]
        //[Authorize(Roles = "Admin,HR,Manager")]
        public async Task<IActionResult> GetDepartments(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null)
        {

            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var userRoles = User.FindAll(ClaimTypes.Role).Select(r => r.Value).ToArray();

            var (departments, totalCount) = await _departmentService.GetAllAsync(page, pageSize, search, userId, userRoles);

            return Ok(new { totalCount, departments });
        }

        [HttpGet("{id:Guid}")]
        public async Task<IActionResult> GeDepartmentById(Guid id)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var userRoles = User.FindAll(ClaimTypes.Role).Select(r => r.Value).ToArray();

            var department = await _departmentService.GetByIdAsync(id, userId, userRoles);

            return Ok(department);
        }

        [HttpPost]
        //[Authorize(Roles = "Admin,HR")]
        public async Task<IActionResult> CreateDepartment([FromBody] DepartmentCreateRequestDTO dto)
        {
            var department = await _departmentService.CreateAsync(dto);

            return CreatedAtAction(nameof(GeDepartmentById), new { id = department.Id }, department);
        }

        // /////

        [HttpPut("{id:Guid}")]
        public async Task<IActionResult> UpdateDepartment(Guid id, [FromBody] DepartmentUpdateRequestDTO dto)
        {
            var department = await _departmentService.UpdateAsync(id, dto);
            return Ok(department);
        }

        [HttpPatch("{id:Guid}")]
        public async Task<IActionResult> PartialUpdateDepartment(Guid id, [FromBody] DepartmentUpdateRequestDTO dto)
        {
            var department = await _departmentService.PartialUpdateAsync(id, dto);
            return Ok(department);
        }

        [HttpDelete("{id:Guid}")]
        public async Task<IActionResult> DeleteDepartment(Guid id)
        {
            await _departmentService.DeleteAsync(id);
            return NoContent();
        }

        [HttpPost("{id:Guid}/assign-head")]
        public async Task<IActionResult> AssignHead(Guid id, [FromBody] DepartmentAssignHeadRequestDTO dto)
        {
            var department = await _departmentService.AssignHeadAsync(id, dto);
            return Ok(department);
        }

        [HttpGet("{id:Guid}/employees")]
        public async Task<IActionResult> GetDepartmentEmployees(
            Guid id,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var (employees, totalCount) = await _departmentService.GetEmployeesAsync(id, page, pageSize);
            return Ok(new { totalCount, employees });
        }

        [HttpGet("{id:Guid}/positions")]
        public async Task<IActionResult> GetDepartmentPositions(
            Guid id,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var (positions, totalCount) = await _departmentService.GetPositionsAsync(id, page, pageSize);
            return Ok(new { totalCount, positions });
        }
    }
}
