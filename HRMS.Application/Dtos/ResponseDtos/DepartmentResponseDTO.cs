using System.ComponentModel.DataAnnotations.Schema;

namespace HRMS.Dtos.ResponseDtos
{
    public class DepartmentResponseDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string HeadName { get; set; }
    }
}
