namespace HRMS.Dtos.RequestDtos
{
    public class EmployeeUpdateRequestDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string Email { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }

        public DateTime DateOfBirth { get; set; }

        public DateTime HireDate { get; set; }
        public decimal Salary { get; set; }

        public Guid DepartmentId { get; set; }
        public Guid PositionId { get; set; }
        public Guid? ManagerId { get; set; }

        public int TotalLeaves { get; set; }
        public int LeavesUsed { get; set; }
    }
}
