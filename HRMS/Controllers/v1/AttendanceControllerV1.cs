using HRMS.Dtos.RequestDtos;
using HRMS.Dtos.ResponseDtos;
using HRMS.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HRMS.Controllers.v1
{
    [Route("api/v1/attendances")]
    [ApiController]
    [Authorize]
    public class AttendancesControllerV1 : ControllerBase
    {
        private readonly IAttendanceService _attendanceService;

        public AttendancesControllerV1(IAttendanceService attendanceService)
        {
            _attendanceService = attendanceService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAttendances(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] Guid? employeeId = null,
            [FromQuery] string? status = null,
            [FromQuery] DateOnly? startDate = null,
            [FromQuery] DateOnly? endDate = null)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var userRoles = User.FindAll(ClaimTypes.Role).Select(r => r.Value).ToArray();

            var (attendances, totalCount) = await _attendanceService.GetAllAsync(
                page, pageSize, employeeId, status, startDate, endDate, userId, userRoles);

            return Ok(new { totalCount, attendances });
        }

        [HttpGet("{id:Guid}")]
        public async Task<IActionResult> GetAttendanceById(Guid id)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var userRoles = User.FindAll(ClaimTypes.Role).Select(r => r.Value).ToArray();

            var attendance = await _attendanceService.GetByIdAsync(id, userId, userRoles);
            return Ok(attendance);
        }

        [HttpPost("checkin")]
        public async Task<IActionResult> CheckIn([FromBody] AttendanceCheckInRequestDTO dto)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var userRoles = User.FindAll(ClaimTypes.Role).Select(r => r.Value).ToArray();

            var attendance = await _attendanceService.CheckInAsync(dto, userId, userRoles);
            return CreatedAtAction(nameof(GetAttendanceById), new { id = attendance.Id }, attendance);
        }

        [HttpPost("checkout")]
        public async Task<IActionResult> CheckOut([FromBody] AttendanceCheckOutRequestDTO dto)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var userRoles = User.FindAll(ClaimTypes.Role).Select(r => r.Value).ToArray();

            var attendance = await _attendanceService.CheckOutAsync(dto, userId, userRoles);
            return Ok(attendance);
        }

        [HttpPut("{id:Guid}")]
        [Authorize(Roles = "Admin,HR")]
        public async Task<IActionResult> UpdateAttendance(Guid id, [FromBody] AttendanceUpdateRequestDTO dto)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var userRoles = User.FindAll(ClaimTypes.Role).Select(r => r.Value).ToArray();

            var attendance = await _attendanceService.UpdateAsync(id, dto, userId, userRoles);
            return Ok(attendance);
        }

        [HttpPatch("{id:Guid}")]
        [Authorize(Roles = "Admin,HR")]
        public async Task<IActionResult> PartialUpdateAttendance(Guid id, [FromBody] AttendanceUpdateRequestDTO dto)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var userRoles = User.FindAll(ClaimTypes.Role).Select(r => r.Value).ToArray();

            var attendance = await _attendanceService.PartialUpdateAsync(id, dto, userId, userRoles);
            return Ok(attendance);
        }

        [HttpDelete("{id:Guid}")]
        [Authorize(Roles = "Admin,HR")]
        public async Task<IActionResult> DeleteAttendance(Guid id)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var userRoles = User.FindAll(ClaimTypes.Role).Select(r => r.Value).ToArray();

            await _attendanceService.DeleteAsync(id, userId, userRoles);
            return NoContent();
        }

        [HttpGet("employee/{employeeId:Guid}")]
        public async Task<IActionResult> GetEmployeeAttendances(
            Guid employeeId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? status = null,
            [FromQuery] DateOnly? startDate = null,
            [FromQuery] DateOnly? endDate = null)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var userRoles = User.FindAll(ClaimTypes.Role).Select(r => r.Value).ToArray();

            var (attendances, totalCount) = await _attendanceService.GetByEmployeeIdAsync(
                employeeId, page, pageSize, status, startDate, endDate, userId, userRoles);

            return Ok(new { totalCount, attendances });
        }
    }
}
