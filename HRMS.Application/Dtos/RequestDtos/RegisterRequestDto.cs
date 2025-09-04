namespace HRMS.Dtos.RequestDtos
{
    public class RegisterRequestDto
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public string Address { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public DateTime HireDate { get; set; }
        public decimal Salary { get; set; }
        public Guid DepartmentId { get; set; }
        public Guid PositionId { get; set; }
        public Guid? ManagerId { get; set; }
        public int TotalLeaves { get; set; } = 25;
        public int LeavesUsed { get; set; } = 0;
        public string[] Roles { get; set; } = Array.Empty<string>();
    }
}
