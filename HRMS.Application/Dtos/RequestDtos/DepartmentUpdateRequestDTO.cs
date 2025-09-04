
using System.ComponentModel.DataAnnotations;

namespace HRMS.Dtos.RequestDtos
{
    public class DepartmentUpdateRequestDTO
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? HeadEmployeeEmail { get; set; }
    }
}