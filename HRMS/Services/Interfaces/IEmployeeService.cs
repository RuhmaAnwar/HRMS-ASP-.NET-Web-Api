using HRMS.Dtos;
using HRMS.Dtos.RequestDtos;
using HRMS.Dtos.ResponseDtos;

namespace HRMS.Services.Interfaces
{
    public interface IEmployeeService
    {
        Task<(IEnumerable<EmployeeResponseDto> Employees, int TotalCount)> GetAllAsync(
            int page,
            int pageSize,
            Guid? departmentId,
            Guid? managerId,
            string? search,
            Guid currentUserId,
            string[] userRoles
        );

        Task<EmployeeResponseDto> GetByIdAsync(
            Guid id,
            Guid currentUserId,
            string[] userRoles
        );

        Task<(IEnumerable<EmployeeResponseDto> Subordinates, int TotalCount)> GetSubordinatesAsync(
            Guid id,
            int page,
            int pageSize,
            Guid currentUserId,
            string[] userRoles
        );

        Task<EmployeeResponseDto> CreateAsync(EmployeeCreateRequestDto dto);

        Task<EmployeeResponseDto> UpdateAsync(
            Guid id,
            EmployeeUpdateRequestDto dto,
            Guid currentUserId,
            string[] userRoles
        );

        Task DeleteAsync(Guid id);

        Task<LeaveBalanceResponseDto> GetLeaveBalanceAsync(
            Guid id,
            Guid currentUserId,
            string[] userRoles
        );

        Guid? GetIdByEmail(string email);
    }
}
