using System.ComponentModel.DataAnnotations.Schema;
using HRMS.Enums;

namespace HRMS.Models
{
    public class Attendance
    {

        [Column("id")]
        public Guid Id { get; set; }

        [Column("employee_id")]
        public Guid EmployeeId { get; set; }

        [Column("date")]
        public DateOnly Date { get; set; }

        [Column("check_in_time")]
        public TimeOnly? CheckInTime { get; set; }

        [Column("check_out_time")]
        public TimeOnly? CheckOutTime { get; set; }

        [Column("status")]
        public AttendanceStatus Status { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }

        [Column("is_deleted")]
        public bool IsDeleted { get; set; }

        // Navigation property
        public Employee Employee { get; set; }
    }
}
