using AutoMapper;
using HRMS.Data;
using HRMS.Dtos.RequestDtos;
using HRMS.Dtos.ResponseDtos;
using HRMS.Middleware;
using HRMS.Models;
using HRMS.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using HRMS.Services.Interfaces;

namespace HRMS.Services
{
    
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IPositionRepository _positionRepository;
        private readonly UserManager<Employee> _userManager;
        private readonly IMapper _mapper;

        public EmployeeService(
            IEmployeeRepository employeeRepository,
            IDepartmentRepository departmentRepository,
            IPositionRepository positionRepository,
            UserManager<Employee> userManager,
            IMapper mapper
        )
        {
            _employeeRepository = employeeRepository;
            _departmentRepository = departmentRepository;
            _positionRepository = positionRepository;
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<(IEnumerable<EmployeeResponseDto> Employees, int TotalCount)> GetAllAsync(
            int page,
            int pageSize,
            Guid? departmentId,
            Guid? managerId,
            string? search,
            Guid currentUserId,
            string[] userRoles
        )
        {
            if (userRoles.Contains("Employee"))
                throw new UnauthorizedAccessException("Employees cannot access all employees.");

            var managerIdToUse = userRoles.Contains("Manager") ? currentUserId : managerId;

            if (userRoles.Contains("Manager") && managerId.HasValue && managerId != currentUserId)
                throw new UnauthorizedAccessException("Managers can only access their own subordinates.");

            var employees = await _employeeRepository.GetAllAsync(page, pageSize, departmentId, managerIdToUse, search);
            var totalCount = await _employeeRepository.GetTotalCountAsync(departmentId, managerIdToUse, search);

            var response = _mapper.Map<IEnumerable<EmployeeResponseDto>>(employees);
            return (response, totalCount);
        }

        public async Task<EmployeeResponseDto> GetByIdAsync(Guid id, Guid currentUserId, string[] userRoles)
        {
            if (userRoles.Contains("Employee") && id != currentUserId)
                throw new UnauthorizedAccessException("Employees can only access their own data.");

            var employee = await _employeeRepository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Employee with ID {id} not found or is deleted.");

            if (userRoles.Contains("Manager") && employee.ManagerId != currentUserId)
                throw new UnauthorizedAccessException("Managers can only access their subordinates' data.");

            return _mapper.Map<EmployeeResponseDto>(employee);
        }

        public async Task<EmployeeResponseDto> CreateAsync(EmployeeCreateRequestDto dto)
        {
            if (!await _departmentRepository.ExistsAsync(dto.DepartmentId))
                throw new ValidationException("Invalid DepartmentId.");

            if (!await _positionRepository.ExistsAsync(dto.PositionId))
                throw new ValidationException("Invalid PositionId.");

            if (dto.ManagerId.HasValue && ! await _employeeRepository.ManagerExistsAsync(dto.ManagerId))
                throw new ValidationException("Invalid ManagerId.");

            var (isSalaryValid, messsage) = await _employeeRepository.CheckSalaryValidity(dto.Salary, dto.PositionId); // check if salary for position is valid
            if (!isSalaryValid) 
                throw new ValidationException(messsage);

            var employee = _mapper.Map<Employee>(dto);
            employee.Id = Guid.NewGuid();
            employee.CreatedAt = DateTime.UtcNow;
            employee.UpdatedAt = DateTime.UtcNow;

            var result = await _userManager.CreateAsync(employee, dto.Password);
            if (!result.Succeeded)
                throw new ValidationException(string.Join("; ", result.Errors.Select(e => e.Description)));
            
            if (dto.Roles?.Any() == true)
                await _userManager.AddToRolesAsync(employee, dto.Roles);

            return _mapper.Map<EmployeeResponseDto>(employee);
        }

        public async Task<EmployeeResponseDto> UpdateAsync(
            Guid id,
            EmployeeUpdateRequestDto dto,
            Guid currentUserId,
            string[] userRoles
        )
        {
            var employee = await _employeeRepository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Employee with ID {id} not found or is deleted.");

            if (userRoles.Contains("Employee") && id != currentUserId)
                throw new UnauthorizedAccessException("Employees can only update their own data.");

            if (userRoles.Contains("Manager") && employee.ManagerId != currentUserId)
                throw new UnauthorizedAccessException("Managers can only update their subordinates' data.");

            if (userRoles.Contains("Manager") &&
                (dto.Salary != employee.Salary || dto.TotalLeaves != employee.TotalLeaves || dto.LeavesUsed != employee.LeavesUsed))
                throw new UnauthorizedAccessException("Managers cannot update Salary, TotalLeaves, or LeavesUsed.");

            if (userRoles.Contains("Employee") &&
                (dto.Salary != employee.Salary || dto.TotalLeaves != employee.TotalLeaves ||
                 dto.LeavesUsed != employee.LeavesUsed || dto.DepartmentId != employee.DepartmentId ||
                 dto.PositionId != employee.PositionId || dto.ManagerId != employee.ManagerId))
                throw new UnauthorizedAccessException("Employees cannot update Salary, TotalLeaves, LeavesUsed, DepartmentId, PositionId, or ManagerId.");

            if (dto.TotalLeaves < dto.LeavesUsed || dto.LeavesUsed < 0)
                throw new ValidationException("TotalLeaves must be greater than or equal to LeavesUsed, and LeavesUsed must be non-negative.");

            if (!await _departmentRepository.ExistsAsync(dto.DepartmentId))
                throw new ValidationException("Invalid DepartmentId.");

            if (!await _positionRepository.ExistsAsync(dto.PositionId))
                throw new ValidationException("Invalid PositionId.");

            if (dto.ManagerId.HasValue && ! await _employeeRepository.ManagerExistsAsync(dto.ManagerId))
                throw new ValidationException("Invalid ManagerId.");

            _mapper.Map(dto, employee);
            employee.UpdatedAt = DateTime.UtcNow;

            await _userManager.UpdateAsync(employee);
            await _employeeRepository.UpdateAsync(employee);

            return _mapper.Map<EmployeeResponseDto>(employee);
        }

        public async Task DeleteAsync(Guid id)
        {
            var employee = await _employeeRepository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Employee with ID {id} not found or is deleted.");

            await _employeeRepository.SoftDeleteAsync(id);
        }

        public async Task<LeaveBalanceResponseDto> GetLeaveBalanceAsync(Guid id, Guid currentUserId, string[] userRoles)
        {
            var employee = await _employeeRepository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Employee with ID {id} not found or is deleted.");

            if (userRoles.Contains("Employee") && id != currentUserId)
                throw new UnauthorizedAccessException("Employees can only access their own leave balance.");

            if (userRoles.Contains("Manager") && employee.ManagerId != currentUserId)
                throw new UnauthorizedAccessException("Managers can only access their subordinates' leave balance.");

            return _mapper.Map<LeaveBalanceResponseDto>(employee);
        }

        public async Task<(IEnumerable<EmployeeResponseDto> Subordinates, int TotalCount)> GetSubordinatesAsync(
            Guid id,
            int page,
            int pageSize,
            Guid currentUserId,
            string[] userRoles
        )
        {
            if (userRoles.Contains("Employee"))
                throw new UnauthorizedAccessException("Employees cannot access subordinates.");

            if (userRoles.Contains("Manager") && id != currentUserId)
                throw new UnauthorizedAccessException("Managers can only access their own subordinates.");

            var subordinates = await _employeeRepository.GetSubordinatesAsync(id, page, pageSize);
            var totalCount = await _employeeRepository.GetTotalCountAsync(null, id, null);

            var response = _mapper.Map<IEnumerable<EmployeeResponseDto>>(subordinates);
            return (response, totalCount);
        }

        public Guid? GetIdByEmail(string email)
        {
            return _employeeRepository.GetIdByEmail(email);
        }
    }
}
