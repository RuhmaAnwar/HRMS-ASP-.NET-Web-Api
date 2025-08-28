using System.ComponentModel.DataAnnotations;

namespace HRMS.Models.DTO
{
    public class UpdateProfileDTO
    {
        public string? Role { get; set; }

        public string? PhoneNumber { get; set; } 
    }
}
