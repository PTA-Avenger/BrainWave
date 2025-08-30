using BrainWave.API.Services;
using BrainWave.API.Entities;

namespace BrainWave.API.Services
{
    public class ReminderBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ReminderBackgroundService> _logger;

        public ReminderBackgroundService(IServiceProvider serviceProvider, ILogger<ReminderBackgroundService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async System.Threading.Tasks.Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var reminderRepo = scope.ServiceProvider.GetRequiredService<ReminderRepository>();
                    var taskRepo = scope.ServiceProvider.GetRequiredService<TasksRepository>();
                    var userRepo = scope.ServiceProvider.GetRequiredService<UserRepository>();
                    var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

                    var dueReminders = await reminderRepo.GetDueRemindersAsync();
                    _logger.LogInformation($"Found {dueReminders.Count} due reminders to process");

                    foreach (var reminder in dueReminders)
                    {
                        try
                        {
                            var task = await taskRepo.GetTaskByIdAsync(reminder.TaskID);
                            if (task?.User != null)
                            {
                                var subject = $"⏰ Reminder: {task.Title}";
                                var body = GenerateReminderEmailBody(task);

                                await emailService.SendReminderEmailAsync(task.User.Email, subject, body);
                                _logger.LogInformation($"Sent reminder email for task {task.TaskID} to {task.User.Email}");
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, $"Failed to process reminder {reminder.ReminderID}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in reminder background service");
                }

                // Check every 5 minutes
                await System.Threading.Tasks.Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
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