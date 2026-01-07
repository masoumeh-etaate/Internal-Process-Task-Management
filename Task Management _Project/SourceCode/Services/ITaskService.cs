using InternalProcessMgmt.Models;

namespace InternalProcessMgmt.Services
{
    public interface ITaskService
    {
        Task<TaskItem> CreateTaskAsync(TaskItem task, int createdByUserId);
        Task<IEnumerable<TaskItem>>GetAllAsync(Models.DTOs.TaskQueryDto query);
        Task<TaskItem?> GetByIdAsync(int id);
        Task UpdateStatusAsync(int taskId, string newStatus, int userId);
        Task DeleteTaskAsync(int taskId);

    }
}
