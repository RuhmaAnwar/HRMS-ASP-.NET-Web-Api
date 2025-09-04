
namespace HRMS.Dtos.ResponseDtos
{
    public class PositionResponseDTO
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal SalaryRangeMin { get; set; }
        public decimal SalaryRangeMax { get; set; }
        public string? DepartmentName { get; set; }
    }
}