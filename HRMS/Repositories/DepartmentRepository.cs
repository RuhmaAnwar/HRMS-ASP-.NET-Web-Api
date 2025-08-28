using HRMS.Data;
using HRMS.Models;
using HRMS.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Repositories
{
    public class DepartmentRepository : IDepartmentRepository
    {

        private readonly ApplicationDbContext _context;

        public DepartmentRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<bool> ExistsAsync(Guid departmentId)
        {
            return await _context.Departments.AnyAsync(d => d.Id == departmentId && !d.IsDeleted);
        }
        public async Task<IEnumerable<Department>> GetAllAsync(
            int page,
            int pageSize,
            Guid? headId,
            string search
        )
        {
            var query = _context.Departments
                .Where(d => !d.IsDeleted)
                .Include(d => d.Employees)
                .Include(d => d.Positions)
                .Include(d => d.HeadEmployee)
                .AsNoTracking();

            if (headId.HasValue)
                query = query.Where(d => d.Id == headId.Value);

            if (!string.IsNullOrEmpty(search))
                query = query.Where(
                    d =>
                        EF.Functions.ILike(d.Name, $"%{search}%")
                        || EF.Functions.ILike(d.Description, $"%{search}%")
                );

            return await query
                .OrderBy(d => d.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<Department?> GetByIdAsync(Guid id)
        {
            return await _context.Departments
                .Include(d => d.Employees)
                .Include(d => d.Positions)
                .Include(d => d.HeadEmployee)
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.Id == id && !d.IsDeleted);
                
        }

        public async Task<int> GetTotalCountAsync(
            Guid? headId,
            string? search
        )
        {
            var query = _context.Departments.Where(d => !d.IsDeleted);

            if (headId.HasValue)
                query = query.Where(d => d.Id == headId.Value);

            if (!string.IsNullOrEmpty(search))
                query = query.Where(
                    d =>
                        EF.Functions.ILike(d.Name, $"%{search}%")
                        || EF.Functions.ILike(d.Description, $"%{search}%")
                );

            return await query.CountAsync();
        }

        public  async Task<Guid> GetDepartmentByEmployeeId(Guid id)
        {
            var user = await _context.Employees.FirstOrDefaultAsync(e => e.Id == id);
            return user.DepartmentId;
        }

        public async Task<bool> CheckDepartmentByName(string name)
        {
            return await _context.Departments.AnyAsync(d => d.Name == name && !d.IsDeleted);
        }

        public async Task<Department> CreateAsync(Department department)
        {
            _context.Departments.Add(department);
            await _context.SaveChangesAsync();

            var createdDepartment = await _context.Departments
             .Include(d => d.Employees)
             .Include(d => d.Positions)
             .Include(d => d.HeadEmployee)
             .FirstOrDefaultAsync(d => d.Id == department.Id && !d.IsDeleted);

            return department;
        }


        public async Task UpdateAsync(Department department)
        {
            _context.Departments.Update(department);
            await _context.SaveChangesAsync();
        }

        public async Task SoftDeleteAsync(Guid id)
        {
            var department = await _context.Departments.FirstOrDefaultAsync(d => d.Id == id && !d.IsDeleted);
            if (department != null)
            {
                department.IsDeleted = true;
                department.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        public async Task AssignHeadAsync(Guid departmentId, Guid headEmployeeId)
        {
            var department = await _context.Departments.FirstOrDefaultAsync(d => d.Id == departmentId && !d.IsDeleted);
            if (department != null)
            {
                // Check if the employee is already heading another department
                var existingHeadDept = await _context.Departments.FirstOrDefaultAsync(d => d.HeadEmployeeId == headEmployeeId && !d.IsDeleted);
                if (existingHeadDept != null && existingHeadDept.Id != departmentId)
                {
                    throw new InvalidOperationException("Employee is already heading another department.");
                }

                department.HeadEmployeeId = headEmployeeId;
                department.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Employee>> GetEmployeesAsync(Guid departmentId, int page, int pageSize)
        {
            return await _context.Employees
                .Where(e => e.DepartmentId == departmentId && !e.IsDeleted)
                .Include(e => e.Position)
                .Include(e => e.Manager)
                .OrderBy(e => e.LastName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> GetEmployeesCountAsync(Guid departmentId)
        {
            return await _context.Employees
                .CountAsync(e => e.DepartmentId == departmentId && !e.IsDeleted);
        }

        public async Task<IEnumerable<Position>> GetPositionsAsync(Guid departmentId, int page, int pageSize)
        {
            return await _context.Positions
                .Where(p => p.DepartmentId == departmentId && !p.IsDeleted)
                .Include(p => p.Department)
                .Include(p => p.Employees)
                .OrderBy(p => p.Title)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> GetPositionsCountAsync(Guid departmentId)
        {
            return await _context.Positions
                .CountAsync(p => p.DepartmentId == departmentId && !p.IsDeleted);
        }

        public async Task<string> GetDepartmentName(Guid id)
        {
            var dept = await _context.Departments.FirstOrDefaultAsync(d => d.Id  == id);
            return dept.Name;
        }
    }
}