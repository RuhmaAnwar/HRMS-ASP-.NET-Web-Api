using System.ComponentModel.DataAnnotations;

namespace HRMS.Dtos.RequestDtos
{
    public class LeaveApprovalRequestDTO
    {
        [Required]
        public string Status { get; set; } 

        public string Comments { get; set; }
    }
}
