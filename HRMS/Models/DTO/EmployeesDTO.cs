using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRMS.Models.DTO
{
    [Table("employees")]
    public class EmployeeDTO
    {
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

        [Column("department_id")]
        [Required]
        public int DepartmentId { get; set; }

        [Column("role")]
        [Required]
        [MaxLength(50)]
        public string Role { get; set; } = null!;
    }
}