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
    public class LeaveService : ILeaveService
    {
        private readonly ILeaveRepository _leaveRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IMapper _mapper;

        public LeaveService(
            ILeaveRepository leaveRepository,
            IEmployeeRepository employeeRepository,
            IMapper mapper)
        {
            _leaveRepository = leaveRepository;
            _employeeRepository = employeeRepository;
            _mapper = mapper;
        }

        public async Task<(IEnumerable<LeaveResponseDTO> Leaves, int TotalCount)> GetAllAsync(
            int page, int pageSize, Guid? employeeId, string? status, DateOnly? startDate, DateOnly? endDate,
            Guid currentUserId, string[] userRoles)
        {
            if (userRoles.Contains("Employee"))
                throw new UnauthorizedAccessException("Employees cannot access all leave requests.");

            if (userRoles.Contains("Manager"))
            {
                var subordinates = await _employeeRepository.GetSubordinatesAsync(currentUserId, page, pageSize);
                var subordinateIds = subordinates.Select(e => e.Id).ToList();
                employeeId = employeeId.HasValue && subordinateIds.Contains(employeeId.Value) ? employeeId : null;
                if (!employeeId.HasValue)
                {
                    // Managers can only see leaves they need to approve or their subordinates' leaves
                    var leaves = await _leaveRepository.GetAllAsync(page, pageSize, null, status, startDate, endDate);
                    leaves = leaves.Where(l => l.ApproverId == currentUserId || subordinateIds.Contains(l.EmployeeId));
                    var totalCount = await _leaveRepository.GetTotalCountAsync(null, status, startDate, endDate);
                    var response = _mapper.Map<IEnumerable<LeaveResponseDTO>>(leaves);
                    return (response, totalCount);
                }
            }

            var allLeaves = await _leaveRepository.GetAllAsync(page, pageSize, employeeId, status, startDate, endDate);
            var totalCountAll = await _leaveRepository.GetTotalCountAsync(employeeId, status, startDate, endDate);
            var responseAll = _mapper.Map<IEnumerable<LeaveResponseDTO>>(allLeaves);
            return (responseAll, totalCountAll);
        }

        public async Task<LeaveResponseDTO> GetByIdAsync(Guid id, Guid currentUserId, string[] userRoles)
        {
            var leave = await _leaveRepository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Leave with ID {id} not found.");

            if (userRoles.Contains("Employee") && leave.EmployeeId != currentUserId)
                throw new UnauthorizedAccessException("Employees can only access their own leave requests.");

            if (userRoles.Contains("Manager"))
            {
                var subordinates = await _employeeRepository.GetSubordinatesAsync(currentUserId, 1, 10);
                var subordinateIds = subordinates.Select(e => e.Id).ToList();
                if (leave.EmployeeId != currentUserId && leave.ApproverId != currentUserId && !subordinateIds.Contains(leave.EmployeeId))
                    throw new UnauthorizedAccessException("Managers can only access leaves of their subordinates or leaves they need to approve.");
            }

            return _mapper.Map<LeaveResponseDTO>(leave);
        }

        public async Task<LeaveResponseDTO> CreateAsync(LeaveCreateRequestDTO dto, Guid currentUserId, string[] userRoles)
        {
            var (isValidLeave, message) = CheckLeaveValidity(dto);

            if(!isValidLeave)
                throw new ValidationException(message);

            var employeeId = userRoles.Contains("Employee") ? currentUserId : dto.ApproverId ?? currentUserId;
            var employee = await _employeeRepository.GetByIdAsync(employeeId)
                ?? throw new KeyNotFoundException($"Employee with ID {employeeId} not found.");

            if (dto.ApproverId.HasValue && !await _employeeRepository.ManagerExistsAsync(dto.ApproverId))
                throw new KeyNotFoundException($"Approver with ID {dto.ApproverId} not found.");

            var leaveDays = dto.EndDate.DayNumber - dto.StartDate.DayNumber + 1;
            if (employee.TotalLeaves - employee.LeavesUsed < leaveDays)
                throw new InvalidOperationException("Insufficient leave balance.");

            if (await _leaveRepository.HasOverlappingLeaveAsync(employeeId, dto.StartDate, dto.EndDate))
                throw new ConflictException("Overlapping leave request exists.");

            var leave = _mapper.Map<Leave>(dto);
            leave.EmployeeId = employeeId;
            var result = await _leaveRepository.CreateAsync(leave);
            return _mapper.Map<LeaveResponseDTO>(result);
        }

        public (bool isValid, string message) CheckLeaveValidity(LeaveCreateRequestDTO dto)
        {
            if (dto.StartDate > dto.EndDate)
                throw new InvalidOperationException("StartDate must be before or equal to EndDate.");

            if (!Enum.TryParse<LeaveType>(dto.Type, true, out _))
                throw new InvalidOperationException("Invalid leave type.");

            DateOnly currentDate = DateOnly.FromDateTime(DateTime.Now);
            var daysUntilLeave = dto.StartDate.DayNumber - currentDate.DayNumber + 1;
            var leaveDays = dto.EndDate.DayNumber - dto.StartDate.DayNumber + 1;

            if (daysUntilLeave > 60)
                return (false, "Cannot apply leave for after more than 2 months");

            if (Enum.Parse<LeaveType>(dto.Type) == LeaveType.Sick && daysUntilLeave > 2)
                return (false, "Cannot apply sick leave for after more than one day");

            if (Enum.Parse<LeaveType>(dto.Type) == LeaveType.Casual && leaveDays > 2)
                return (false, "Cannot apply casual leave for more than 2 days at once");

            return (true, "valid leave format");
        }

        public async Task<LeaveResponseDTO> UpdateAsync(Guid id, LeaveUpdateRequestDTO dto, Guid currentUserId, string[] userRoles)
        {
            var leave = await _leaveRepository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Leave with ID {id} not found.");

            if (leave.Status != LeaveStatus.Pending)
                throw new InvalidOperationException("Only pending leaves can be updated.");

            if (userRoles.Contains("Employee") && leave.EmployeeId != currentUserId)
                throw new UnauthorizedAccessException("Employees can only update their own leave requests.");

            if (dto.StartDate.HasValue && dto.EndDate.HasValue && dto.StartDate.Value > dto.EndDate.Value)
                throw new ArgumentException("StartDate must be before or equal to EndDate.");

            if (dto.Type != null && !Enum.TryParse<LeaveType>(dto.Type, true, out _))
                throw new ArgumentException("Invalid leave type.");

            if (dto.ApproverId.HasValue && !await _employeeRepository.ManagerExistsAsync(dto.ApproverId))
                throw new KeyNotFoundException($"Approver with ID {dto.ApproverId} not found.");

            var employee = await _employeeRepository.GetByIdAsync(leave.EmployeeId)
                ?? throw new KeyNotFoundException($"Employee with ID {leave.EmployeeId} not found.");

            DateOnly startDate = dto.StartDate ?? leave.StartDate;
            DateOnly endDate = dto.EndDate ?? leave.EndDate;

            int leaveDays = endDate.DayNumber - startDate.DayNumber + 1;

            if (employee.TotalLeaves - employee.LeavesUsed < leaveDays)
                throw new InvalidOperationException("Insufficient leave balance.");

            if (await _leaveRepository.HasOverlappingLeaveAsync(
                leave.EmployeeId,
                dto.StartDate ?? leave.StartDate,
                dto.EndDate ?? leave.EndDate,
                id))
            {
                throw new ConflictException("Overlapping leave request exists.");
            }

            _mapper.Map(dto, leave);
            await _leaveRepository.UpdateAsync(leave);

            return _mapper.Map<LeaveResponseDTO>(leave);
        }

        public async Task<LeaveResponseDTO> PartialUpdateAsync(Guid id, LeaveUpdateRequestDTO dto, Guid currentUserId, string[] userRoles)
        {
            var leave = await _leaveRepository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Leave with ID {id} not found.");

            if (leave.Status != LeaveStatus.Pending)
                throw new InvalidOperationException("Only pending leaves can be updated.");

            if (userRoles.Contains("Employee") && leave.EmployeeId != currentUserId)
                throw new UnauthorizedAccessException("Employees can only update their own leave requests.");

            if (dto.StartDate.HasValue && dto.EndDate.HasValue && dto.StartDate.Value > dto.EndDate.Value)
                throw new ArgumentException("StartDate must be before or equal to EndDate.");

            if (dto.Type != null && !Enum.TryParse<LeaveType>(dto.Type, true, out _))
                throw new ArgumentException("Invalid leave type.");

            if (dto.ApproverId.HasValue && !await _employeeRepository.ManagerExistsAsync(dto.ApproverId))
                throw new KeyNotFoundException($"Approver with ID {dto.ApproverId} not found.");

            var employee = await _employeeRepository.GetByIdAsync(leave.EmployeeId)
                ?? throw new KeyNotFoundException($"Employee with ID {leave.EmployeeId} not found.");

            var newStartDate = dto.StartDate ?? leave.StartDate;
            var newEndDate = dto.EndDate ?? leave.EndDate;
            if (dto.StartDate.HasValue || dto.EndDate.HasValue)
            {
                var leaveDays = newEndDate.DayNumber - newStartDate.DayNumber + 1;
                if (employee.TotalLeaves - employee.LeavesUsed < leaveDays)
                    throw new InvalidOperationException("Insufficient leave balance.");

                if (await _leaveRepository.HasOverlappingLeaveAsync(leave.EmployeeId, newStartDate, newEndDate, id))
                    throw new ConflictException("Overlapping leave request exists.");
            }

            if (dto.StartDate.HasValue) leave.StartDate = dto.StartDate.Value;
            if (dto.EndDate.HasValue) leave.EndDate = dto.EndDate.Value;
            if (dto.Type != null) leave.Type = Enum.Parse<LeaveType>(dto.Type);
            if (dto.Reason != null) leave.Reason = dto.Reason;
            if (dto.ApproverId.HasValue) leave.ApproverId = dto.ApproverId;

            leave.UpdatedAt = DateTime.UtcNow;
            await _leaveRepository.UpdateAsync(leave);

            return _mapper.Map<LeaveResponseDTO>(leave);
        }

        public async Task DeleteAsync(Guid id, Guid currentUserId, string[] userRoles)
        {

            var leave = await _leaveRepository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Leave with ID {id} not found.");

            if (userRoles.Contains("Employee") && leave.EmployeeId != currentUserId)
                throw new UnauthorizedAccessException("Employees can only delete their own leave requests.");

            if (leave.Status != LeaveStatus.Pending)
                throw new InvalidOperationException("Only pending leaves can be deleted.");

            await _leaveRepository.SoftDeleteAsync(id);
        }

        public async Task<LeaveResponseDTO> ApproveAsync(Guid id, LeaveApprovalRequestDTO dto, Guid currentUserId, string[] userRoles)
        {
            var leave = await _leaveRepository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Leave with ID {id} not found.");

            if (leave.Status != LeaveStatus.Pending)
                throw new InvalidOperationException("Only pending leaves can be approved or rejected.");

            if (userRoles.Contains("Employee"))
                throw new UnauthorizedAccessException("Employees cannot approve or reject leave requests.");

            if (userRoles.Contains("Manager") && leave.ApproverId != currentUserId)
                throw new UnauthorizedAccessException("Managers can only approve or reject leaves assigned to them.");

            if (!Enum.TryParse<LeaveStatus>(dto.Status, true, out var newStatus) || newStatus == LeaveStatus.Pending)
                throw new ArgumentException("Invalid or pending leave status.");

            var employee = await _employeeRepository.GetByIdAsync(leave.EmployeeId)
                ?? throw new KeyNotFoundException($"Employee with ID {leave.EmployeeId} not found.");

            if (newStatus == LeaveStatus.Approved)
            {
                var leaveDays = leave.EndDate.DayNumber - leave.StartDate.DayNumber + 1;
                if (employee.TotalLeaves - employee.LeavesUsed < leaveDays)
                    throw new InvalidOperationException("Insufficient leave balance.");

                //await _employeeRepository.UpdateLeaveBalanceAsync(leave.EmployeeId, employee.LeavesUsed + leaveDays);
                employee.LeavesUsed += leaveDays;
                await _employeeRepository.SaveChangesAsync();

            }

            leave.Status = newStatus;
            leave.ApproverId = currentUserId;
            _mapper.Map(dto, leave); // Maps Comments to Reason
            await _leaveRepository.ApproveAsync(leave);

            return _mapper.Map<LeaveResponseDTO>(leave);
        }

        public async Task<(IEnumerable<LeaveResponseDTO> Leaves, int TotalCount)> GetByEmployeeIdAsync(
            Guid employeeId, int page, int pageSize, string? status, DateOnly? startDate, DateOnly? endDate,
            Guid currentUserId, string[] userRoles)
        {
            var employee = await _employeeRepository.GetByIdAsync(employeeId)
                ?? throw new KeyNotFoundException($"Employee with ID {employeeId} not found.");

            if (userRoles.Contains("Employee") && employeeId != currentUserId)
                throw new UnauthorizedAccessException("Employees can only access their own leave requests.");

            if (userRoles.Contains("Manager"))
            {
                var subordinates = await _employeeRepository.GetSubordinatesAsync(currentUserId, 1, int.MaxValue);
                var subordinateIds = subordinates.Select(e => e.Id).ToList();
                if (!subordinateIds.Contains(employeeId) && employeeId != currentUserId)
                    throw new UnauthorizedAccessException("Managers can only access leaves of their subordinates.");
            }

            var leaves = await _leaveRepository.GetByEmployeeIdAsync(employeeId, page, pageSize, status, startDate, endDate);
            var totalCount = await _leaveRepository.GetEmployeeLeavesCountAsync(employeeId, status, startDate, endDate);
            var response = _mapper.Map<IEnumerable<LeaveResponseDTO>>(leaves);
            return (response, totalCount);
        }
    }
}
