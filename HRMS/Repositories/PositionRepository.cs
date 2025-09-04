using HRMS.Data;
using HRMS.Models;
using HRMS.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Repositories
{
    public class PositionRepository : IPositionRepository
    {
        private readonly ApplicationDbContext _context;

        public PositionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> ExistsAsync(Guid positionId)
        {
            return await _context.Positions.AnyAsync(p => p.Id == positionId && !p.IsDeleted);
        }

        public async Task<IEnumerable<Position>> GetAllAsync(int page, int pageSize, Guid? departmentId, string? search)
        {
            var query = _context.Positions
                .Where(p => !p.IsDeleted)
                .Include(p => p.Department)
                .AsNoTracking();

            if (departmentId.HasValue)
                query = query.Where(p => p.DepartmentId == departmentId.Value);

            if (!string.IsNullOrEmpty(search))
                query = query.Where(p =>
                    EF.Functions.ILike(p.Title, $"%{search}%") ||
                    EF.Functions.ILike(p.Description, $"%{search}%"));

            return await query
                .OrderBy(p => p.Title)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<Position?> GetByIdAsync(Guid id)
        {
            return await _context.Positions
                .Include(p => p.Department)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);
        }

        public async Task<Position> CreateAsync(Position position)
        {
            _context.Positions.Add(position);
            await _context.SaveChangesAsync();

            var createdPosition = await _context.Positions
                .Include(p => p.Department)
                .FirstOrDefaultAsync(p => p.Id == position.Id && !p.IsDeleted);

            return createdPosition!;
        }


        public async Task UpdateAsync(Position position)
        {
            _context.Positions.Update(position);
            await _context.SaveChangesAsync();
        }

        public async Task SoftDeleteAsync(Guid id)
        {
            var position = await _context.Positions.FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);
            if (position != null)
            {
                position.IsDeleted = true;
                position.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Employee>> GetEmployeesAsync(Guid positionId, int page, int pageSize)
        {
            return await _context.Employees
                .Where(e => e.PositionId == positionId && !e.IsDeleted)
                .Include(e => e.Department)
                .Include(e => e.Position)
                .Include(e => e.Manager)
                .OrderBy(e => e.LastName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> GetEmployeesCountAsync(Guid positionId)
        {
            return await _context.Employees
                .CountAsync(e => e.PositionId == positionId && !e.IsDeleted);
        }

        public async Task<int> GetTotalCountAsync(Guid? departmentId, string? search)
        {
            var query = _context.Positions.Where(p => !p.IsDeleted);

            if (departmentId.HasValue)
                query = query.Where(p => p.DepartmentId == departmentId.Value);

            if (!string.IsNullOrEmpty(search))
                query = query.Where(p =>
                    EF.Functions.ILike(p.Title, $"%{search}%") ||
                    EF.Functions.ILike(p.Description, $"%{search}%"));

            return await query.CountAsync();
        }

        public async Task<bool> CheckPositionByTitleAsync(string title)
        {
            return await _context.Positions.AnyAsync(p => p.Title == title && !p.IsDeleted);
        }
    }
}
