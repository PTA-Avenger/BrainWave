namespace BrainWave.API.Services
{
    public interface IEmailService
    {
        Task SendReminderEmailAsync(string toEmail, string subject, string body);
        Task SendCollaborationInviteAsync(string toEmail, string taskTitle, string invitePin);
        Task SendTaskSharedNotificationAsync(string toEmail, string taskTitle, string shareUrl);
    }
}