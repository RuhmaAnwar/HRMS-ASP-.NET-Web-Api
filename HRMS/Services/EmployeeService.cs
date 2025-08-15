using AutoMapper;
using HRMS.Models;
using HRMS.Models.DTO;
using HRMS.Repositories.Interfaces;
using HRMS.Services.Interfaces;
using HRMS.Utilities;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HRMS.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IMapper _mapper;

        public EmployeeService(IEmployeeRepository employeeRepository, IDepartmentRepository departmentRepository, IMapper mapper)
        {
            _employeeRepository = employeeRepository;
            _departmentRepository = departmentRepository;
            _mapper = mapper;
        }

        public async Task<IActionResult> GetAllAsync(string filter, string sort, bool sortDescending, int page, int pageSize)
        {
            var validationResult = RequestValidator.ValidatePaginationParameters(page, pageSize);
            if (validationResult != null)
                return validationResult;

            try
            {
                var employees = await _employeeRepository.GetAllAsync(filter, sort, sortDescending, page, pageSize);
                var employeeDTOs = _mapper.Map<List<EmployeeDTO>>(employees);
                return new OkObjectResult(employeeDTOs);
            }
            catch (Exception ex)
            {
                return ExceptionHandler.HandleException(ex, "employee", "Internal server error while retrieving employees.");
            }
        }

        public async Task<IActionResult> GetByIdAsync(int id)
        {
            try
            {
                var employee = await _employeeRepository.GetByIdAsync(id);
                if (employee == null)
                    return new NotFoundObjectResult(new { Message = "Employee does not exist." });
                var employeeDTO = _mapper.Map<EmployeeDTO>(employee);
                return new OkObjectResult(employeeDTO);
            }
            catch (Exception ex)
            {
                return ExceptionHandler.HandleException(ex, "employee", "Internal server error while retrieving employee.");
            }
        }

        public async Task<IActionResult> CreateAsync(EmployeeDTO employeeDTO)
        {
            if (employeeDTO == null)
                return new BadRequestObjectResult(new { Message = "Employee data is required." });

            try
            {
                var department = await _departmentRepository.GetByIdAsync(employeeDTO.DepartmentId);
                if (department == null)
                    return new BadRequestObjectResult(new { Message = "Invalid department ID." });

                var employee = _mapper.Map<Employee>(employeeDTO);
                await _employeeRepository.AddAsync(employee);
                return new CreatedAtActionResult("GetEmployee", "EmployeesControllerV1", new { id = employee.Id }, _mapper.Map<EmployeeDTO>(employee));
            }
            catch (Exception ex)
            {
                return ExceptionHandler.HandleException(ex, "employee", "Internal server error while creating employee.");
            }
        }

        public async Task<IActionResult> UpdateAsync(int id, EmployeeDTO employeeDTO)
        {
            if (employeeDTO == null)
                return new BadRequestObjectResult(new { Message = "Employee data is required." });

            try
            {
                var employee = await _employeeRepository.GetByIdAsync(id);
                if (employee == null)
                    return new NotFoundObjectResult(new { Message = "Employee not found." });

                var department = await _departmentRepository.GetByIdAsync(employeeDTO.DepartmentId);
                if (department == null)
                    return new BadRequestObjectResult(new { Message = "Invalid department ID." });

                _mapper.Map(employeeDTO, employee);
                await _employeeRepository.UpdateAsync(employee);
                return new NoContentResult();
            }
            catch (Exception ex)
            {
                return ExceptionHandler.HandleException(ex, "employee", "Internal server error while updating employee.");
            }
        }

        public async Task<IActionResult> PatchAsync(int id, EmployeeDTOPatch employeeDTO)
        {
            if (employeeDTO == null)
                return new BadRequestObjectResult(new { Message = "Employee data is required." });

            try
            {
                var employee = await _employeeRepository.GetByIdAsync(id);
                if (employee == null)
                    return new NotFoundObjectResult(new { Message = "Employee not found." });

                var department = await _departmentRepository.GetByIdAsync(employeeDTO.DepartmentId);
                if (department == null)
                    return new BadRequestObjectResult(new { Message = "Invalid department ID." });

                _mapper.Map(employeeDTO, employee);
                await _employeeRepository.UpdateAsync(employee);
                return new NoContentResult();
            }
            catch (Exception ex)
            {
                return ExceptionHandler.HandleException(ex, "employee", "Internal server error while updating employee.");
            }
        }

        public async Task<IActionResult> DeleteAsync(int id)
        {
            try
            {
                var employee = await _employeeRepository.GetByIdAsync(id);
                if (employee == null)
                    return new NotFoundObjectResult(new { Message = "Employee not found." });

                await _employeeRepository.DeleteAsync(id);
                return new NoContentResult();
            }
            catch (Exception ex)
            {
                return ExceptionHandler.HandleException(ex, "employee", "Internal server error while deleting employee.");
            }
        }
    }
}