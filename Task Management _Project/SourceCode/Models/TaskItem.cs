using System.ComponentModel.DataAnnotations;
namespace InternalProcessMgmt.Models
{
    public class TaskItem
    {
        [Key]
        public int TaskId { get; set; } 

        public required string Title { get; set; } 
        public string? Description { get; set; } 
        public int CreatedByUserId { get; set; } 
        public User? CreatedBy { get; set; }
        public int? AssignedToUserId { get; set; } 
        public User? AssignedTo { get; set; }
        public string Status { get; set; } = "New"; 
        public string? Priority { get; set; } 
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? DueDate { get; set; } 
    }
}