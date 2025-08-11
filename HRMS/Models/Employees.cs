using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRMS.Models
{
    [Table("Employees")]
    public class Employee
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("first_name")]
        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; } = null!;

        [Column("last_name")]
        [Required]
        [MaxLength(50)]
        public string LastName { get; set; } = null!;

        [Column("email")]
        [Required]
        [MaxLength(100)]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Column("department")]
        [Required]
        [MaxLength(50)]
        public string Department { get; set; } = null!;

        [Column("role")]
        [Required]
        [MaxLength(50)]
        public string Role { get; set; } = null!;
    }
}