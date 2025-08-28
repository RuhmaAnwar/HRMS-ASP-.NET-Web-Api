using System.ComponentModel.DataAnnotations;

namespace HRMS.Models.DTO
{
    public class RegisterDTO
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


        [Required]
        [MaxLength(50)]
        public string Role { get; set; } = null!;

        [Required]
        [MinLength(6)]  // strong password
        public string Password { get; set; } = null!;
    }
}