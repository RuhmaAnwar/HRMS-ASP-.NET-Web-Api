using System.ComponentModel.DataAnnotations;

namespace HRMS.Dtos.RequestDtos
{
    public class RefreshTokenRequestDTO
    {
        [Required]
        public string RefreshToken { get; set; }
    }
}
