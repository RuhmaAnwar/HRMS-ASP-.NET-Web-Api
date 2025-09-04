using HRMS.Dtos.RequestDtos;
using HRMS.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HRMS.Controllers.v1
{
    [Route("api/v1/leaves")]
    [ApiController]
    [Authorize]
    public class LeavesControllerV1 : ControllerBase
    {
        private readonly ILeaveService _leaveService;

        public LeavesControllerV1(ILeaveService leaveService)
        {
            _leaveService = leaveService;
        }

        [HttpGet]
        public async Task<IActionResult> GetLeaves(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] Guid? employeeId = null,
            [FromQuery] string? status = null,
            [FromQuery] DateOnly? startDate = null,
            [FromQuery] DateOnly? endDate = null)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var userRoles = User.FindAll(ClaimTypes.Role).Select(r => r.Value).ToArray();

            var (leaves, totalCount) = await _leaveService.GetAllAsync(page, pageSize, employeeId, status, startDate, endDate, userId, userRoles);
            return Ok(new { totalCount, leaves });
        }

        [HttpGet("{id:Guid}")]
        public async Task<IActionResult> GetLeaveById(Guid id)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var userRoles = User.FindAll(ClaimTypes.Role).Select(r => r.Value).ToArray();

            var leave = await _leaveService.GetByIdAsync(id, userId, userRoles);
            return Ok(leave);
        }

        [HttpPost]
        public async Task<IActionResult> CreateLeave([FromBody] LeaveCreateRequestDTO dto)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var userRoles = User.FindAll(ClaimTypes.Role).Select(r => r.Value).ToArray();

            var leave = await _leaveService.CreateAsync(dto, userId, userRoles);
            return CreatedAtAction(nameof(GetLeaveById), new { id = leave.Id }, leave);
        }

        [HttpPut("{id:Guid}")]
        public async Task<IActionResult> UpdateLeave(Guid id, [FromBody] LeaveUpdateRequestDTO dto)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var userRoles = User.FindAll(ClaimTypes.Role).Select(r => r.Value).ToArray();

            var leave = await _leaveService.UpdateAsync(id, dto, userId, userRoles);
            return Ok(leave);
        }

        [HttpPatch("{id:Guid}")]
        public async Task<IActionResult> PartialUpdateLeave(Guid id, [FromBody] LeaveUpdateRequestDTO dto)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var userRoles = User.FindAll(ClaimTypes.Role).Select(r => r.Value).ToArray();

            var leave = await _leaveService.PartialUpdateAsync(id, dto, userId, userRoles);
            return Ok(leave);
        }

        [HttpDelete("{id:Guid}")]
        public async Task<IActionResult> DeleteLeave(Guid id)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var userRoles = User.FindAll(ClaimTypes.Role).Select(r => r.Value).ToArray();

            await _leaveService.DeleteAsync(id, userId, userRoles);
            return NoContent();
        }

        [HttpPatch("{id:Guid}/approve")]
        [Authorize(Roles = "Admin,HR,Manager")]
        public async Task<IActionResult> ApproveLeave(Guid id, [FromBody] LeaveApprovalRequestDTO dto)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var userRoles = User.FindAll(ClaimTypes.Role).Select(r => r.Value).ToArray();

            var leave = await _leaveService.ApproveAsync(id, dto, userId, userRoles);
            return Ok(leave);
        }

        [HttpGet("employee/{employeeId:Guid}")]
        public async Task<IActionResult> GetEmployeeLeaves(
            Guid employeeId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? status = null,
            [FromQuery] DateOnly? startDate = null,
            [FromQuery] DateOnly? endDate = null)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var userRoles = User.FindAll(ClaimTypes.Role).Select(r => r.Value).ToArray();

            var (leaves, totalCount) = await _leaveService.GetByEmployeeIdAsync(employeeId, page, pageSize, status, startDate, endDate, userId, userRoles);
            return Ok(new { totalCount, leaves });
        }
    }
}
