using HRMS.Models;
using Microsoft.EntityFrameworkCore;
using System.Data.Entity;

namespace HRMS.Repositories.Interfaces
{
    public interface IDepartmentRepository
    {
        Task<bool> ExistsAsync(Guid departmentId);
        Task<IEnumerable<Department>> GetAllAsync(
           int page,
           int pageSize,
           Guid? headId,
           string search
        );
        Task<Department?> GetByIdAsync(Guid id);
        Task<int> GetTotalCountAsync(
            Guid? headId,
            string? search
        );

        Task<Guid> GetDepartmentByEmployeeId(Guid id);

        Task<bool> CheckDepartmentByName(string name);

        Task<Department> CreateAsync(Department department);
        Task UpdateAsync(Department department);

        Task SoftDeleteAsync(Guid id);

        Task AssignHeadAsync(Guid departmentId, Guid headEmployeeId);

        Task<IEnumerable<Employee>> GetEmployeesAsync(Guid departmentId, int page, int pageSize);

        Task<int> GetEmployeesCountAsync(Guid departmentId);

        Task<IEnumerable<Position>> GetPositionsAsync(Guid departmentId, int page, int pageSize);

        Task<int> GetPositionsCountAsync(Guid departmentId);
        Task<string> GetDepartmentName(Guid id);
    }
}
