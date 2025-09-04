// Response DTO for GET endpoints (e.g., GetAll, GetById)

namespace HRMS.Dtos.ResponseDtos
{
    public class EmployeeResponseDto
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string Email { get; set; }
        public DateTime DateOfBirth { get; set; }

        public string Address { get; set; }
        public string PhoneNumber { get; set; }

        public DateTime HireDate { get; set; }
        public decimal Salary { get; set; }

        //public Guid DepartmentId { get; set; }
        public string DepartmentName { get; set; } 

        //public Guid PositionId { get; set; }
        public string PositionTitle { get; set; } 

        //public Guid? ManagerId { get; set; }
        public string ManagerName { get; set; } 

        public int TotalLeaves { get; set; }
        public int LeavesUsed { get; set; }
    }
}
