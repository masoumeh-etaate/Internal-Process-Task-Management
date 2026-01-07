using InternalProcessMgmt.Models;
using InternalProcessMgmt.Models.DTOs;
using InternalProcessMgmt.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InternalProcessMgmt.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TasksController : ControllerBase
    {
        private readonly ITaskService _svc;

        public TasksController(ITaskService svc)
        {
            _svc = svc;
        }

        // GET: api/tasks
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] TaskQueryDto query)
        {
            var tasks = await _svc.GetAllAsync(query);
            return Ok(tasks);
        }

        // GET: api/tasks/(id}
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var task = await _svc.GetByIdAsync(id);
            if (task == null) return NotFound();
            return Ok(task);
        }

        // POST: api/tasks
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TaskCreateDto dto)
        {
            var userId = int.Parse(User.Claims.First(c => c.Type == "userId").Value);

            var created = await _svc.CreateTaskAsync(new TaskItem
            {
                Title = dto.Title,
                Description = dto.Description,
                Status = dto.Status,
                Priority = dto.Priority,
                AssignedToUserId = dto.AssignedToUserId
            }, userId);

            return CreatedAtAction(nameof(Get), new { id = created.TaskId }, created);
        }

        // PUT: api/tasks/{id}}/status
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] TaskUpdateDto dto)
        {
            int userId = int.Parse(User.Claims.First(c => c.Type == "userId").Value);

            await _svc.UpdateStatusAsync(id, dto.Status, userId);
            return NoContent();
        }

        //DELETE/api/tasks/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _svc.DeleteTaskAsync(id);
            return NoContent();
        }
    }
}
