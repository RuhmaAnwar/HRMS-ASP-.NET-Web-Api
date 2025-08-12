using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HRMS.Models
{
    public class Department
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("first_name")]
        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = null!;

        // navigation property for employees
        public ICollection<Employee> Employees { get; set; } = null!;
    }
}
