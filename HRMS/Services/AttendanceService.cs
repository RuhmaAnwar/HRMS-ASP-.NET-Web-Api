using AutoMapper;
using HRMS.Dtos.RequestDtos;
using HRMS.Dtos.ResponseDtos;
using HRMS.Enums;
using HRMS.Middleware;
using HRMS.Models;
using HRMS.Repositories.Interfaces;
using HRMS.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Services
{
    public class AttendanceService : IAttendanceService
    {
        private readonly IAttendanceRepository _attendanceRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ILeaveRepository _leaveRepository;
        private readonly IMapper _mapper;

        public AttendanceService(
            IAttendanceRepository attendanceRepository,
            IEmployeeRepository employeeRepository,
            ILeaveRepository leaveRepository,
            IMapper mapper)
        {
            _attendanceRepository = attendanceRepository;
            _employeeRepository = employeeRepository;
            _leaveRepository = leaveRepository;
            _mapper = mapper;
        }

        public async Task<(IEnumerable<AttendanceResponseDTO> Attendances, int TotalCount)> GetAllAsync(
            int page,
            int pageSize,
            Guid? employeeId,
            string? status,
            DateOnly? startDate,
            DateOnly? endDate,
            Guid currentUserId,
            string[] userRoles)
        {
            if (userRoles.Contains("Employee"))
                throw new UnauthorizedAccessException("Employees cannot access all attendance records.");

            if (userRoles.Contains("Manager"))
            {
                var subordinates = await _employeeRepository.GetSubordinatesAsync(currentUserId, 1, int.MaxValue);
                var subordinateIds = subordinates.Select(e => e.Id).ToList();

                employeeId = employeeId.HasValue && subordinateIds.Contains(employeeId.Value)
                    ? employeeId
                    : null;

                if (!employeeId.HasValue)
                {
                    var attendances = await _attendanceRepository.GetAllAsync(page, pageSize, null, status, startDate, endDate);
                    attendances = attendances.Where(a => subordinateIds.Contains(a.EmployeeId));
                    var totalCount = await _attendanceRepository.GetTotalCountAsync(null, status, startDate, endDate);
                    var response = _mapper.Map<IEnumerable<AttendanceResponseDTO>>(attendances);
                    return (response, totalCount);
                }
            }

            var allAttendances = await _attendanceRepository.GetAllAsync(page, pageSize, employeeId, status, startDate, endDate);
            var totalCountAll = await _attendanceRepository.GetTotalCountAsync(employeeId, status, startDate, endDate);
            var responseAll = _mapper.Map<IEnumerable<AttendanceResponseDTO>>(allAttendances);
            return (responseAll, totalCountAll);
        }

        public async Task<AttendanceResponseDTO> GetByIdAsync(Guid id, Guid currentUserId, string[] userRoles)
        {
            var attendance = await _attendanceRepository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Attendance with ID {id} not found.");

            if (userRoles.Contains("Employee") && attendance.EmployeeId != currentUserId)
                throw new UnauthorizedAccessException("Employees can only access their own attendance records.");

            if (userRoles.Contains("Manager"))
            {
                var subordinates = await _employeeRepository.GetSubordinatesAsync(currentUserId, 1, int.MaxValue);
                var subordinateIds = subordinates.Select(e => e.Id).ToList();
                if (!subordinateIds.Contains(attendance.EmployeeId))
                    throw new UnauthorizedAccessException("Managers can only access attendance records of their subordinates.");
            }

            return _mapper.Map<AttendanceResponseDTO>(attendance);
        }

        public async Task<AttendanceResponseDTO> CheckInAsync(
            AttendanceCheckInRequestDTO dto,
            Guid currentUserId,
            string[] userRoles)
        {
            if (!Enum.TryParse<AttendanceStatus>(dto.Status, true, out var status))
                throw new ArgumentException("Invalid attendance status.");

            var employeeId = userRoles.Contains("Employee")
                ? currentUserId
                : dto.EmployeeId ?? throw new ArgumentException("EmployeeId is required for Admin/HR.");

            var employee = await _employeeRepository.GetByIdAsync(employeeId)
                ?? throw new KeyNotFoundException($"Employee with ID {employeeId} not found.");

            if (await _attendanceRepository.HasAttendanceForDateAsync(employeeId, DateOnly.FromDateTime(DateTime.UtcNow.AddHours(5))))
                throw new ConflictException("Attendance record already exists for this employee today.");

            var today = DateOnly.FromDateTime(DateTime.UtcNow.AddHours(5));
            if (await _leaveRepository.HasOverlappingLeaveAsync(employeeId, today, today))
                throw new InvalidOperationException("Employee is on leave today");

            if (userRoles.Contains("Employee"))
            {
                var currentTime = TimeOnly.FromDateTime(DateTime.UtcNow.AddHours(5));
                TimeOnly referenceTime = new TimeOnly(9, 30); // 9:30 AM

                if (currentTime > referenceTime)
                    dto.Status = "Late";
                else
                    dto.Status = "Present";
            }

            var attendance = _mapper.Map<Attendance>(dto);
            attendance.EmployeeId = employeeId;

            var result = await _attendanceRepository.CreateAsync(attendance);

            return _mapper.Map<AttendanceResponseDTO>(result);
        }

        public async Task<AttendanceResponseDTO> CheckOutAsync(
            AttendanceCheckOutRequestDTO dto,
            Guid currentUserId,
            string[] userRoles)
        {
            var employeeId = userRoles.Contains("Employee")
                ? currentUserId
                : dto.EmployeeId ?? throw new ArgumentException("EmployeeId is required for Admin/HR.");

            var employee = await _employeeRepository.GetByIdAsync(employeeId)
                ?? throw new KeyNotFoundException($"Employee with ID {employeeId} not found.");

            var attendance = await _attendanceRepository.GetAttendanceForDateAsync(employeeId, DateOnly.FromDateTime(DateTime.UtcNow.AddHours(5)))
                ?? throw new KeyNotFoundException("No check-in record found for this employee today.");

            if (attendance.CheckOutTime.HasValue)
                throw new ConflictException("Check-out has already been recorded for this employee today.");

            var checkOutTime = TimeOnly.FromDateTime(DateTime.UtcNow).AddHours(5);
            if (checkOutTime <= attendance.CheckInTime)
                throw new InvalidOperationException("Check-out time must be after check-in time.");

            attendance.CheckOutTime = checkOutTime;
            attendance.UpdatedAt = DateTime.UtcNow.AddHours(5);
            await _attendanceRepository.UpdateAsync(attendance);

            return _mapper.Map<AttendanceResponseDTO>(attendance);
        }

        public async Task<AttendanceResponseDTO> UpdateAsync(
            Guid id,
            AttendanceUpdateRequestDTO dto,
            Guid currentUserId,
            string[] userRoles)
        {
            if (!userRoles.Contains("Admin") && !userRoles.Contains("HR"))
                throw new UnauthorizedAccessException("Only Admin and HR can update attendance records.");

            var attendance = await _attendanceRepository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Attendance with ID {id} not found.");

            if (dto.Date == null || dto.CheckInTime == null || dto.CheckOutTime == null || dto.Status == null)
                throw new ArgumentException("Date, CheckInTime, CheckOutTime, and Status are required for full update.");

            if (!Enum.TryParse<AttendanceStatus>(dto.Status, true, out _))
                throw new ArgumentException("Invalid attendance status.");

            if (dto.CheckInTime > dto.CheckOutTime)
                throw new ArgumentException("CheckInTime must be before or equal to CheckOutTime.");

            if (dto.Date != attendance.Date &&
                await _attendanceRepository.HasAttendanceForDateAsync(attendance.EmployeeId, dto.Date.Value))
                throw new ConflictException("Attendance record already exists for this employee on the specified date.");

            _mapper.Map(dto, attendance);
            await _attendanceRepository.UpdateAsync(attendance);

            return _mapper.Map<AttendanceResponseDTO>(attendance);
        }

        public async Task<AttendanceResponseDTO> PartialUpdateAsync(
            Guid id,
            AttendanceUpdateRequestDTO dto,
            Guid currentUserId,
            string[] userRoles)
        {
            if (!userRoles.Contains("Admin") && !userRoles.Contains("HR"))
                throw new UnauthorizedAccessException("Only Admin and HR can update attendance records.");

            var attendance = await _attendanceRepository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Attendance with ID {id} not found.");

            if (dto.Status != null && !Enum.TryParse<AttendanceStatus>(dto.Status, true, out _))
                throw new ArgumentException("Invalid attendance status.");

            if (dto.CheckInTime.HasValue &&
                dto.CheckOutTime.HasValue &&
                dto.CheckInTime.Value > dto.CheckOutTime.Value)
                throw new ArgumentException("CheckInTime must be before or equal to CheckOutTime.");

            if (dto.Date.HasValue &&
                dto.Date.Value != attendance.Date &&
                await _attendanceRepository.HasAttendanceForDateAsync(attendance.EmployeeId, dto.Date.Value))
                throw new ConflictException("Attendance record already exists for this employee on the specified date.");

            if (dto.Date.HasValue) attendance.Date = dto.Date.Value;
            if (dto.CheckInTime.HasValue) attendance.CheckInTime = dto.CheckInTime.Value;
            if (dto.CheckOutTime.HasValue) attendance.CheckOutTime = dto.CheckOutTime;
            if (dto.Status != null) attendance.Status = Enum.Parse<AttendanceStatus>(dto.Status);

            attendance.UpdatedAt = DateTime.UtcNow.AddHours(5);
            await _attendanceRepository.UpdateAsync(attendance);

            return _mapper.Map<AttendanceResponseDTO>(attendance);
        }

        public async Task DeleteAsync(Guid id, Guid currentUserId, string[] userRoles)
        {
            if (!userRoles.Contains("Admin") && !userRoles.Contains("HR"))
                throw new UnauthorizedAccessException("Only Admin and HR can delete attendance records.");

            if (!await _attendanceRepository.ExistsAsync(id))
                throw new KeyNotFoundException($"Attendance with ID {id} not found.");

            await _attendanceRepository.SoftDeleteAsync(id);
        }

        public async Task<(IEnumerable<AttendanceResponseDTO> Attendances, int TotalCount)> GetByEmployeeIdAsync(
            Guid employeeId,
            int page,
            int pageSize,
            string? status,
            DateOnly? startDate,
            DateOnly? endDate,
            Guid currentUserId,
            string[] userRoles)
        {
            var employee = await _employeeRepository.GetByIdAsync(employeeId)
                ?? throw new KeyNotFoundException($"Employee with ID {employeeId} not found.");

            if (userRoles.Contains("Employee") && employeeId != currentUserId)
                throw new UnauthorizedAccessException("Employees can only access their own attendance records.");

            if (userRoles.Contains("Manager"))
            {
                var subordinates = await _employeeRepository.GetSubordinatesAsync(currentUserId, 1, int.MaxValue);
                var subordinateIds = subordinates.Select(e => e.Id).ToList();
                if (!subordinateIds.Contains(employeeId) && employeeId != currentUserId)
                    throw new UnauthorizedAccessException("Managers can only access attendance records of their subordinates.");
            }

            var attendances = await _attendanceRepository.GetByEmployeeIdAsync(employeeId, page, pageSize, status, startDate, endDate);
            var totalCount = await _attendanceRepository.GetEmployeeAttendanceCountAsync(employeeId, status, startDate, endDate);
            var response = _mapper.Map<IEnumerable<AttendanceResponseDTO>>(attendances);

            return (response, totalCount);
        }
    }
}
