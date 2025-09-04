using HRMS.Enums;
using HRMS.Models;
using HRMS.Repositories.Interfaces;
using Quartz;

namespace HRMS.Jobs
{
    public class AutoLeaveJob : IJob
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IAttendanceRepository _attendanceRepository;
        private readonly ILeaveRepository _leaveRepository;

        public AutoLeaveJob(
            IEmployeeRepository employeeRepository,
            IAttendanceRepository attendanceRepository,
            ILeaveRepository leaveRepository)
        {
            _employeeRepository = employeeRepository;
            _attendanceRepository = attendanceRepository;
            _leaveRepository = leaveRepository;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow.AddHours(5));

            var employees = await _employeeRepository.GetAllAsync(1,10,null,null,null);

            foreach (var employee in employees)
            {
                var isOnLeave = await _leaveRepository.HasOverlappingLeaveAsync(employee.Id, today, today);
                var hasCheckedIn = await _attendanceRepository.HasAttendanceForDateAsync(employee.Id, today);
                
                TimeSpan tenAM = new TimeSpan(10, 0, 0);
                TimeSpan currentTime = DateTime.Now.AddHours(5).TimeOfDay;

                if (!hasCheckedIn && !isOnLeave)
                {
                    var leave = new Leave
                    {
                        Id = Guid.NewGuid(),
                        EmployeeId = employee.Id,
                        StartDate = today,
                        EndDate = today,
                        Type = LeaveType.Other,
                        Status = LeaveStatus.Approved,
                        ApproverId = null,
                        Reason = "Auto-generated due to no check-in by 10:00 AM PKT",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        IsDeleted = false
                    };
                    await _leaveRepository.CreateAsync(leave);
                }
            }
        }
    }
}
