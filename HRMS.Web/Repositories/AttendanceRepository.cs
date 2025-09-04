using HRMS.Data;
using HRMS.Enums;
using HRMS.Models;
using HRMS.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS.Repositories
{
    public class AttendanceRepository : IAttendanceRepository
    {
        private readonly ApplicationDbContext _context;

        public AttendanceRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> ExistsAsync(Guid attendanceId)
        {
            return await _context.Attendances.AnyAsync(a => a.Id == attendanceId && !a.IsDeleted);
        }

        public async Task<IEnumerable<Attendance>> GetAllAsync(
            int page,
            int pageSize,
            Guid? employeeId,
            string? status,
            DateOnly? startDate,
            DateOnly? endDate)
        {
            var query = _context.Attendances
                .Where(a => !a.IsDeleted)
                .Include(a => a.Employee)
                .AsNoTracking();

            if (employeeId.HasValue)
                query = query.Where(a => a.EmployeeId == employeeId.Value);

            if (!string.IsNullOrEmpty(status))
                query = query.Where(a => a.Status == Enum.Parse<AttendanceStatus>(status));

            if (startDate.HasValue)
                query = query.Where(a => a.Date >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(a => a.Date <= endDate.Value);

            return await query
                .OrderBy(a => a.Date)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<Attendance?> GetByIdAsync(Guid id)
        {
            return await _context.Attendances
                .Include(a => a.Employee)
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == id && !a.IsDeleted);
        }

        public async Task<Attendance> CreateAsync(Attendance attendance)
        {
            _context.Attendances.Add(attendance);
            await _context.SaveChangesAsync();

            var createdAttendance = await _context.Attendances
            .Include(a => a.Employee)
                .FirstOrDefaultAsync(a => a.Id == attendance.Id && !a.IsDeleted);

            return createdAttendance;
        }

        public async Task UpdateAsync(Attendance attendance)
        {
            _context.Attendances.Update(attendance);
            await _context.SaveChangesAsync();
        }

        public async Task SoftDeleteAsync(Guid id)
        {
            //var attendance = await _context.Attendances.FirstOrDefaultAsync(a => a.Id == id && !a.IsDeleted);
            //if (attendance != null)
            //{
            //    attendance.IsDeleted = true;
            //    attendance.UpdatedAt = DateTime.UtcNow;
            //    await _context.SaveChangesAsync();
            //}
            var attendance = await _context.Attendances.FirstOrDefaultAsync(a => a.Id == id);
            if (attendance != null)
            {
                _context.Attendances.Remove(attendance);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Attendance>> GetByEmployeeIdAsync(
            Guid employeeId,
            int page,
            int pageSize,
            string? status,
            DateOnly? startDate,
            DateOnly? endDate)
        {
            var query = _context.Attendances
                .Where(a => a.EmployeeId == employeeId && !a.IsDeleted)
                .Include(a => a.Employee)
                .AsNoTracking();

            if (!string.IsNullOrEmpty(status))
                query = query.Where(a => a.Status == Enum.Parse<AttendanceStatus>(status));

            if (startDate.HasValue)
                query = query.Where(a => a.Date >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(a => a.Date <= endDate.Value);

            return await query
                .OrderBy(a => a.Date)
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
            var query = _context.Attendances.Where(a => !a.IsDeleted);

            if (employeeId.HasValue)
                query = query.Where(a => a.EmployeeId == employeeId.Value);

            if (!string.IsNullOrEmpty(status))
                query = query.Where(a => a.Status == Enum.Parse<AttendanceStatus>(status));

            if (startDate.HasValue)
                query = query.Where(a => a.Date >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(a => a.Date <= endDate.Value);

            return await query.CountAsync();
        }

        public async Task<int> GetEmployeeAttendanceCountAsync(
            Guid employeeId,
            string? status,
            DateOnly? startDate,
            DateOnly? endDate)
        {
            var query = _context.Attendances.Where(a => a.EmployeeId == employeeId && !a.IsDeleted);

            if (!string.IsNullOrEmpty(status))
                query = query.Where(a => a.Status == Enum.Parse<AttendanceStatus>(status));

            if (startDate.HasValue)
                query = query.Where(a => a.Date >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(a => a.Date <= endDate.Value);

            return await query.CountAsync();
        }

        public async Task<bool> HasAttendanceForDateAsync(Guid employeeId, DateOnly date)
        {
            return await _context.Attendances
                .AnyAsync(a => a.EmployeeId == employeeId && a.Date == date && !a.IsDeleted);
        }

        public async Task<Attendance?> GetAttendanceForDateAsync(Guid employeeId, DateOnly date)
        {
            return await _context.Attendances
                .FirstOrDefaultAsync(a => a.EmployeeId == employeeId && a.Date == date && !a.IsDeleted);
        }
    }
}
