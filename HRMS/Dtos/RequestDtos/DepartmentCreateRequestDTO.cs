using System.ComponentModel.DataAnnotations.Schema;

namespace HRMS.Dtos.RequestDtos
{
    public class DepartmentCreateRequestDTO
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string? HeadEmployeeEmail { get; set; }
    }
}
