using System.ComponentModel.DataAnnotations.Schema;

namespace HRMS.Models
{
    [Table("positions")]
    public class Position
    {
        [Column("id")]
        public Guid Id { get; set; }

        [Column("title")]
        public string Title { get; set; }

        [Column("description")]
        public string Description { get; set; }

        [Column("salary_range_min")]
        public decimal SalaryRangeMin { get; set; }

        [Column("salary_range_max")]
        public decimal SalaryRangeMax { get; set; }

        [Column("department_id")]
        public Guid? DepartmentId { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }

        [Column("is_deleted")]
        public bool IsDeleted { get; set; }

        // Navigation properties
        public Department? Department { get; set; }
        public ICollection<Employee> ? Employees { get; set; } = new List<Employee>();
    }
}
