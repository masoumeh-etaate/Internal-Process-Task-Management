using Dapper;
using InternalProcessMgmt.Data;
using InternalProcessMgmt.Models;
using InternalProcessMgmt.Models.DTOs;
using InternalProcessMgmt.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Runtime.CompilerServices;
namespace InternalProcessMgmt.Services
{
    public class TaskService : ITaskService
    {
        private readonly AppDbContext _db;

        public TaskService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<TaskItem> CreateTaskAsync(TaskItem task, int createdByUserId)
        {
            task.CreatedByUserId = createdByUserId;
            task.CreatedAt = DateTime.UtcNow;

            using var conn = _db.Database.GetDbConnection();
            await conn.OpenAsync();

            var parameters = new DynamicParameters();
            parameters.Add("@Title", task.Title);
            parameters.Add("@Description", task.Description);
            parameters.Add("@CreatedByUserId", createdByUserId);
            parameters.Add("@AssignedToUserId", task.AssignedToUserId);
            parameters.Add("@Status", task.Status ?? "New");
            parameters.Add("@Priority", task.Priority);
            parameters.Add("@DueDate", task.DueDate);
            parameters.Add("@NewTaskId", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await conn.ExecuteAsync("dbo.Task_Insert", parameters, commandType: CommandType.StoredProcedure);

            task.TaskId = parameters.Get<int>("@NewTaskId");

            return task;
        }

        public async Task<IEnumerable<TaskItem>> GetAllAsync(TaskQueryDto query)
        {
            using var conn = _db.Database.GetDbConnection();
            await conn.OpenAsync();

            var parameters = new DynamicParameters();
            parameters.Add("@Status", query.Status);
            parameters.Add("@AssignedToUserId", query.AssignedToUserId);
            parameters.Add("@Search", query.Search);

            var tasks = await conn.QueryAsync<TaskItem>(
                "dbo.Task_Search",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return tasks;
        }


        public async Task<TaskItem?> GetByIdAsync(int id)
        {
            using var conn = _db.Database.GetDbConnection();
            await conn.OpenAsync();

            var task = await conn.QuerySingleOrDefaultAsync<TaskItem>(
                "dbo.Task_GetById",
                new { TaskId = id },
                commandType: CommandType.StoredProcedure
            );

            return task;
        }

        public async Task UpdateStatusAsync(int taskId, string newStatus, int userId)
        {
            
            using var conn = _db.Database.GetDbConnection();
            await conn.OpenAsync();

            
            var task = await conn.QuerySingleOrDefaultAsync<TaskItem>(
                "dbo.Task_GetById",
                new { TaskId = taskId },
                commandType: CommandType.StoredProcedure
            );

            if (task == null) return;

            string oldValue = task.Status;

            var parameters = new DynamicParameters();
            parameters.Add("@TaskId", taskId, DbType.Int32);
            parameters.Add("@Status", newStatus, DbType.String);
            parameters.Add("@UpdatedByUserId", userId, DbType.Int32);

            await conn.ExecuteAsync(
                "dbo.Task_UpdateStatus",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            _db.TaskHistories.Add(new TaskHistory
            {
                TaskId = taskId,
                ChangedByUserId = userId,
                ChangeType = "StatusChanged",
                OldValue = oldValue,
                NewValue = newStatus,
                ChangedAt = DateTime.UtcNow
            });

            await _db.SaveChangesAsync();
        }

        public async Task DeleteTaskAsync(int taskId)
        {
            using var conn = _db.Database.GetDbConnection();
            await conn.OpenAsync();

            var parameters = new DynamicParameters();
            parameters.Add("@TaskId", taskId);

            await conn.ExecuteAsync(
                "dbo.Task_Delete",
                parameters,
                commandType: CommandType.StoredProcedure
            );
        }

    }
}
