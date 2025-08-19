using AutoMapper;
using HRMS.Data;
using HRMS.Models;
using HRMS.Models.DTO;
using HRMS.Services.Interfaces;
using HRMS.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly UserManager<Employee> _userManager;

        public EmployeeService(ApplicationDbContext context, IMapper mapper, UserManager<Employee> userManager)
        {
            _context = context;
            _mapper = mapper;
            _userManager = userManager;
        }

        public async Task<IActionResult> GetAllAsync(string filter, string sort, bool sortDescending, int page, int pageSize)
        {
            // Validate pagination (unchanged)
            var validationResult = RequestValidator.ValidatePaginationParameters(page, pageSize);
            if (validationResult != null) return validationResult;

            try
            {
                var query = _context.Employees
                    .Include(e => e.Department)
                    .AsQueryable();

                // Apply filter (example: filter by name or email)
                if (!string.IsNullOrEmpty(filter))
                {
                    query = query.Where(e => e.FirstName.Contains(filter) || e.LastName.Contains(filter) || e.Email.Contains(filter));
                }

                // Apply sorting
                query = sort.ToLower() switch
                {
                    "firstname" => sortDescending ? query.OrderByDescending(e => e.FirstName) : query.OrderBy(e => e.FirstName),
                    "lastname" => sortDescending ? query.OrderByDescending(e => e.LastName) : query.OrderBy(e => e.LastName),
                    "email" => sortDescending ? query.OrderByDescending(e => e.Email) : query.OrderBy(e => e.Email),
                    _ => sortDescending ? query.OrderByDescending(e => e.Id) : query.OrderBy(e => e.Id),
                };

                // Pagination
                var total = await query.CountAsync();
                var employees = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var result = _mapper.Map<List<EmployeeDTO>>(employees);
                return new OkObjectResult(new { Total = total, Employees = result });
            }
            catch (Exception ex)
            {
                return ExceptionHandler.HandleException(ex, "employee");
            }
        }

        public async Task<IActionResult> GetByIdAsync(int id)
        {
            try
            {
                var employee = await _context.Employees
                    .Include(e => e.Department)
                    .FirstOrDefaultAsync(e => e.Id == id);
                if (employee == null)
                    return new NotFoundObjectResult(new { Message = "Employee not found" });

                return new OkObjectResult(_mapper.Map<EmployeeDTO>(employee));
            }
            catch (Exception ex)
            {
                return ExceptionHandler.HandleException(ex, "employee");
            }
        }

        public async Task<IActionResult> CreateAsync(EmployeeDTO employeeDTO)
        {
            try
            {
                // Map DTO to Employee, excluding PasswordHash
                var employee = _mapper.Map<Employee>(employeeDTO);
                employee.UserName = employeeDTO.Email; // Set UserName to Email

                // Use UserManager to create user with password
                var result = await _userManager.CreateAsync(employee, employeeDTO.Password);
                if (!result.Succeeded)
                    return new BadRequestObjectResult(new { Errors = result.Errors.Select(e => e.Description) });

                return new CreatedAtActionResult("GetEmployee", "EmployeesControllerV1", new { id = employee.Id }, _mapper.Map<EmployeeDTO>(employee));
            }
            catch (Exception ex)
            {
                return ExceptionHandler.HandleException(ex, "employee");
            }
        }

        public async Task<IActionResult> UpdateAsync(int id, EmployeeDTO employeeDTO)
        {
            try
            {
                var employee = await _userManager.FindByIdAsync(id.ToString());
                if (employee == null)
                    return new NotFoundObjectResult(new { Message = "Employee not found" });

                // Update properties
                _mapper.Map(employeeDTO, employee);
                employee.UserName = employeeDTO.Email; // Sync UserName

                var result = await _userManager.UpdateAsync(employee);
                if (!result.Succeeded)
                    return new BadRequestObjectResult(new { Errors = result.Errors.Select(e => e.Description) });

                // If password provided, update it
                if (!string.IsNullOrEmpty(employeeDTO.Password))
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(employee);
                    var passwordResult = await _userManager.ResetPasswordAsync(employee, token, employeeDTO.Password);
                    if (!passwordResult.Succeeded)
                        return new BadRequestObjectResult(new { Errors = passwordResult.Errors.Select(e => e.Description) });
                }

                return new OkObjectResult(_mapper.Map<EmployeeDTO>(employee));
            }
            catch (Exception ex)
            {
                return ExceptionHandler.HandleException(ex, "employee");
            }
        }

        public async Task<IActionResult> PatchAsync(int id, EmployeeDTOPatch employeeDTO)
        {
            try
            {
                var employee = await _userManager.FindByIdAsync(id.ToString());
                if (employee == null)
                    return new NotFoundObjectResult(new { Message = "Employee not found" });

                // Apply partial update
                _mapper.Map(employeeDTO, employee);
                if (employeeDTO.Email != null)
                    employee.UserName = employeeDTO.Email; // Sync UserName

                var result = await _userManager.UpdateAsync(employee);
                if (!result.Succeeded)
                    return new BadRequestObjectResult(new { Errors = result.Errors.Select(e => e.Description) });

                return new OkObjectResult(_mapper.Map<EmployeeDTO>(employee));
            }
            catch (Exception ex)
            {
                return ExceptionHandler.HandleException(ex, "employee");
            }
        }

        public async Task<IActionResult> DeleteAsync(int id)
        {
            try
            {
                var employee = await _userManager.FindByIdAsync(id.ToString());
                if (employee == null)
                    return new NotFoundObjectResult(new { Message = "Employee not found" });

                var result = await _userManager.DeleteAsync(employee);
                if (!result.Succeeded)
                    return new BadRequestObjectResult(new { Errors = result.Errors.Select(e => e.Description) });

                return new NoContentResult();
            }
            catch (Exception ex)
            {
                return ExceptionHandler.HandleException(ex, "employee");
            }
        }
    }
}