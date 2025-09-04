
namespace HRMS.Dtos.RequestDtos
{
    public class AttendanceCheckInRequestDTO
    {
        public Guid? EmployeeId { get; set; } 
        public string Status { get; set; } = "Present"; 
    }
}
