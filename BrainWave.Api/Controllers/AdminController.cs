using BrainWave.Api.DTOs;
using BrainWave.API.Entities;
using Microsoft.AspNetCore.Mvc;
using BrainWave.API.Services;
using Microsoft.AspNetCore.Authorization;
using BrainWave.API.Auth;

namespace BrainWave.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly UserRepository _userRepo;
        private readonly TasksRepository _taskRepo;
        private readonly TokenService _tokenService;

        // Hard-coded admin credentials (as requested)
        private const string AdminUsername = "admin";
        private const string AdminPassword = "admin123!";

        public AdminController(UserRepository userRepo, TasksRepository taskRepo, TokenService tokenService)
        {
            _userRepo = userRepo;
            _taskRepo = taskRepo;
            _tokenService = tokenService;
        }

        [HttpPost("login")]
        public ActionResult AdminLogin([FromBody] AdminLoginDto dto)
        {
            if (dto.Username != AdminUsername || dto.Password != AdminPassword)
            {
                return Unauthorized("Invalid admin credentials");
            }

            // Return a simple success response with admin token
            var token = GenerateAdminToken();
            return Ok(new { Token = token, Role = "Admin", Message = "Admin login successful" });
        }

        private string GenerateAdminToken()
        {
            // Simple admin token generation (you can make this more sophisticated)
            var adminData = $"{AdminUsername}:{DateTime.UtcNow:yyyy-MM-dd}";
            var bytes = System.Text.Encoding.UTF8.GetBytes(adminData);
            return Convert.ToBase64String(bytes);
        }

        [HttpGet("users")]
        public async Task<ActionResult<IEnumerable<AdminUserDto>>> GetAllUsers([FromQuery] UserFilterDto? filter = null)
        {
            // Simple admin verification - check if request has admin token
            if (!IsAdminRequest()) return Unauthorized();

            var users = await _userRepo.GetFilteredUsersAsync(filter);
            var adminUsers = users.Select(u => new AdminUserDto
            {
                UserID = u.UserID,
                F_Name = u.F_Name,
                L_Name = u.L_Name,
                Email = u.Email,
                Role = u.Role,
                Profile_Picture = u.Profile_Picture,
                TaskCount = 0 // Will be populated if needed
            });

            return Ok(adminUsers);
        }

        [HttpGet("tasks")]
        public async Task<ActionResult<IEnumerable<AdminTaskDto>>> GetAllTasks([FromQuery] TaskFilterDto? filter = null)
        {
            if (!IsAdminRequest()) return Unauthorized();

            var tasks = await _taskRepo.GetAllFilteredTasksAsync(filter);
            return Ok(tasks);
        }

        [HttpPut("users/{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UserDtos dto)
        {
            if (!IsAdminRequest()) return Unauthorized();

            var user = await _userRepo.GetUserByIdAsync(id);
            if (user == null) return NotFound();

            user.F_Name = dto.F_Name;
            user.L_Name = dto.L_Name;
            user.Email = dto.Email;
            user.Role = dto.Role;
            user.Profile_Picture = dto.Profile_Picture;

            await _userRepo.UpdateUserAsync(user);
            return NoContent();
        }

        [HttpDelete("users/{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            if (!IsAdminRequest()) return Unauthorized();

            var user = await _userRepo.GetUserByIdAsync(id);
            if (user == null) return NotFound();

            await _userRepo.DeleteUserAsync(user.UserID);
            return NoContent();
        }

        [HttpPut("tasks/{id}")]
        public async Task<IActionResult> UpdateTask(int id, [FromBody] TaskDtos dto)
        {
            if (!IsAdminRequest()) return Unauthorized();

            var task = await _taskRepo.GetTaskByIdAsync(id);
            if (task == null) return NotFound();

            task.Title = dto.Title;
            task.Description = dto.Description;
            task.Due_Date = dto.Due_Date;
            task.Task_Status = dto.Task_Status;
            task.Priority_Level = dto.Priority_Level;

            await _taskRepo.UpdateTaskAsync(task);
            return NoContent();
        }

        [HttpDelete("tasks/{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            if (!IsAdminRequest()) return Unauthorized();

            var task = await _taskRepo.GetTaskByIdAsync(id);
            if (task == null) return NotFound();

            await _taskRepo.DeleteTaskAsync(task);
            return NoContent();
        }

        private bool IsAdminRequest()
        {
            // Simple admin verification - check for admin token in headers
            var authHeader = Request.Headers["Authorization"].FirstOrDefault();
            if (authHeader != null && authHeader.StartsWith("Bearer "))
            {
                var token = authHeader.Substring("Bearer ".Length).Trim();
                try
                {
                    var decoded = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(token));
                    return decoded.StartsWith(AdminUsername);
                }
                catch
                {
                    return false;
                }
            }
            return false;
        }
    }
}