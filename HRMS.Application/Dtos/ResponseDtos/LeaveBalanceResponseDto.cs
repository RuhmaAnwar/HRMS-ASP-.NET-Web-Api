namespace HRMS.Dtos.ResponseDtos
{
    public class LeaveBalanceResponseDto
    {
        public Guid Id { get; set; }
        public int TotalLeaves { get; set; }
        public int LeavesUsed { get; set; }
        public int RemainingLeaves => TotalLeaves - LeavesUsed;
    }
}
