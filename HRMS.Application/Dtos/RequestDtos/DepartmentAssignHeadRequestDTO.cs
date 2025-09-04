
using System.ComponentModel.DataAnnotations;

namespace HRMS.Dtos.RequestDtos
{
    public class DepartmentAssignHeadRequestDTO
    {
        [Required]
        public string HeadEmployeeEmail { get; set; } = null!;
    }
}