using HRMS.Dtos.RequestDtos;
using HRMS.Dtos.ResponseDtos;
using HRMS.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HRMS.Controllers.v1
{
    [Route("api/v1/positions")]
    [ApiController]
    [Authorize]
    public class PositionsControllerV1 : ControllerBase
    {
        private readonly IPositionService _positionService;

        public PositionsControllerV1(IPositionService positionService)
        {
            _positionService = positionService;
        }

        [HttpGet]
        public async Task<IActionResult> GetPositions(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] Guid? departmentId = null,
            [FromQuery] string? search = null)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var userRoles = User.FindAll(ClaimTypes.Role).Select(r => r.Value).ToArray();

            var (positions, totalCount) = await _positionService.GetAllAsync(page, pageSize, departmentId, search, userId, userRoles);
            return Ok(new { totalCount, positions });
        }

        [HttpGet("{id:Guid}")]
        public async Task<IActionResult> GetPositionById(Guid id)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var userRoles = User.FindAll(ClaimTypes.Role).Select(r => r.Value).ToArray();

            var position = await _positionService.GetByIdAsync(id, userId, userRoles);
            return Ok(position);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,HR")]
        public async Task<IActionResult> CreatePosition([FromBody] PositionCreateRequestDTO dto)
        {
            var position = await _positionService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetPositionById), new { id = position.Id }, position);
        }

        [HttpPut("{id:Guid}")]
        [Authorize(Roles = "Admin,HR")]
        public async Task<IActionResult> UpdatePosition(Guid id, [FromBody] PositionUpdateRequestDTO dto)
        {
            var position = await _positionService.UpdateAsync(id, dto);
            return Ok(position);
        }

        [HttpPatch("{id:Guid}")]
        [Authorize(Roles = "Admin,HR")]
        public async Task<IActionResult> PartialUpdatePosition(Guid id, [FromBody] PositionUpdateRequestDTO dto)
        {
            var position = await _positionService.PartialUpdateAsync(id, dto);
            return Ok(position);
        }

        [HttpDelete("{id:Guid}")]
        [Authorize(Roles = "Admin,HR")]
        public async Task<IActionResult> DeletePosition(Guid id)
        {
            await _positionService.DeleteAsync(id);
            return NoContent();
        }

        [HttpGet("{id:Guid}/employees")]
        public async Task<IActionResult> GetPositionEmployees(
            Guid id,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var userRoles = User.FindAll(ClaimTypes.Role).Select(r => r.Value).ToArray();

            var (employees, totalCount) = await _positionService.GetEmployeesAsync(id, page, pageSize, userId, userRoles);
            return Ok(new { totalCount, employees });
        }
    }
}
