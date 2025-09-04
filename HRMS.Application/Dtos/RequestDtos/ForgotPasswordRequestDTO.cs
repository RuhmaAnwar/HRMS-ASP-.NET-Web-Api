using System.ComponentModel.DataAnnotations;

namespace HRMS.Dtos.RequestDtos
{
    public class ForgotPasswordRequestDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        public string newPassword { get; set; } = null!;
    }
}
