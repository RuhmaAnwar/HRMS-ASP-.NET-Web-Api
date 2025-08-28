using HRMS.Models;

namespace HRMS.Repositories.Interfaces
{
    public interface ILeaveRepository
    {
        Task<bool> ExistsAsync(Guid leaveId);
        Task<IEnumerable<Leave>> GetAllAsync(int page, int pageSize, Guid? employeeId, string? status, DateOnly? startDate, DateOnly? endDate);
        Task<Leave?> GetByIdAsync(Guid id);
        Task<Leave> CreateAsync(Leave leave);
        Task UpdateAsync(Leave leave);
        Task SoftDeleteAsync(Guid id);
        Task ApproveAsync(Leave leave);
        Task<IEnumerable<Leave>> GetByEmployeeIdAsync(Guid employeeId, int page, int pageSize, string? status, DateOnly? startDate, DateOnly? endDate);
        Task<int> GetTotalCountAsync(Guid? employeeId, string? status, DateOnly? startDate, DateOnly? endDate);
        Task<int> GetEmployeeLeavesCountAsync(Guid employeeId, string? status, DateOnly? startDate, DateOnly? endDate);
        Task<bool> HasOverlappingLeaveAsync(Guid employeeId, DateOnly startDate, DateOnly endDate, Guid? excludeLeaveId = null);
    }
}