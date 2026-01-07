using System.ComponentModel.DataAnnotations;
namespace InternalProcessMgmt.Models
{
    public class TaskHistory
    {
        [Key]
        public int HistoryId { get; set; } 

        public int TaskId { get; set; } 
        public TaskItem? TaskItem { get; set; }
        public int ChangedByUserId { get; set; } 
        public User? ChangedBy { get; set; }
        public required string ChangeType { get; set; } 
        public string? OldValue { get; set; } 
        public string? NewValue { get; set; } 
        public DateTime ChangedAt { get; set; } = DateTime.UtcNow;
    }
}
