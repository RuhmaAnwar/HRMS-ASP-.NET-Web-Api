namespace HRMS.Dtos.RequestDtos
{
    public class EmployeeCreateRequestDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; } 
        public DateTime DateOfBirth { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }

        public DateTime HireDate { get; set; }
        public decimal Salary { get; set; }

        public Guid DepartmentId { get; set; }
        public Guid PositionId { get; set; }
        public Guid? ManagerId { get; set; }

        // Leave Info
        //public int TotalLeaves { get; set; } = 25; 
        //public int LeavesUsed { get; set; } = 0;   

        public string[] Roles { get; set; } 
    }
}
