using HRMS.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRMS.Models
{
    public class Leave
    {
        [Column("id")]
        public Guid Id { get; set; }

        [Column("employee_id")]
        public Guid EmployeeId { get; set; }

        [Column("start_date")]
        public DateOnly StartDate { get; set; }

        [Column("end_date")]
        public DateOnly EndDate { get; set; }

        [Column("type")]
        public LeaveType Type { get; set; }

        [Column("status")]
        public LeaveStatus Status { get; set; }

        [Column("approver_id")]
        public Guid? ApproverId { get; set; }

        [Column("reason")]
        public string Reason { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }
        [Column("is_deleted")]
        public bool IsDeleted { get; set; }

        // Navigation properties
        public Employee Employee { get; set; }
        public Employee? Approver { get; set; }
    }
}
