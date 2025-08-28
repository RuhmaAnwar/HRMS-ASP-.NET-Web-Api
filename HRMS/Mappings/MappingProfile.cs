using AutoMapper;
using HRMS.Dtos.RequestDtos;
using HRMS.Dtos.ResponseDtos;
using HRMS.Enums;
using HRMS.Models;

namespace HRMS.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Employee to EmployeeResponseDto
            CreateMap<Employee, EmployeeResponseDto>()
                .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => src.Department != null ? src.Department.Name : null))
                .ForMember(dest => dest.PositionTitle, opt => opt.MapFrom(src => src.Position != null ? src.Position.Title : null))
                .ForMember(dest => dest.ManagerName, opt => opt.MapFrom(src => src.Manager != null ? $"{src.Manager.FirstName} {src.Manager.LastName}" : null));

            // EmployeeCreateRequestDto to Employee
            CreateMap<EmployeeCreateRequestDto, Employee>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow.AddHours(5)))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow.AddHours(5)))
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => false))
                .ForMember(dest => dest.TotalLeaves, opt => opt.MapFrom(src => 25))
                .ForMember(dest => dest.LeavesUsed, opt => opt.MapFrom(src => 0))
                .ForMember(dest => dest.Subordinates, opt => opt.Ignore())
                .ForMember(dest => dest.Department, opt => opt.Ignore())
                .ForMember(dest => dest.Position, opt => opt.Ignore())
                .ForMember(dest => dest.Manager, opt => opt.Ignore())
                .ForMember(dest => dest.DepartmentLed, opt => opt.Ignore())
                .ForMember(dest => dest.Attendances, opt => opt.Ignore())
                .ForMember(dest => dest.LeaveRequests, opt => opt.Ignore())
                .ForMember(dest => dest.ApprovedLeaves, opt => opt.Ignore());

            // EmployeeUpdateRequestDto to Employee
            CreateMap<EmployeeUpdateRequestDto, Employee>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow.AddHours(5)))
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.Subordinates, opt => opt.Ignore())
                .ForMember(dest => dest.Department, opt => opt.Ignore())
                .ForMember(dest => dest.Position, opt => opt.Ignore())
                .ForMember(dest => dest.Manager, opt => opt.Ignore())
                .ForMember(dest => dest.DepartmentLed, opt => opt.Ignore())
                .ForMember(dest => dest.Attendances, opt => opt.Ignore())
                .ForMember(dest => dest.LeaveRequests, opt => opt.Ignore())
                .ForMember(dest => dest.ApprovedLeaves, opt => opt.Ignore());

            // Employee to LeaveBalanceResponseDto
            CreateMap<Employee, LeaveBalanceResponseDto>();

            // RegisterRequestDto to Employee
            CreateMap<RegisterRequestDto, Employee>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow.AddHours(5)))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow.AddHours(5)))
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => false))
                .ForMember(dest => dest.Subordinates, opt => opt.Ignore())
                .ForMember(dest => dest.Department, opt => opt.Ignore())
                .ForMember(dest => dest.Position, opt => opt.Ignore())
                .ForMember(dest => dest.Manager, opt => opt.Ignore())
                .ForMember(dest => dest.DepartmentLed, opt => opt.Ignore())
                .ForMember(dest => dest.Attendances, opt => opt.Ignore())
                .ForMember(dest => dest.LeaveRequests, opt => opt.Ignore())
                .ForMember(dest => dest.ApprovedLeaves, opt => opt.Ignore());

            // Department to DepartmentResponseDTO
            CreateMap<Department, DepartmentResponseDTO>()
               .ForMember(dest => dest.HeadName, opt => opt.MapFrom(src => src.HeadEmployee != null ? $"{src.HeadEmployee.FirstName} {src.HeadEmployee.LastName}" : null));

            // DepartmentCreateRequestDto to Department
            CreateMap<DepartmentCreateRequestDTO, Department>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
                .ForMember(dest => dest.HeadEmployeeId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow.AddHours(5)))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow.AddHours(5)))
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => false));

            // DepartmentUpdateRequestDTO to Department (for PUT/PATCH)
            CreateMap<DepartmentUpdateRequestDTO, Department>()
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow.AddHours(5)))
                .ForMember(dest => dest.HeadEmployeeId, opt => opt.Ignore());

            // Position to PositionResponseDTO
            CreateMap<Position, PositionResponseDTO>()
                .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => src.Department != null ? src.Department.Name : null));

            // PositionCreateRequestDTO to Position
            CreateMap<PositionCreateRequestDTO, Position>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow.AddHours(5)))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow.AddHours(5)))
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => false))
                .ForMember(dest => dest.Department, opt => opt.Ignore())
                .ForMember(dest => dest.Employees, opt => opt.Ignore());

            // PositionUpdateRequestDTO to Position
            CreateMap<PositionUpdateRequestDTO, Position>()
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow.AddHours(5)))
                .ForMember(dest => dest.Department, opt => opt.Ignore())
                .ForMember(dest => dest.Employees, opt => opt.Ignore());

            // Leave to LeaveResponseDTO
            CreateMap<Leave, LeaveResponseDTO>()
                .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => src.Employee != null ? $"{src.Employee.FirstName} {src.Employee.LastName}" : null))
                .ForMember(dest => dest.ApproverName, opt => opt.MapFrom(src => src.Approver != null ? $"{src.Approver.FirstName} {src.Approver.LastName}" : null));

            // LeaveCreateRequestDTO to Leave
            CreateMap<LeaveCreateRequestDTO, Leave>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow.AddHours(5)))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow.AddHours(5)))
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => false))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => LeaveStatus.Pending))
                .ForMember(dest => dest.EmployeeId, opt => opt.Ignore())
                .ForMember(dest => dest.Employee, opt => opt.Ignore())
                .ForMember(dest => dest.Approver, opt => opt.Ignore());

            // LeaveUpdateRequestDTO to Leave
            CreateMap<LeaveUpdateRequestDTO, Leave>()
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow.AddHours(5)))
                .ForMember(dest => dest.EmployeeId, opt => opt.Ignore())
                .ForMember(dest => dest.Employee, opt => opt.Ignore())
                .ForMember(dest => dest.Approver, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore());

            // LeaveApprovalRequestDTO to Leave
            CreateMap<LeaveApprovalRequestDTO, Leave>()
                .ForMember(dest => dest.Reason, opt => opt.MapFrom(src => src.Comments))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow.AddHours(5)));

            // Attendance to AttendanceResponseDTO
            CreateMap<Attendance, AttendanceResponseDTO>()
                .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => src.Employee != null ? $"{src.Employee.FirstName} {src.Employee.LastName}" : null));

            // AttendanceCheckInRequestDTO to Attendance
            CreateMap<AttendanceCheckInRequestDTO, Attendance>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
                .ForMember(dest => dest.Date, opt => opt.MapFrom(src =>DateOnly.FromDateTime(DateTime.UtcNow.AddHours(5))))
                .ForMember(dest => dest.CheckInTime, opt => opt.MapFrom(src => TimeOnly.FromDateTime(DateTime.UtcNow).AddHours(5)))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow.AddHours(5)))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow.AddHours(5)))
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => false))
                .ForMember(dest => dest.EmployeeId, opt => opt.Ignore())
                .ForMember(dest => dest.Employee, opt => opt.Ignore())
                .ForMember(dest => dest.CheckOutTime, opt => opt.Ignore());

            // AttendanceUpdateRequestDTO to Attendance
            CreateMap<AttendanceUpdateRequestDTO, Attendance>()
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow.AddHours(5)))
                .ForMember(dest => dest.EmployeeId, opt => opt.Ignore())
                .ForMember(dest => dest.Employee, opt => opt.Ignore());


        }
    }
}
