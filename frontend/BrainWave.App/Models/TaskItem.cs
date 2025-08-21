namespace BrainWave.App.Models;

public enum TaskStatus
{
	Pending = 0,
	InProgress = 1,
	Completed = 2,
	Archived = 3
}

public class TaskItem
{
	public Guid Id { get; set; }
	public string Title { get; set; } = string.Empty;
	public string? Description { get; set; }
	public DateTime? DueAtUtc { get; set; }
	public TaskStatus Status { get; set; }
	public string OwnerId { get; set; } = string.Empty;
	public DateTime CreatedAtUtc { get; set; }
	public DateTime UpdatedAtUtc { get; set; }
}
