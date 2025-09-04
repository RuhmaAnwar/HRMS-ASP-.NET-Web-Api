using HRMS.Data;
using HRMS.Models;
using HRMS.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Repositories
{

    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly ApplicationDbContext _context;

        public EmployeeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Employee>> GetAllAsync(
            int page,
            int pageSize,
            Guid? departmentId,
            Guid? managerId,
            string? search
        )
        {
            var query = _context.Employees
                .Where(e => !e.IsDeleted)
                .Include(e => e.Department)
                .Include(e => e.Position)
                .Include(e => e.Manager)
                .AsNoTracking();

            if (departmentId.HasValue)
                query = query.Where(e => e.DepartmentId == departmentId.Value);

            if (managerId.HasValue)
                query = query.Where(e => e.ManagerId == managerId.Value);

            if (!string.IsNullOrEmpty(search))
                query = query.Where(
                    e =>
                        EF.Functions.ILike(e.FirstName, $"%{search}%")
                        || EF.Functions.ILike(e.LastName, $"%{search}%")
                );

            return await query
                .OrderBy(e => e.FirstName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<Employee?> GetByIdAsync(Guid id)
        {
            return await _context.Employees
                .Include(e => e.Department)
                .Include(e => e.Position)
                .Include(e => e.Manager)
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted);

        }

        public async Task<Employee> CreateAsync(Employee employee)
        {
            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            var createdEmployee = await _context.Employees
                .Include(e => e.Department)
                .Include(e => e.Position)
                .Include(e => e.Manager)
                .FirstOrDefaultAsync(e => e.Id == employee.Id && !e.IsDeleted);

            return employee;
        }

        public async Task UpdateAsync(Employee employee)
        {
            _context.Employees.Update(employee);
            await _context.SaveChangesAsync();
        }

        public async Task SoftDeleteAsync(Guid id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee != null)
            {
                employee.IsDeleted = true;
                employee.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Employee>> GetSubordinatesAsync(
            Guid managerId,
            int page,
            int pageSize
        )
        {
            return await _context.Employees
                .Where(e => e.ManagerId == managerId && !e.IsDeleted)
                .Include(e => e.Department)
                .Include(e => e.Position)
                .OrderBy(e => e.FirstName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> GetTotalCountAsync(
            Guid? departmentId,
            Guid? managerId,
            string? search
        )
        {
            var query = _context.Employees.Where(e => !e.IsDeleted);

            if (departmentId.HasValue)
                query = query.Where(e => e.DepartmentId == departmentId.Value);

            if (managerId.HasValue)
                query = query.Where(e => e.ManagerId == managerId.Value);

            if (!string.IsNullOrEmpty(search))
                query = query.Where(
                    e =>
                        EF.Functions.ILike(e.FirstName, $"%{search}%")
                        || EF.Functions.ILike(e.LastName, $"%{search}%")
                );

            return await query.CountAsync();
        }

        public async Task UpdateLeaveBalanceAsync(Guid id, int leavesUsed)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee != null)
            {
                employee.LeavesUsed = leavesUsed;
                employee.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        public Guid? GetIdByEmail(string email)
        {
            var employee = _context.Employees.FirstOrDefault(e =>
                e.Email == email && !e.IsDeleted
            );
            return employee != null ? employee.Id : Guid.Empty;
        }

        public async Task<bool> ManagerExistsAsync(Guid? managerId)
        {
            return await _context.Employees
                .AnyAsync(e => e.Id == managerId && !e.IsDeleted);
        }


        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    

    public async Task<(bool isValid, string message)> CheckSalaryValidity(decimal salary, Guid posId)
        {
            var position = await _context.Positions.FindAsync(posId);
            if (salary < position.SalaryRangeMin || salary > position.SalaryRangeMax)
            {
                return (false, $"salary for {position.Title} position should be between {position.SalaryRangeMin} and {position.SalaryRangeMax}");
            }
            return (true, "valid salary");
        }
    }
}