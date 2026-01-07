using Dapper;
using InternalProcessMgmt.Data;
using InternalProcessMgmt.Models;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace InternalProcessMgmt.Repositories
{
    public class TaskRepository : ITaskRepository
    {
        private readonly AppDbContext _db;
        public TaskRepository(AppDbContext db)
        {
            _db = db;
        }
        public  async Task<TaskItem?>GetByIdAsync(int id)
        {
            using var conn = _db.Database.GetDbConnection();
            await conn.OpenAsync();

            var task = await conn.QuerySingleOrDefaultAsync<TaskItem>(
                "dbo.Task_GetById",
                new { TaskId = id },
                commandType: CommandType.StoredProcedure);

            return task; 
        }
        public async Task<IEnumerable<TaskItem>>GetAllAsync()
        {
            return await _db.Tasks.Include(t => t.CreatedBy).Include(t => t.AssignedTo).ToListAsync();
        }
        public async Task AddAsync(TaskItem task)
        {
            using var conn = _db.Database.GetDbConnection();
            await conn.OpenAsync();

            var parameters = new DynamicParameters();
            parameters.Add("@Title", task.Title);
            parameters.Add("@Description", task.Description);
            parameters.Add("@CreatedByUserId", task.CreatedByUserId);
            parameters.Add("@AssignedToUserId", task.AssignedToUserId);
            parameters.Add("@Status", task.Status ?? "New");
            parameters.Add("@Priority", task.Priority);
            parameters.Add("@DueDate", task.DueDate);
            parameters.Add("@NewTaskId", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await conn.ExecuteAsync("dbo.Task_Insert", parameters, commandType: CommandType.StoredProcedure);

            task.TaskId = parameters.Get<int>("@NewTaskId"); 
        }
        public async Task UpdateAsync(TaskItem task)
        {
            using var conn = _db.Database.GetDbConnection();
            await conn.OpenAsync();

            var parameters = new DynamicParameters();
            parameters.Add("@TaskId", task.TaskId);
            parameters.Add("@Title", task.Title);
            parameters.Add("@Description", task.Description);
            parameters.Add("@AssignedToUserId", task.AssignedToUserId);
            parameters.Add("@Status", task.Status);
            parameters.Add("@Priority", task.Priority);
            parameters.Add("@DueDate", task.DueDate);
            parameters.Add("@ChangedByUserId", task.CreatedByUserId); 

            await conn.ExecuteAsync("dbo.Task_Update", parameters, commandType: CommandType.StoredProcedure);
        }
        public async Task DeleteAsync(int id)
        {
            var task = await _db.Tasks.FindAsync(id);
            if (task != null)
            {
                _db.Tasks.Remove(task);
                await _db.SaveChangesAsync();
            }
        }
    }
}
