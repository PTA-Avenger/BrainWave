namespace BrainWave.Api.Models;

public class Reminder
{
	public Guid Id { get; set; } = Guid.NewGuid();
	public Guid TaskId { get; set; }
	public DateTime RemindAtUtc { get; set; }
	public bool Sent { get; set; }
}
