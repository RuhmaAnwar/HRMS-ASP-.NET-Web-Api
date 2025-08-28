using System;
using System.ComponentModel.DataAnnotations;

namespace HRMS.Dtos.RequestDtos
{
    public class LeaveUpdateRequestDTO
    {
        public DateOnly? StartDate { get; set; }

        public DateOnly? EndDate { get; set; }

        public string? Type { get; set; }

        public string? Reason { get; set; }

        public Guid? ApproverId { get; set; }
    }
}
