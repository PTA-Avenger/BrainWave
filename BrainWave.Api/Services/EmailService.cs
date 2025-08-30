using System.Net;
using System.Net.Mail;

namespace BrainWave.API.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SendReminderEmailAsync(string toEmail, string subject, string body)
        {
            await SendEmailAsync(toEmail, subject, body);
        }

        public async Task SendCollaborationInviteAsync(string toEmail, string taskTitle, string invitePin)
        {
            var subject = $"Collaboration Invite: {taskTitle}";
            var body = $@"
            <html>
            <head>
                <style>
                    body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
                    .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                    .header {{ background-color: #4CAF50; color: white; padding: 20px; text-align: center; }}
                    .content {{ padding: 20px; background-color: #f9f9f9; }}
                    .pin {{ font-size: 24px; font-weight: bold; color: #4CAF50; text-align: center; 
                           background-color: #e8f5e8; padding: 15px; margin: 20px 0; border-radius: 5px; }}
                    .footer {{ text-align: center; padding: 20px; color: #666; }}
                </style>
            </head>
            <body>
                <div class='container'>
                    <div class='header'>
                        <h1>🤝 Collaboration Invitation</h1>
                    </div>
                    <div class='content'>
                        <h2>You've been invited to collaborate!</h2>
                        <p>You have been invited to collaborate on the task: <strong>{taskTitle}</strong></p>
                        
                        <p>To join this collaboration, use the following PIN code in the BrainWave app:</p>
                        <div class='pin'>{invitePin}</div>
                        
                        <p><strong>Instructions:</strong></p>
                        <ol>
                            <li>Open the BrainWave app</li>
                            <li>Go to the Collaborations tab</li>
                            <li>Enter the PIN code above</li>
                            <li>Click 'Join' to start collaborating</li>
                        </ol>
                        
                        <p><em>Note: This PIN will expire in 24 hours for security reasons.</em></p>
                    </div>
                    <div class='footer'>
                        <p>Best regards,<br>The BrainWave Team</p>
                    </div>
                </div>
            </body>
            </html>";

            await SendEmailAsync(toEmail, subject, body);
        }

        public async Task SendTaskSharedNotificationAsync(string toEmail, string taskTitle, string shareUrl)
        {
            var subject = $"Task Shared: {taskTitle}";
            var body = $@"
            <html>
            <head>
                <style>
                    body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
                    .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                    .header {{ background-color: #2196F3; color: white; padding: 20px; text-align: center; }}
                    .content {{ padding: 20px; background-color: #f9f9f9; }}
                    .share-link {{ background-color: #e3f2fd; padding: 15px; margin: 20px 0; 
                                  border-radius: 5px; word-break: break-all; }}
                    .footer {{ text-align: center; padding: 20px; color: #666; }}
                </style>
            </head>
            <body>
                <div class='container'>
                    <div class='header'>
                        <h1>📋 Task Shared</h1>
                    </div>
                    <div class='content'>
                        <h2>A task has been shared with you!</h2>
                        <p>The task '<strong>{taskTitle}</strong>' has been shared with you.</p>
                        
                        <p>You can view this task using the following link:</p>
                        <div class='share-link'>
                            <a href='{shareUrl}' target='_blank'>{shareUrl}</a>
                        </div>
                        
                        <p>Click the link above to view the task details.</p>
                    </div>
                    <div class='footer'>
                        <p>Best regards,<br>The BrainWave Team</p>
                    </div>
                </div>
            </body>
            </html>";

            await SendEmailAsync(toEmail, subject, body);
        }

        private async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            try
            {
                var smtpServer = _configuration["Email:SmtpServer"];
                var port = int.Parse(_configuration["Email:Port"] ?? "587");
                var username = _configuration["Email:Username"];
                var password = _configuration["Email:Password"];
                var fromAddress = _configuration["Email:FromAddress"] ?? "noreply@brainwave.com";
                var enableSsl = bool.Parse(_configuration["Email:EnableSsl"] ?? "true");

                if (string.IsNullOrEmpty(smtpServer) || string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                {
                    _logger.LogWarning("Email configuration is incomplete. Skipping email send.");
                    return;
                }

                using var smtpClient = new SmtpClient(smtpServer)
                {
                    Port = port,
                    Credentials = new NetworkCredential(username, password),
                    EnableSsl = enableSsl,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false
                };

                using var mailMessage = new MailMessage
                {
                    From = new MailAddress(fromAddress, "BrainWave App"),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };

                mailMessage.To.Add(toEmail);

                await smtpClient.SendMailAsync(mailMessage);
                _logger.LogInformation($"Email sent successfully to {toEmail}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send email to {toEmail}: {ex.Message}");
                // Don't throw to prevent application crash
            }
        }
    }
}