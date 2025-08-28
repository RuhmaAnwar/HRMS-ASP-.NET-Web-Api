using System.ComponentModel.DataAnnotations;

namespace HRMS.Dtos.RequestDtos
{
    public class PositionUpdateRequestDTO
    {
        public string? Title { get; set; }

        public string? Description { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? SalaryRangeMin { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? SalaryRangeMax { get; set; }

        public Guid? DepartmentId { get; set; }
    }
}
