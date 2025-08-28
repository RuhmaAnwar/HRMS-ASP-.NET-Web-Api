using HRMS.Data;
using HRMS.Enums;
using HRMS.Models;
using HRMS.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace HRMS.Repositories
{
    public class LeaveRepository : ILeaveRepository
    {
        private readonly ApplicationDbContext _context;

        public LeaveRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> ExistsAsync(Guid leaveId)
        {
            return await _context.Leaves.AnyAsync(l => l.Id == leaveId && !l.IsDeleted);
        }

        public async Task<IEnumerable<Leave>> GetAllAsync(
            int page,
            int pageSize,
            Guid? employeeId,
            string? status,
            DateOnly? startDate,
            DateOnly? endDate)
        {
            var query = _context.Leaves
                .Where(l => !l.IsDeleted)
                .Include(l => l.Employee)
                .Include(l => l.Approver)
                .AsNoTracking();

            if (employeeId.HasValue)
                query = query.Where(l => l.EmployeeId == employeeId.Value);

            if (!string.IsNullOrEmpty(status))
                query = query.Where(l => l.Status == Enum.Parse<LeaveStatus>(status));

            if (startDate.HasValue)
                query = query.Where(l => l.StartDate >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(l => l.EndDate <= endDate.Value);

            return await query
                .OrderBy(l => l.StartDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<Leave?> GetByIdAsync(Guid id)
        {
            return await _context.Leaves
                .Include(l => l.Employee)
                .Include(l => l.Approver)
                .AsNoTracking()
                .FirstOrDefaultAsync(l => l.Id == id && !l.IsDeleted);
        }

        public async Task<Leave> CreateAsync(Leave leave)
        {
            _context.Leaves.Add(leave);
            await _context.SaveChangesAsync();

            var createdLeave = await _context.Leaves
            .Include(l => l.Employee)
            .Include(l => l.Approver)
            .FirstOrDefaultAsync(l => l.Id == leave.Id && !l.IsDeleted);

            return createdLeave!;
        }

        public async Task UpdateAsync(Leave leave)
        {
            _context.Leaves.Update(leave);
            await _context.SaveChangesAsync();
        }

        public async Task SoftDeleteAsync(Guid id)
        {
            var leave = await _context.Leaves.FirstOrDefaultAsync(l => l.Id == id && !l.IsDeleted);
            if (leave != null)
            {
                leave.IsDeleted = true;
                leave.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        public async Task ApproveAsync(Leave leave)
        {
            _context.Leaves.Update(leave);
            await _context.SaveChangesAsync();

        }

        public async Task<IEnumerable<Leave>> GetByEmployeeIdAsync(
            Guid employeeId,
            int page,
            int pageSize,
            string? status,
            DateOnly? startDate,
            DateOnly? endDate)
        {
            var query = _context.Leaves
                .Where(l => l.EmployeeId == employeeId && !l.IsDeleted)
                .Include(l => l.Employee)
                .Include(l => l.Approver)
                .AsNoTracking();

            if (!string.IsNullOrEmpty(status))
                query = query.Where(l => l.Status == Enum.Parse<LeaveStatus>(status));

            if (startDate.HasValue)
                query = query.Where(l => l.StartDate >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(l => l.EndDate <= endDate.Value);

            return await query
                .OrderBy(l => l.StartDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> GetTotalCountAsync(
            Guid? employeeId,
            string? status,
            DateOnly? startDate,
            DateOnly? endDate)
        {
            var query = _context.Leaves.Where(l => !l.IsDeleted);

            if (employeeId.HasValue)
                query = query.Where(l => l.EmployeeId == employeeId.Value);

            if (!string.IsNullOrEmpty(status))
                query = query.Where(l => l.Status == Enum.Parse<LeaveStatus>(status));

            if (startDate.HasValue)
                query = query.Where(l => l.StartDate >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(l => l.EndDate <= endDate.Value);

            return await query.CountAsync();
        }

        public async Task<int> GetEmployeeLeavesCountAsync(
            Guid employeeId,
            string? status,
            DateOnly? startDate,
            DateOnly? endDate)
        {
            var query = _context.Leaves.Where(l => l.EmployeeId == employeeId && !l.IsDeleted);

            if (!string.IsNullOrEmpty(status))
                query = query.Where(l => l.Status == Enum.Parse<LeaveStatus>(status));

            if (startDate.HasValue)
                query = query.Where(l => l.StartDate >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(l => l.EndDate <= endDate.Value);

            return await query.CountAsync();
        }

        public async Task<bool> HasOverlappingLeaveAsync(
            Guid employeeId,
            DateOnly startDate,
            DateOnly endDate,
            Guid? excludeLeaveId = null)
        {
            var query = _context.Leaves
                .Where(l => l.EmployeeId == employeeId && !l.IsDeleted && l.Status != LeaveStatus.Rejected);

            if (excludeLeaveId.HasValue)
                query = query.Where(l => l.Id != excludeLeaveId.Value);

            return await query.AnyAsync(l => l.StartDate <= endDate && l.EndDate >= startDate);
        }
    }
}
