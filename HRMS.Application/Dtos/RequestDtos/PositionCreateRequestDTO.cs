using System.ComponentModel.DataAnnotations;

namespace HRMS.Dtos.RequestDtos
{
    public class PositionCreateRequestDTO
    {
        [Required]
        public string Title { get; set; }

        public string Description { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal SalaryRangeMin { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal SalaryRangeMax { get; set; }

        [Required]
        public Guid DepartmentId { get; set; }
    }
}
