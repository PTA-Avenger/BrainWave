using BrainWave.Api.DTOs;
using BrainWave.Api.Entities;
using BrainWave.API.DTOs;
using BrainWave.API.Entities;
using BrainWave.API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace BrainWave.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ReminderController : ControllerBase
    {
        private readonly ReminderRepository _repo;
        private readonly TasksRepository _taskRepo;
        private readonly UserRepository _userRepo;
        private readonly IEmailService _emailService;

        public ReminderController(ReminderRepository repo, TasksRepository taskRepo, UserRepository userRepo, IEmailService emailService)
        {
            _repo = repo;
            _taskRepo = taskRepo;
            _userRepo = userRepo;
            _emailService = emailService;
        }

        [HttpGet("{taskId}")]
        public async System.Threading.Tasks.Task<ActionResult<IEnumerable<ReminderDtos>>> GetReminders(int taskId)
        {
            var reminders = await _repo.GetRemindersByTaskIdAsync(taskId);
            return Ok(reminders.Select(r => new ReminderDtos
            {
                ReminderID = r.ReminderID,
                TaskID = r.TaskID,
                Reminder_Type = r.Reminder_Type,
                Notify_Time = r.Notify_Time
            }));
        }

        [HttpGet("user/{userId}")]
        public async System.Threading.Tasks.Task<ActionResult<IEnumerable<ReminderDtos>>> GetUserReminders(int userId)
        {
            var reminders = await _repo.GetRemindersByUserIdAsync(userId);
            return Ok(reminders.Select(r => new ReminderDtos
            {
                ReminderID = r.ReminderID,
                TaskID = r.TaskID,
                Reminder_Type = r.Reminder_Type,
                Notify_Time = r.Notify_Time
            }));
        }

        [HttpPost]
        public async System.Threading.Tasks.Task<ActionResult<ReminderDtos>> CreateReminder([FromBody] ReminderDtos dto)
        {
            var reminder = new Reminder
            {
                TaskID = dto.TaskID,
                Reminder_Type = dto.Reminder_Type,
                Notify_Time = dto.Notify_Time
            };

            await _repo.AddReminderAsync(reminder);

            dto.ReminderID = reminder.ReminderID;
            return Ok(dto);
        }

        [HttpPost("send-due-reminders")]
        public async System.Threading.Tasks.Task<ActionResult> SendDueReminders()
        {
            var dueReminders = await _repo.GetDueRemindersAsync();
            var emailsSent = 0;

            foreach (var reminder in dueReminders)
            {
                try
                {
                    var task = await _taskRepo.GetTaskByIdAsync(reminder.TaskID);
                    if (task?.User != null)
                    {
                        var subject = $"⏰ Reminder: {task.Title}";
                        var body = GenerateReminderEmailBody(task);

                        await _emailService.SendReminderEmailAsync(task.User.Email, subject, body);
                        emailsSent++;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to send reminder {reminder.ReminderID}: {ex.Message}");
                }
            }

            return Ok(new { EmailsSent = emailsSent, Message = $"Sent {emailsSent} reminder emails" });
        }

        [HttpPut("{id}")]
        public async System.Threading.Tasks.Task<IActionResult> UpdateReminder(int id, [FromBody] ReminderDtos dto)
        {
            var reminder = await _repo.GetReminderByIdAsync(id);
            if (reminder == null) return NotFound();

            reminder.Reminder_Type = dto.Reminder_Type;
            reminder.Notify_Time = dto.Notify_Time;

            await _repo.UpdateReminderAsync(reminder);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async System.Threading.Tasks.Task<IActionResult> DeleteReminder(int id)
        {
            var reminder = await _repo.GetReminderByIdAsync(id);
            if (reminder == null) return NotFound();

            await _repo.DeleteReminderAsync(id);
            return NoContent();
        }

        private string GenerateReminderEmailBody(BrainWave.API.Entities.Tasks task)
        {
            return $@"
            <html>
            <head>
                <style>
                    body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
                    .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                    .header {{ background-color: #FF9800; color: white; padding: 20px; text-align: center; }}
                    .content {{ padding: 20px; background-color: #f9f9f9; }}
                    .task-details {{ background-color: white; padding: 15px; margin: 15px 0; border-radius: 5px; }}
                    .priority {{ padding: 5px 10px; border-radius: 3px; color: white; display: inline-block; }}
                    .priority-high {{ background-color: #f44336; }}
                    .priority-medium {{ background-color: #ff9800; }}
                    .priority-low {{ background-color: #4caf50; }}
                    .footer {{ text-align: center; padding: 20px; color: #666; }}
                </style>
            </head>
            <body>
                <div class='container'>
                    <div class='header'>
                        <h1>⏰ Task Reminder</h1>
                    </div>
                    <div class='content'>
                        <h2>Don't forget about your task!</h2>
                        <div class='task-details'>
                            <h3>📋 {task.Title}</h3>
                            <p><strong>Description:</strong> {task.Description ?? "No description"}</p>
                            <p><strong>Due Date:</strong> {task.Due_Date?.ToString("MMM dd, yyyy 'at' HH:mm") ?? "No due date set"}</p>
                            <p><strong>Priority:</strong> <span class='priority priority-{task.Priority_Level?.ToLower()}'>{task.Priority_Level ?? "Medium"}</span></p>
                            <p><strong>Status:</strong> {task.Task_Status ?? "Pending"}</p>
                        </div>
                        
                        <p>This is an automated reminder for your task. Please check your BrainWave app to update the task status.</p>
                    </div>
                    <div class='footer'>
                        <p>Best regards,<br>The BrainWave Team</p>
                        <p><small>You received this email because you have reminders enabled for this task.</small></p>
                    </div>
                </div>
            </body>
            </html>";
        }
    }
}