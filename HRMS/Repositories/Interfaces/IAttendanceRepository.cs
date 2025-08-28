using HRMS.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HRMS.Repositories.Interfaces
{
    public interface IAttendanceRepository
    {
        Task<bool> ExistsAsync(Guid attendanceId);
        Task<IEnumerable<Attendance>> GetAllAsync(int page, int pageSize, Guid? employeeId, string? status, DateOnly? startDate, DateOnly? endDate);
        Task<Attendance?> GetByIdAsync(Guid id);
        Task<Attendance> CreateAsync(Attendance attendance);
        Task UpdateAsync(Attendance attendance);
        Task SoftDeleteAsync(Guid id);
        Task<IEnumerable<Attendance>> GetByEmployeeIdAsync(Guid employeeId, int page, int pageSize, string? status, DateOnly? startDate, DateOnly? endDate);
        Task<int> GetTotalCountAsync(Guid? employeeId, string? status, DateOnly? startDate, DateOnly? endDate);
        Task<int> GetEmployeeAttendanceCountAsync(Guid employeeId, string? status, DateOnly? startDate, DateOnly? endDate);
        Task<bool> HasAttendanceForDateAsync(Guid employeeId, DateOnly date);
        Task<Attendance?> GetAttendanceForDateAsync(Guid employeeId, DateOnly date);
    }
}
