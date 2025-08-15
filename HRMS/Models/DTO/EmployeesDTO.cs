using System.ComponentModel.DataAnnotations;

namespace HRMS.Models.DTO
{
    public class EmployeeDTO
    {
        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; } = null!;

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; } = null!;

        [Required]
        [MaxLength(100)]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        public int DepartmentId { get; set; }

        public string DepartmentName { get; set; } = null!;

        [Required]
        [MaxLength(50)]
        public string Role { get; set; } = null!;
    }
}