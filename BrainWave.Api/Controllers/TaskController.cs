using BrainWave.API.DTOs;
using BrainWave.API.Entities;
using BrainWave.API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System;
using Task = BrainWave.API.Entities.Tasks;
using BrainWave.Api.DTOs;

namespace BrainWave.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TaskController : ControllerBase
    {
        private readonly TasksRepository _repo;
        private readonly ExportRepository _exportRepo;

        public TaskController(TasksRepository repo, ExportRepository exportRepo)
        {
            _repo = repo;
            _exportRepo = exportRepo;
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult<IEnumerable<TaskDtos>>> GetTasks(int userId, [FromQuery] TaskFilterDto? filter = null)
        {
            var tasks = await _repo.GetFilteredTasksAsync(userId, filter);
            return Ok(tasks);
        }

        [HttpPost]
        public async Task<ActionResult<TaskDtos>> CreateTask([FromBody] TaskDtos dto)
        {
            var task = new Task
            {
                UserID = dto.UserID,
                Title = dto.Title,
                Description = dto.Description,
                Due_Date = dto.Due_Date,
                Task_Status = dto.Task_Status,
                Priority_Level = dto.Priority_Level
            };

            await _repo.AddTaskAsync(task);

            dto.TaskID = task.TaskID;
            return Ok(dto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(int id, [FromBody] TaskDtos dto)
        {
            var task = await _repo.GetTaskByIdAsync(id);
            if (task == null) return NotFound();

            task.Title = dto.Title;
            task.Description = dto.Description;
            task.Due_Date = dto.Due_Date;
            task.Task_Status = dto.Task_Status;
            task.Priority_Level = dto.Priority_Level;

            await _repo.UpdateTaskAsync(task);
            return NoContent();
        }

        [HttpPost("{id}/share")]
        public async Task<ActionResult<string>> ShareTask(int id)
        {
            var shareToken = Guid.NewGuid().ToString();
            var shareUrl = $"{Request.Scheme}://{Request.Host}/api/task/shared/{shareToken}";

            // For now, just return the share URL (you can implement storage later if needed)
            return Ok(new { ShareToken = shareToken, ShareUrl = shareUrl });
        }

        [HttpPost("{id}/export")]
        public async Task<ActionResult> ExportTask(int id, [FromQuery] string format = "json")
        {
            var task = await _repo.GetTaskByIdAsync(id);
            if (task == null) return NotFound();

            // Create export record
            var export = new BrainWave.Api.Entities.Export
            {
                UserID = task.UserID,
                TaskID = id,
                Export_Format = format,
                Date_Requested = DateTime.UtcNow
            };

            await _exportRepo.AddExportAsync(export);

            if (format.ToLower() == "json")
            {
                return Ok(task);
            }
            else if (format.ToLower() == "csv")
            {
                var csv = $"TaskID,Title,Description,DueDate,Status,Priority\n{task.TaskID},\"{task.Title}\",\"{task.Description}\",{task.Due_Date},{task.Task_Status},{task.Priority_Level}";
                return Ok(csv);
            }

            return BadRequest("Unsupported format");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var task = await _repo.GetTaskByIdAsync(id);
            if (task == null) return NotFound();

            await _repo.DeleteTaskAsync(task);
            return NoContent();
        }
    }
}