using System;
using System.ComponentModel.DataAnnotations;

namespace HRMS.Dtos.RequestDtos
{
    public class LeaveCreateRequestDTO
    {
        [Required]
        public DateOnly StartDate { get; set; }

        [Required]
        public DateOnly EndDate { get; set; }

        [Required]
        public string Type { get; set; } // Maps to LeaveType enum

        public string Reason { get; set; }

        public Guid? ApproverId { get; set; }
    }
}
