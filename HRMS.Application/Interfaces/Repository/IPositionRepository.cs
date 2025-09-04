using HRMS.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HRMS.Repositories.Interfaces
{
    public interface IPositionRepository
    {
        Task<bool> ExistsAsync(Guid positionId);

        Task<IEnumerable<Position>> GetAllAsync(int page, int pageSize, Guid? departmentId, string? search);

        Task<Position?> GetByIdAsync(Guid id);

        Task<Position> CreateAsync(Position position);

        Task UpdateAsync(Position position);

        Task SoftDeleteAsync(Guid id);

        Task<IEnumerable<Employee>> GetEmployeesAsync(Guid positionId, int page, int pageSize);

        Task<int> GetEmployeesCountAsync(Guid positionId);

        Task<int> GetTotalCountAsync(Guid? departmentId, string? search);

        Task<bool> CheckPositionByTitleAsync(string title);
    }
}
