using HRMS.Dtos.RequestDtos;
using HRMS.Dtos.ResponseDtos;

namespace HRMS.Services.Interfaces
{
    public interface ILeaveService
    {
        Task<(IEnumerable<LeaveResponseDTO> Leaves, int TotalCount)> GetAllAsync(
            int page,
            int pageSize,
            Guid? employeeId,
            string? status,
            DateOnly? startDate,
            DateOnly? endDate,
            Guid currentUserId,
            string[] userRoles);

        Task<LeaveResponseDTO> GetByIdAsync(
            Guid id,
            Guid currentUserId,
            string[] userRoles);

        Task<LeaveResponseDTO> CreateAsync(
            LeaveCreateRequestDTO dto,
            Guid currentUserId,
            string[] userRoles);

        Task<LeaveResponseDTO> UpdateAsync(
            Guid id,
            LeaveUpdateRequestDTO dto,
            Guid currentUserId,
            string[] userRoles);

        Task<LeaveResponseDTO> PartialUpdateAsync(
            Guid id,
            LeaveUpdateRequestDTO dto,
            Guid currentUserId,
            string[] userRoles);

        Task DeleteAsync(
            Guid id,
            Guid currentUserId,
            string[] userRoles);

        Task<LeaveResponseDTO> ApproveAsync(
            Guid id,
            LeaveApprovalRequestDTO dto,
            Guid currentUserId,
            string[] userRoles);

        Task<(IEnumerable<LeaveResponseDTO> Leaves, int TotalCount)> GetByEmployeeIdAsync(
            Guid employeeId,
            int page,
            int pageSize,
            string? status,
            DateOnly? startDate,
            DateOnly? endDate,
            Guid currentUserId,
            string[] userRoles);
        (bool isValid, string message) CheckLeaveValidity(LeaveCreateRequestDTO dto);
    }
}
