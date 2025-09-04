using HRMS.Dtos.RequestDtos;
using HRMS.Dtos.ResponseDtos;
using System;
using System.Threading.Tasks;

namespace HRMS.Services.Interfaces
{
    public interface IAttendanceService
    {
        Task<(IEnumerable<AttendanceResponseDTO> Attendances, int TotalCount)> GetAllAsync(
            int page,
            int pageSize,
            Guid? employeeId,
            string? status,
            DateOnly? startDate,
            DateOnly? endDate,
            Guid currentUserId,
            string[] userRoles);

        Task<AttendanceResponseDTO> GetByIdAsync(
            Guid id,
            Guid currentUserId,
            string[] userRoles);

        Task<AttendanceResponseDTO> CheckInAsync(
            AttendanceCheckInRequestDTO dto,
            Guid currentUserId,
            string[] userRoles);

        Task<AttendanceResponseDTO> CheckOutAsync(
            AttendanceCheckOutRequestDTO dto,
            Guid currentUserId,
            string[] userRoles);

        Task<AttendanceResponseDTO> UpdateAsync(
            Guid id,
            AttendanceUpdateRequestDTO dto,
            Guid currentUserId,
            string[] userRoles);

        Task<AttendanceResponseDTO> PartialUpdateAsync(
            Guid id,
            AttendanceUpdateRequestDTO dto,
            Guid currentUserId,
            string[] userRoles);

        Task DeleteAsync(
            Guid id,
            Guid currentUserId,
            string[] userRoles);

        Task<(IEnumerable<AttendanceResponseDTO> Attendances, int TotalCount)> GetByEmployeeIdAsync(
            Guid employeeId,
            int page,
            int pageSize,
            string? status,
            DateOnly? startDate,
            DateOnly? endDate,
            Guid currentUserId,
            string[] userRoles);
    }
}
