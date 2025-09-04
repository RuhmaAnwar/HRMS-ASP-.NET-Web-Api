using System.ComponentModel.DataAnnotations;

namespace HRMS.Models
{
    public class SigningKey
    {
        [Key]
        public Guid Id { get; set; }
        // Unique identifier for the key (Key ID).
        [Required]
        [MaxLength(100)]
        public string KeyId { get; set; }
        // The RSA private key.
        [Required]
        public string PrivateKey { get; set; }
        // The RSA public key in XML or PEM format.
        [Required]
        public string PublicKey { get; set; }
        [Required]
        public bool IsActive { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; }
        [Required]
        public DateTime ExpiresAt { get; set; }
    }
}
