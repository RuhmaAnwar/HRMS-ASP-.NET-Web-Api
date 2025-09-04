using System.ComponentModel.DataAnnotations.Schema;

namespace HRMS.Models
{
    public class Department
    {
        [Column("id")]
        public Guid Id { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("description")]
        public string Description { get; set; }

        [Column("head_employee_id")]
        public Guid? HeadEmployeeId { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }

        [Column("is_deleted")]
        public bool IsDeleted { get; set; }
        // Navigation properties
        public Employee? HeadEmployee { get; set; }
        public ICollection<Employee> Employees { get; set; }
        public ICollection<Position> Positions { get; set; }
    }
}
