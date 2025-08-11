using HRMS.Data;
using HRMS.Models;
using HRMS.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace HRMS.Controllers.v1
{
    [ApiController]
    [Route("api/v1/Employees")]
    public class EmployeesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public EmployeesController(ApplicationDbContext context)
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
                if (page < 1 || pageSize < 1)
                {
                    return BadRequest(new { Message = "Page and pageSize must be greater than 0." });
                }
                if (pageSize > 100) // Prevent excessive data
                {
                    return BadRequest(new { Message = "PageSize cannot exceed 100." });
                }

                var employees = await _context.GetEmployeesWithRawSqlAsync(filter, sort, sortDescending, page, pageSize);
                //Console.WriteLine(employees);
                return Ok(employees);
            }
            catch (Npgsql.PostgresException ex)
            {
                // Handle PostgreSQL-specific errors (e.g., invalid column name)
                if (ex.SqlState == "42703") // Undefined column
                {
                    return BadRequest(new { Message = $"Invalid sort column: {sort}." });
                }
                Console.WriteLine($"Database error: {ex.Message}");
                return StatusCode(500, new { Message = "Internal server error while retrieving employees." });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
                return StatusCode(500, new { Message = "Internal server error while retrieving employees." });
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
                return StatusCode(500, new { Message = "Internal server error while retrieving employee." });
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
                Department = employeeDTO.Department,
                Role = employeeDTO.Role
            };

            try
            {
                _context.Employees.Add(employee);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetEmployee), new { id = employee.Id }, employee);
            }
            catch (DbUpdateException ex) when (ex.InnerException is Npgsql.PostgresException pgEx)
            {
                switch (pgEx.SqlState)
                {
                    case "23505": // Unique violation (e.g., duplicate email)
                        return Conflict(new { Message = "An employee with this email already exists." });
                    case "23502": // Not-null violation
                        return BadRequest(new { Message = "A required field is missing." });
                    default:
                        Console.WriteLine($"Database error: {pgEx.Message}");
                        return StatusCode(500, new { Message = "Internal server error while creating employee." });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
                return StatusCode(500, new { Message = "Internal server error while creating employee." });
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
            employee.Department = employeeDTO.Department;
            employee.Role = employeeDTO.Role;

            try
            {
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                return Conflict(new { Message = "The employee record was modified by another user. Please refresh and try again." });
            }
            catch (DbUpdateException ex) when (ex.InnerException is Npgsql.PostgresException pgEx)
            {
                switch (pgEx.SqlState)
                {
                    case "23505": // Unique violation
                        return Conflict(new { Message = "An employee with this email already exists." });
                    case "23502": // Not-null violation
                        return BadRequest(new { Message = "A required field is missing." });
                    default:
                        Console.WriteLine($"Database error: {pgEx.Message}");
                        return StatusCode(500, new { Message = "Internal server error while updating employee." });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
                return StatusCode(500, new { Message = "Internal server error while updating employee." });
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
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"Database error: {ex.Message}");
                return StatusCode(500, new { Message = "Internal server error while deleting employee." });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
                return StatusCode(500, new { Message = "Internal server error while deleting employee." });
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
            if (employeeDTO.Department != null)
                employee.Department = employeeDTO.Department;
            if (employeeDTO.Role != null)
                employee.Role = employeeDTO.Role;

            try
            {
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                return Conflict(new { Message = "The employee record was modified by another user. Please refresh and try again." });
            }
            catch (DbUpdateException ex) when (ex.InnerException is Npgsql.PostgresException pgEx)
            {
                switch (pgEx.SqlState)
                {
                    case "23505": // Unique violation
                        return Conflict(new { Message = "An employee with this email already exists." });
                    case "23502": // Not-null violation
                        return BadRequest(new { Message = "A required field cannot be set to null." });
                    default:
                        Console.WriteLine($"Database error: {pgEx.Message}");
                        return StatusCode(500, new { Message = "Internal server error while updating employee." });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
                return StatusCode(500, new { Message = "Internal server error while updating employee." });
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