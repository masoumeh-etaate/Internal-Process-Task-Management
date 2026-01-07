namespace InternalProcessMgmt.Models.DTOs
{
    public class TaskQueryDto
    {
        public string? Status { get; set; }
        public string? Priority { get; set; }
        public int? CreatedByUserId { get; set; }
        public int? AssignedToUserId { get; set; }
        public string? Search { get; set; }
    }
}
