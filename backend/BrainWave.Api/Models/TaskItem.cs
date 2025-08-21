namespace BrainWave.Api.Models;

public enum TaskStatus
{
	Pending = 0,
	InProgress = 1,
	Completed = 2,
	Archived = 3
}

public class TaskItem
{
	public Guid Id { get; set; } = Guid.NewGuid();
	public string Title { get; set; } = string.Empty;
	public string? Description { get; set; }
	public DateTime? DueAtUtc { get; set; }
	public TaskStatus Status { get; set; } = TaskStatus.Pending;
	public string OwnerId { get; set; } = string.Empty;
	public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
	public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
}
