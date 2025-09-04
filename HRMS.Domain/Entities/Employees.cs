using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HRMS.Models;
using Microsoft.AspNetCore.Identity;

namespace HRMS.Models
{
    [Table("employees")]
    public class Employee : IdentityUser<Guid>
    {
        [Column("id")]
        public override Guid Id {  get; set; }

        [Column("first_name")]
        public string FirstName { get; set; }

        [Column("last_name")]
        public string LastName { get; set; }

        [Column("dob")]
        public DateTime DateOfBirth { get; set; }

        [Column("address")]
        public string Address { get; set; }

        [Phone]
        public override string PhoneNumber { get; set; } // Overrides Identity's PhoneNumber for custom use

        [Column("hire_date")]
        public DateTime HireDate { get; set; }

        [Column("salary")]
        public decimal Salary { get; set; }

        // auditing fields
        [Column("created_at")]
        public DateTime CreatedAt { get; set; }
        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }
        [Column("is_deleted")]
        public bool IsDeleted { get; set; }


        [Column("department_id")]
        public Guid DepartmentId { get; set; } // FK to Departments

        [Column("position_id")]
        public Guid PositionId { get; set; } // FK to Positions

        [Column("manager_id")]
        public Guid? ManagerId { get; set; } // Self-reference FK to Employees (hierarchical)

        [Column("total_leaves")]
        public int TotalLeaves { get; set; }

        [Column("leaves_used")]
        public int LeavesUsed { get; set; }

        // Navigation properties
        public Department Department { get; set; }
        public Position Position { get; set; }
        public Employee? Manager { get; set; }
        public ICollection<Employee> Subordinates { get; set; } = new List<Employee>();
        public Department? DepartmentLed { get; set; } // inverse navigation
        public ICollection<Attendance> Attendances { get; set; }

        public ICollection<Leave> LeaveRequests { get; set; }

        public ICollection<Leave> ApprovedLeaves { get; set; }
        public ICollection<RefreshToken> RefreshTokens { get; set; }

    }
}