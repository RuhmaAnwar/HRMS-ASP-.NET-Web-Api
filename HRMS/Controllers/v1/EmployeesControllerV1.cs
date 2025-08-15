using HRMS.Models.DTO;
using HRMS.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.Controllers.v1
{
    [ApiController]
    [Route("api/v1/employees")]
    public class EmployeesControllerV1 : ControllerBase
    {
        private readonly IEmployeeService _employeeService;

        public EmployeesControllerV1(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        [HttpGet]
        public async Task<IActionResult> GetEmployees(
            [FromQuery] string filter = "",
            [FromQuery] string sort = "Id",
            [FromQuery] bool sortDescending = false,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            return await _employeeService.GetAllAsync(filter, sort, sortDescending, page, pageSize);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEmployee(int id)
        {
            return await _employeeService.GetByIdAsync(id);
        }

        [HttpPost]
        public async Task<IActionResult> CreateEmployee([FromBody] EmployeeDTO employeeDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { Errors = GetModelStateErrors() });

            return await _employeeService.CreateAsync(employeeDTO);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEmployee(int id, [FromBody] EmployeeDTO employeeDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { Errors = GetModelStateErrors() });

            return await _employeeService.UpdateAsync(id, employeeDTO);
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> ModifyEmployee(int id, [FromBody] EmployeeDTOPatch employeeDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { Errors = GetModelStateErrors() });

            return await _employeeService.PatchAsync(id, employeeDTO);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            return await _employeeService.DeleteAsync(id);
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