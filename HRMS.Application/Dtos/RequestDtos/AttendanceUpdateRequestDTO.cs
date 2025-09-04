

namespace HRMS.Dtos.RequestDtos
{
    public class AttendanceUpdateRequestDTO
    {
        public DateOnly? Date { get; set; }
        public TimeOnly? CheckInTime { get; set; }
        public TimeOnly? CheckOutTime { get; set; }
        public string? Status { get; set; } // Maps to AttendanceStatus enum
    }
}
