
using System.ComponentModel.DataAnnotations;


namespace HRMS.Models
{
    public class RefreshToken
    {
        [Key]
        public int Id { get; set; }
        // The refresh token string (should be a secure random string)
        [Required]
        public string Token { get; set; }

        // The user associated with the refresh token
        [Required]
        public Guid EmployeeId { get; set; }
        public Employee Employee { get; set; }

        // Token expiration date
        [Required]
        public DateTime ExpiresAt { get; set; }
        [Required]
        public bool IsRevoked { get; set; } 
        [Required]
        public DateTime CreatedAt { get; set; }
        public DateTime? RevokedAt { get; set; } = null;
    }
}
