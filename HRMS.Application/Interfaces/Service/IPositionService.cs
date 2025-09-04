using HRMS.Dtos.RequestDtos;
using HRMS.Dtos.ResponseDtos;

namespace HRMS.Services.Interfaces
{
    public interface IPositionService
    {
        Task<(IEnumerable<PositionResponseDTO> Positions, int TotalCount)> GetAllAsync(
            int page, int pageSize, Guid? departmentId, string? search, Guid currentUserId, string[] userRoles);

        Task<PositionResponseDTO> GetByIdAsync(Guid id, Guid currentUserId, string[] userRoles);

        Task<PositionResponseDTO> CreateAsync(PositionCreateRequestDTO dto);

        Task<PositionResponseDTO> UpdateAsync(Guid id, PositionUpdateRequestDTO dto);

        Task<PositionResponseDTO> PartialUpdateAsync(Guid id, PositionUpdateRequestDTO dto);

        Task DeleteAsync(Guid id);

        Task<(IEnumerable<EmployeeResponseDto> Employees, int TotalCount)> GetEmployeesAsync(
            Guid id, int page, int pageSize, Guid currentUserId, string[] userRoles);
    }
}
