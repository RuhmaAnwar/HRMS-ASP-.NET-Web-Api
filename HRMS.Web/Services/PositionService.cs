using AutoMapper;
using HRMS.Dtos.RequestDtos;
using HRMS.Dtos.ResponseDtos;
using HRMS.Models;
using HRMS.Repositories.Interfaces;
using HRMS.Services.Interfaces;
using HRMS.Middleware;

namespace HRMS.Services
{
    public class PositionService : IPositionService
    {
        private readonly IPositionRepository _positionRepository;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IMapper _mapper;

        public PositionService(
            IPositionRepository positionRepository,
            IDepartmentRepository departmentRepository,
            IEmployeeRepository employeeRepository,
            IMapper mapper)
        {
            _positionRepository = positionRepository;
            _departmentRepository = departmentRepository;
            _employeeRepository = employeeRepository;
            _mapper = mapper;
        }

        public async Task<(IEnumerable<PositionResponseDTO> Positions, int TotalCount)> GetAllAsync(
            int page, int pageSize, Guid? departmentId, string? search, Guid currentUserId, string[] userRoles)
        {
            if (userRoles.Contains("Employee"))
                throw new UnauthorizedAccessException("Employees cannot access all positions.");

            Guid? departmentIdToUse = userRoles.Contains("Manager")
                ? await _departmentRepository.GetDepartmentByEmployeeId(currentUserId)
                : departmentId;

            var positions = await _positionRepository.GetAllAsync(page, pageSize, departmentIdToUse, search);
            var totalCount = await _positionRepository.GetTotalCountAsync(departmentIdToUse, search);

            var response = _mapper.Map<IEnumerable<PositionResponseDTO>>(positions);
            return (response, totalCount);
        }

        public async Task<PositionResponseDTO> GetByIdAsync(Guid id, Guid currentUserId, string[] userRoles)
        {
            var position = await _positionRepository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Position with ID {id} not found.");

            var userDeptId = await _departmentRepository.GetDepartmentByEmployeeId(currentUserId);

            if (userRoles.Contains("Employee"))
            {
                var user = await _employeeRepository.GetByIdAsync(currentUserId);
                if (user?.PositionId != id)
                    throw new UnauthorizedAccessException("Employees can only access their own position.");
            }

            if (userRoles.Contains("Manager") && position.DepartmentId != userDeptId)
                throw new UnauthorizedAccessException("Managers can only access positions in their department.");

            return _mapper.Map<PositionResponseDTO>(position);
        }

        public async Task<PositionResponseDTO> CreateAsync(PositionCreateRequestDTO dto)
        {
            if (!await _departmentRepository.ExistsAsync(dto.DepartmentId))
                throw new KeyNotFoundException($"Department with ID {dto.DepartmentId} not found.");

            if (await _positionRepository.CheckPositionByTitleAsync(dto.Title))
                throw new ConflictException("Position with this title already exists.");

            if (dto.SalaryRangeMax < dto.SalaryRangeMin || dto.SalaryRangeMax < 0 || dto.SalaryRangeMin < 0)
                throw new ValidationException("Invalid min or max salary values"); // 400 bad request for invalid salary range values

            var position = _mapper.Map<Position>(dto);
            var result = await _positionRepository.CreateAsync(position);

            return _mapper.Map<PositionResponseDTO>(result);
        }

        public async Task<PositionResponseDTO> UpdateAsync(Guid id, PositionUpdateRequestDTO dto)
        {
            if (string.IsNullOrEmpty(dto.Title) ||
                !dto.SalaryRangeMin.HasValue ||
                !dto.SalaryRangeMax.HasValue ||
                !dto.DepartmentId.HasValue)
            {
                throw new ArgumentException("Title, SalaryRangeMin, SalaryRangeMax, and DepartmentId are required for full update.");
            }

            var position = await _positionRepository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Position with ID {id} not found.");

            if (!await _departmentRepository.ExistsAsync(dto.DepartmentId.Value))
                throw new KeyNotFoundException($"Department with ID {dto.DepartmentId} not found.");

            if (dto.Title != position.Title && await _positionRepository.CheckPositionByTitleAsync(dto.Title))
                throw new ConflictException("Position with this title already exists.");

            _mapper.Map(dto, position);
            await _positionRepository.UpdateAsync(position);

            return _mapper.Map<PositionResponseDTO>(position);
        }

        public async Task<PositionResponseDTO> PartialUpdateAsync(Guid id, PositionUpdateRequestDTO dto)
        {
            var position = await _positionRepository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Position with ID {id} not found.");

            if (dto.DepartmentId.HasValue && !await _departmentRepository.ExistsAsync(dto.DepartmentId.Value))
                throw new KeyNotFoundException($"Department with ID {dto.DepartmentId} not found.");

            if (!string.IsNullOrEmpty(dto.Title) &&
                dto.Title != position.Title &&
                await _positionRepository.CheckPositionByTitleAsync(dto.Title))
            {
                throw new ConflictException("Position with this title already exists.");
            }

            if (!string.IsNullOrEmpty(dto.Title))
                position.Title = dto.Title;

            if (!string.IsNullOrEmpty(dto.Description))
                position.Description = dto.Description;

            if (dto.SalaryRangeMin.HasValue)
                position.SalaryRangeMin = dto.SalaryRangeMin.Value;

            if (dto.SalaryRangeMax.HasValue)
                position.SalaryRangeMax = dto.SalaryRangeMax.Value;

            if (dto.DepartmentId.HasValue)
                position.DepartmentId = dto.DepartmentId.Value;

            position.UpdatedAt = DateTime.UtcNow;
            await _positionRepository.UpdateAsync(position);

            return _mapper.Map<PositionResponseDTO>(position);
        }

        public async Task DeleteAsync(Guid id)
        {
            if (!await _positionRepository.ExistsAsync(id))
                throw new KeyNotFoundException($"Position with ID {id} not found.");

            await _positionRepository.SoftDeleteAsync(id);
        }

        public async Task<(IEnumerable<EmployeeResponseDto> Employees, int TotalCount)> GetEmployeesAsync(
            Guid id, int page, int pageSize, Guid currentUserId, string[] userRoles)
        {
            if (!await _positionRepository.ExistsAsync(id))
                throw new KeyNotFoundException($"Position with ID {id} not found.");

            var position = await _positionRepository.GetByIdAsync(id);
            var userDeptId = await _departmentRepository.GetDepartmentByEmployeeId(currentUserId);

            if (userRoles.Contains("Employee"))
            {
                var user = await _employeeRepository.GetByIdAsync(currentUserId);
                if (user?.PositionId != id)
                    throw new UnauthorizedAccessException("Employees can only access their own position.");
            }

            if (userRoles.Contains("Manager") && position.DepartmentId != userDeptId)
                throw new UnauthorizedAccessException("Managers can only access positions in their department.");

            var employees = await _positionRepository.GetEmployeesAsync(id, page, pageSize);
            var totalCount = await _positionRepository.GetEmployeesCountAsync(id);

            var response = _mapper.Map<IEnumerable<EmployeeResponseDto>>(employees);
            return (response, totalCount);
        }
    }
}
