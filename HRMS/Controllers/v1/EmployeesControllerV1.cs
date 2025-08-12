using HRMS.Data;
using HRMS.Models;
using HRMS.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HRMS.Utilities;



namespace HRMS.Controllers.v1
{

    [ApiController]
    [Route("api/v1/employees")]
    public class EmployeesControllerV1 : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public EmployeesControllerV1(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/v1/employees?filter=John&sort=FirstName&sortDescending=true&page=1&pageSize=10
        [HttpGet]
        public async Task<IActionResult> GetEmployees(
            [FromQuery] string filter = "",
            [FromQuery] string sort = "Id",
            [FromQuery] bool sortDescending = false,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                // Validate pagination parameters
               var result = RequestValidator.ValidatePaginationParameters(page,pageSize);
                if (result == null)
                {
                    var employees = await _context.GetEmployeesWithRawSqlAsync(filter, sort, sortDescending, page, pageSize);
                    Console.WriteLine(employees.ToString());
                    return Ok(employees);
                }
                return result;
            }
            catch (Exception ex)
            {
                // Handle PostgreSQL-specific errors (e.g., invalid column name)
                return ExceptionHandler.HandleException(ex, "employee", "Internal server error while retrieving employees");
            }
        }

        // GET: api/v1/employees/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetEmployee(int id)
        {
            try
            {
                var employee = await _context.Employees.FindAsync(id);
                if (employee == null)
                {
                    return NotFound(new { Message = "Employee does not exist." });
                }
                return Ok(employee);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching employee {id}: {ex.Message}");
                return ExceptionHandler.HandleException(ex, "employee");
            }
        }

        // POST: api/v1/employees
        [HttpPost]
        public async Task<IActionResult> CreateEmployee([FromBody] EmployeeDTO employeeDTO)
        {
            if (employeeDTO == null || !ModelState.IsValid)
            {
                return BadRequest(new { Errors = GetModelStateErrors() });
            }

            var employee = new Employee
            {
                FirstName = employeeDTO.FirstName,
                LastName = employeeDTO.LastName,
                Email = employeeDTO.Email,
                DepartmentId = employeeDTO.DepartmentId,
                Role = employeeDTO.Role
            };

            try
            {
                _context.Employees.Add(employee);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetEmployee), new { id = employee.Id }, employee);
            }
            catch (Exception ex)
            {
                return ExceptionHandler.HandleException(ex, "employee", "Internal server error while creating employee.");
            }
        }

        // PUT: api/v1/employees/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEmployee(int id, [FromBody] EmployeeDTO employeeDTO)
        {
            if (employeeDTO == null || !ModelState.IsValid)
            {
                return BadRequest(new { Errors = GetModelStateErrors() });
            }

            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound(new { Message = "Employee not found." });
            }

            employee.FirstName = employeeDTO.FirstName;
            employee.LastName = employeeDTO.LastName;
            employee.Email = employeeDTO.Email;
            employee.DepartmentId = employeeDTO.DepartmentId;
            employee.Role = employeeDTO.Role;

            try
            {
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                return ExceptionHandler.HandleException(ex, "employee", "Internal server error while updating employee.");
            }
        }

        // DELETE: api/v1/employees/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound(new { Message = "Employee not found." });
            }

            try
            {
                _context.Employees.Remove(employee);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                return ExceptionHandler.HandleException(ex, "employee", "Internal server error while deleting employee.");
            }
        }

        // PATCH: api/v1/employees/{id}
        [HttpPatch("{id}")]
        public async Task<IActionResult> ModifyEmployee(int id, [FromBody] EmployeeDTOPatch employeeDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { Errors = GetModelStateErrors() });
            }

            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound(new { Message = "Employee not found." });
            }

            if (employeeDTO.FirstName != null)
                employee.FirstName = employeeDTO.FirstName;
            if (employeeDTO.LastName != null)
                employee.LastName = employeeDTO.LastName;
            if (employeeDTO.Email != null)
                employee.Email = employeeDTO.Email;
            employee.DepartmentId = employeeDTO.DepartmentId;
            if (employeeDTO.Role != null)
                employee.Role = employeeDTO.Role;

            try
            {
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                return ExceptionHandler.HandleException(ex, "employee", "Internal server error while updating employee.");
            }
        }

        // Helper method to extract ModelState errors
        private List<string> GetModelStateErrors()
        {
            return ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();
        }
    }
}