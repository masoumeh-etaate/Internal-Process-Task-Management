namespace InternalProcessMgmt.Models.DTOs
{
    public class TaskCreateDto
    {
        public required string Title { get; set; }
        public required string Description { get; set; }
        public required string Status { get; set; }
        public required string Priority { get; set; }
        public int? AssignedToUserId { get; set; }
    }
}
