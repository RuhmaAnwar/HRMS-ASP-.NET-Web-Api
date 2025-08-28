using HRMS.Dtos.RequestDtos;
using HRMS.Dtos.ResponseDtos;
using HRMS.Models;

namespace HRMS.Services.Interfaces
{
    public interface IDepartmentService
    {
        Task<(IEnumerable<DepartmentResponseDTO> Departments, int TotalCount)> GetAllAsync(
            int page,
            int pageSize,
            string? search,
            Guid currentUserId,
            string[] userRoles
        );

        Task<DepartmentResponseDTO> GetByIdAsync(
            Guid id,
            Guid currentUserId,
            string[] userRoles
        );

        Task<DepartmentResponseDTO> CreateAsync(DepartmentCreateRequestDTO dto);
        Task<DepartmentResponseDTO> UpdateAsync(Guid id, DepartmentUpdateRequestDTO dto);

        Task<DepartmentResponseDTO> PartialUpdateAsync(Guid id, DepartmentUpdateRequestDTO dto);

        Task DeleteAsync(Guid id);

        Task<DepartmentResponseDTO> AssignHeadAsync(Guid id, DepartmentAssignHeadRequestDTO dto);

        Task<(IEnumerable<EmployeeResponseDto> Employees, int TotalCount)> GetEmployeesAsync(Guid id, int page, int pageSize);

        Task<(IEnumerable<PositionResponseDTO> Positions, int TotalCount)> GetPositionsAsync(Guid id, int page, int pageSize);

    }
}