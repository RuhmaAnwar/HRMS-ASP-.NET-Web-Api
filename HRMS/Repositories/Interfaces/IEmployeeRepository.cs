using HRMS.Models; 
namespace HRMS.Repositories.Interfaces
{
    public interface IEmployeeRepository
    {
        Task<IEnumerable<Employee>> GetAllAsync(
            int page,
            int pageSize,
            Guid? departmentId,
            Guid? managerId,
            string? search
        );

        Task<Employee?> GetByIdAsync(Guid id);

        Task<Employee> CreateAsync(Employee employee);

        Task UpdateAsync(Employee employee);

        Task SoftDeleteAsync(Guid id);

        Task<IEnumerable<Employee>> GetSubordinatesAsync(
            Guid managerId,
            int page,
            int pageSize
        );

        Task<int> GetTotalCountAsync(
            Guid? departmentId,
            Guid? managerId,
            string? search
        );

        Task UpdateLeaveBalanceAsync(Guid id, int leavesUsed);

        Guid? GetIdByEmail(string email);
        Task<bool> ManagerExistsAsync(Guid? managerId);

        Task SaveChangesAsync();

        Task<(bool isValid, string message)> CheckSalaryValidity(decimal salary, Guid posId);

    }

}
