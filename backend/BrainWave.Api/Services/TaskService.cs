using BrainWave.Api.Data;
using BrainWave.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace BrainWave.Api.Services;

public interface ITaskService
{
	Task<List<TaskItem>> GetAllAsync(CancellationToken ct);
	Task<TaskItem?> GetByIdAsync(Guid id, CancellationToken ct);
	Task<TaskItem> CreateAsync(TaskItem task, CancellationToken ct);
	Task<bool> UpdateAsync(TaskItem task, CancellationToken ct);
	Task<bool> DeleteAsync(Guid id, CancellationToken ct);
}

public class TaskService : ITaskService
{
	private readonly ApplicationDbContext _db;

	public TaskService(ApplicationDbContext db)
	{
		_db = db;
	}

	public async Task<List<TaskItem>> GetAllAsync(CancellationToken ct) =>
		await _db.Tasks.AsNoTracking().OrderByDescending(t => t.UpdatedAtUtc).ToListAsync(ct);

	public async Task<TaskItem?> GetByIdAsync(Guid id, CancellationToken ct) =>
		await _db.Tasks.FindAsync(new object?[] { id }, ct);

	public async Task<TaskItem> CreateAsync(TaskItem task, CancellationToken ct)
	{
		task.CreatedAtUtc = DateTime.UtcNow;
		task.UpdatedAtUtc = DateTime.UtcNow;
		_db.Tasks.Add(task);
		await _db.SaveChangesAsync(ct);
		return task;
	}

	public async Task<bool> UpdateAsync(TaskItem task, CancellationToken ct)
	{
		var existing = await _db.Tasks.FirstOrDefaultAsync(t => t.Id == task.Id, ct);
		if (existing is null) return false;
		existing.Title = task.Title;
		existing.Description = task.Description;
		existing.DueAtUtc = task.DueAtUtc;
		existing.Status = task.Status;
		existing.UpdatedAtUtc = DateTime.UtcNow;
		await _db.SaveChangesAsync(ct);
		return true;
	}

	public async Task<bool> DeleteAsync(Guid id, CancellationToken ct)
	{
		var existing = await _db.Tasks.FirstOrDefaultAsync(t => t.Id == id, ct);
		if (existing is null) return false;
		_db.Tasks.Remove(existing);
		await _db.SaveChangesAsync(ct);
		return true;
	}
}
