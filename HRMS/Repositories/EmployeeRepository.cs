using HRMS.Data;
using HRMS.Models;
using HRMS.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HRMS.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly ApplicationDbContext _context;

        public EmployeeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Employee>> GetAllAsync(string filter, string sort, bool sortDescending, int page, int pageSize)
        {
            filter = string.IsNullOrWhiteSpace(filter) ? "%%" : $"%{filter.Replace("'", "''")}%";
            var sortColumn = sort switch
            {
                "FirstName" => "e.first_name",
                "LastName" => "e.last_name",
                "Email" => "e.email",
                "Role" => "e.role",
                "DepartmentId" => "e.department_id",
                "DepartmentName" => "d.name",
                _ => "e.id"
            };
            var sortDirection = sortDescending ? "DESC" : "ASC";
            var offset = (page - 1) * pageSize;

            var query = @"
                SELECT e.id, e.first_name, e.last_name, e.email, e.department_id, e.role, d.name AS department_name
                FROM employees e
                JOIN departments d ON e.department_id = d.id
                WHERE e.first_name ILIKE @filter
                   OR e.last_name ILIKE @filter
                   OR e.email ILIKE @filter
                   OR d.name ILIKE @filter
                ORDER BY {0} {1}
                LIMIT @pageSize OFFSET @offset";

            query = string.Format(query, sortColumn, sortDirection);
            var employees = await _context.Employees
                .FromSqlRaw(query,
                    new NpgsqlParameter("@filter", filter),
                    new NpgsqlParameter("@pageSize", pageSize),
                    new NpgsqlParameter("@offset", offset))
                .Include(e => e.Department) // Ensure Department is loaded
                .ToListAsync();

            return employees;
        }

        public async Task<Employee?> GetByIdAsync(int id)
        {
            return await _context.Employees
                .Include(e => e.Department)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task AddAsync(Employee employee)
        {
            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Employee employee)
        {
            _context.Employees.Update(employee);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee != null)
            {
                _context.Employees.Remove(employee);
                await _context.SaveChangesAsync();
            }
        }
    }
}