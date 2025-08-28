using System;

namespace HRMS.Dtos.ResponseDtos
{
    public class LeaveResponseDTO
    {
        public Guid Id { get; set; }

        public Guid EmployeeId { get; set; }

        public string EmployeeName { get; set; }

        public DateOnly StartDate { get; set; }

        public DateOnly EndDate { get; set; }

        public string Type { get; set; }

        public string Status { get; set; }

        public string Reason { get; set; }

        public Guid? ApproverId { get; set; }

        public string ApproverName { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}
