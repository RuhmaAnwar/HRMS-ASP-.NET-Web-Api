using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRMS.Models.DTO
{
    [Table("employees")]
    public class EmployeeDTOPatch
    {
        [Column("first_name")]
        [MaxLength(50)]
        public string? FirstName { get; set; }

        [Column("last_name")]
        [MaxLength(50)]
        public string? LastName { get; set; }

        [Column("email")]
        [MaxLength(100)]
        [EmailAddress]
        public string? Email { get; set; }

        [Column("department_id")]
        [Required]
        public int DepartmentId { get; set; }

        [Column("role")]
        [MaxLength(50)]
        public string? Role { get; set; }
    }
}