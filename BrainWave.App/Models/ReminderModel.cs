namespace BrainWave.APP.Models;
public class ReminderModel
{
    public int Id { get; set; }
    public string TaskName { get; set; } = string.Empty;
    public DateTime ReminderDate { get; set; }
    public TimeSpan ReminderTime { get; set; }
}