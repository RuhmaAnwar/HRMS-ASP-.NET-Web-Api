using AutoMapper;
using HRMS.Data;
using HRMS.Dtos.RequestDtos;
using HRMS.Dtos.ResponseDtos;
using HRMS.Models;
using HRMS.Repositories.Interfaces;
using HRMS.Services.Interfaces;
using System.Data.Entity.Infrastructure;
using HRMS.Middleware;

namespace HRMS.Services
{
    public class DepartmentService: IDepartmentService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IPositionRepository _positionRepository;
        private readonly IMapper _mapper;

        public DepartmentService(
            IEmployeeRepository employeeRepository,
            IDepartmentRepository departmentRepository,
            IPositionRepository positionRepository,
            IMapper mapper
        )
        {
            _employeeRepository = employeeRepository;
            _departmentRepository = departmentRepository;
            _positionRepository = positionRepository;
            _mapper = mapper;
        }
        public async Task<(IEnumerable<DepartmentResponseDTO> Departments, int TotalCount)> GetAllAsync(
            int page,
            int pageSize,
            string? search,
            Guid currentUserId,
            string[] userRoles
        )
        {
            if (userRoles.Contains("Employee"))
                throw new UnauthorizedAccessException("Employees cannot access all Departments.");

            Guid? headIdToUse = userRoles.Contains("Manager") ? currentUserId : null;

            var departments = await _departmentRepository.GetAllAsync(page, pageSize, headIdToUse , search);
            var totalCount = await _departmentRepository.GetTotalCountAsync(headIdToUse, search);

            var response = _mapper.Map<IEnumerable<DepartmentResponseDTO>>(departments);
            return (response, totalCount);
        }

        public async Task<DepartmentResponseDTO> GetByIdAsync(
            Guid id,
            Guid currentUserId,
            string[] userRoles
        )
        {
            Guid userDeptId = await _departmentRepository.GetDepartmentByEmployeeId(currentUserId);
            if (userRoles.Contains("Employee") && id != userDeptId)
                throw new UnauthorizedAccessException("Employees can only access their own Department.");

            var department = await _departmentRepository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Department with ID {id} not found.");

            if (userRoles.Contains("Manager") && department.HeadEmployeeId != currentUserId)
                throw new UnauthorizedAccessException("Managers can only access Department they head");

            return _mapper.Map<DepartmentResponseDTO>(department);
        }

        public async Task<DepartmentResponseDTO> CreateAsync(DepartmentCreateRequestDTO dto)
        {   
            if ( dto.HeadEmployeeEmail != null)
            {
                if ( _employeeRepository.GetIdByEmail(dto.HeadEmployeeEmail) == null)
                    throw new KeyNotFoundException("No Employee with this Email exists");
            }

            if (!await _departmentRepository.CheckDepartmentByName(dto.Name))
                throw new ConflictException("Department of this name already exists");

            // check if employee already a head

            var department = _mapper.Map<Department>(dto);

            if(dto.HeadEmployeeEmail != null)
                department.HeadEmployeeId = _employeeRepository.GetIdByEmail(dto.HeadEmployeeEmail);

            var result = await _departmentRepository.CreateAsync(department);

            return _mapper.Map<DepartmentResponseDTO>(department);
        }
        public async Task<DepartmentResponseDTO> UpdateAsync(Guid id, DepartmentUpdateRequestDTO dto)
        {
            if (string.IsNullOrEmpty(dto.Name) || string.IsNullOrEmpty(dto.Description))
            {
                throw new ArgumentException("Name and Description are required for full update.");
            }

            var department = await _departmentRepository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Department with ID {id} not found.");

            // Check for unique name if changed
            if (dto.Name != department.Name && await _departmentRepository.CheckDepartmentByName(dto.Name))
            {
                throw new ConflictException("Department with this name already exists.");
            }

            _mapper.Map(dto, department);

            if (!string.IsNullOrEmpty(dto.HeadEmployeeEmail))
            {
                var headId = _employeeRepository.GetIdByEmail(dto.HeadEmployeeEmail)
                    ?? throw new KeyNotFoundException("No Employee with this Email exists.");
                department.HeadEmployeeId = headId;
            }

            await _departmentRepository.UpdateAsync(department);

            return _mapper.Map<DepartmentResponseDTO>(department);
        }

        public async Task<DepartmentResponseDTO> PartialUpdateAsync(Guid id, DepartmentUpdateRequestDTO dto)
        {
            var department = await _departmentRepository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Department with ID {id} not found.");

            if (!string.IsNullOrEmpty(dto.Name))
            {
                if (dto.Name != department.Name && await _departmentRepository.CheckDepartmentByName(dto.Name))
                {
                    throw new ConflictException("Department with this name already exists.");
                }
                department.Name = dto.Name;
            }

            if (!string.IsNullOrEmpty(dto.Description))
            {
                department.Description = dto.Description;
            }

            if (!string.IsNullOrEmpty(dto.HeadEmployeeEmail))
            {
                var headId = _employeeRepository.GetIdByEmail(dto.HeadEmployeeEmail)
                    ?? throw new KeyNotFoundException("No Employee with this Email exists.");
                department.HeadEmployeeId = headId;
            }

            department.UpdatedAt = DateTime.UtcNow;
            await _departmentRepository.UpdateAsync(department);

            return _mapper.Map<DepartmentResponseDTO>(department);
        }

        public async Task DeleteAsync(Guid id)
        {
            if (!await _departmentRepository.ExistsAsync(id))
            {
                throw new KeyNotFoundException($"Department with ID {id} not found.");
            }

            await _departmentRepository.SoftDeleteAsync(id);
        }

        public async Task<DepartmentResponseDTO> AssignHeadAsync(Guid id, DepartmentAssignHeadRequestDTO dto)
        {
            var department = await _departmentRepository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Department with ID {id} not found.");

            var headId = _employeeRepository.GetIdByEmail(dto.HeadEmployeeEmail)
                ?? throw new KeyNotFoundException("No Employee with this Email exists.");

            await _departmentRepository.AssignHeadAsync(id, headId);

            return _mapper.Map<DepartmentResponseDTO>(department);
        }

        public async Task<(IEnumerable<EmployeeResponseDto> Employees, int TotalCount)> GetEmployeesAsync(Guid id, int page, int pageSize)
        {
            if (!await _departmentRepository.ExistsAsync(id))
            {
                throw new KeyNotFoundException($"Department with ID {id} not found.");
            }

            var employees = await _departmentRepository.GetEmployeesAsync(id, page, pageSize);
            var totalCount = await _departmentRepository.GetEmployeesCountAsync(id);

            var response = _mapper.Map<IEnumerable<EmployeeResponseDto>>(employees);
            return (response, totalCount);
        }

        public async Task<(IEnumerable<PositionResponseDTO> Positions, int TotalCount)> GetPositionsAsync(Guid id, int page, int pageSize)
        {
            if (!await _departmentRepository.ExistsAsync(id))
            {
                throw new KeyNotFoundException($"Department with ID {id} not found.");
            }

            var positions = await _departmentRepository.GetPositionsAsync(id, page, pageSize);
            var totalCount = await _departmentRepository.GetPositionsCountAsync(id);

            var response = _mapper.Map<IEnumerable<PositionResponseDTO>>(positions);
            return (response, totalCount);
        }
    }
}
    