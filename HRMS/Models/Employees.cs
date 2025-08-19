using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HRMS.Models;
using Microsoft.AspNetCore.Identity;

namespace HRMS.Models
{
    [Table("employees")]
    public class Employee : IdentityUser<int>
    {
        [Column("id")]
        public override int Id { get; set; }

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
        public override string  Email { get; set; } = null!;

        [Column("role")]
        [Required]
        [MaxLength(50)]
        public string Role { get; set; } = null!;

        [Column("department_id")]
        [Required]
        public int DepartmentId { get; set; }

        // Navigation property
        public Department Department { get; set; } = null!;
    }
}