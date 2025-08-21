using BrainWave.Api.Data;
using BrainWave.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace BrainWave.Api.Services;

public interface IReminderService
{
	Task<List<Reminder>> GetAllAsync(CancellationToken ct);
	Task<Reminder?> GetByIdAsync(Guid id, CancellationToken ct);
	Task<Reminder> CreateAsync(Reminder reminder, CancellationToken ct);
	Task<bool> UpdateAsync(Reminder reminder, CancellationToken ct);
	Task<bool> DeleteAsync(Guid id, CancellationToken ct);
}

public class ReminderService : IReminderService
{
	private readonly ApplicationDbContext _db;

	public ReminderService(ApplicationDbContext db)
	{
		_db = db;
	}

	public async Task<List<Reminder>> GetAllAsync(CancellationToken ct) =>
		await _db.Reminders.AsNoTracking().OrderBy(r => r.RemindAtUtc).ToListAsync(ct);

	public async Task<Reminder?> GetByIdAsync(Guid id, CancellationToken ct) =>
		await _db.Reminders.FindAsync(new object?[] { id }, ct);

	public async Task<Reminder> CreateAsync(Reminder reminder, CancellationToken ct)
	{
		_db.Reminders.Add(reminder);
		await _db.SaveChangesAsync(ct);
		return reminder;
	}

	public async Task<bool> UpdateAsync(Reminder reminder, CancellationToken ct)
	{
		var existing = await _db.Reminders.FirstOrDefaultAsync(r => r.Id == reminder.Id, ct);
		if (existing is null) return false;
		existing.RemindAtUtc = reminder.RemindAtUtc;
		existing.Sent = reminder.Sent;
		await _db.SaveChangesAsync(ct);
		return true;
	}

	public async Task<bool> DeleteAsync(Guid id, CancellationToken ct)
	{
		var existing = await _db.Reminders.FirstOrDefaultAsync(r => r.Id == id, ct);
		if (existing is null) return false;
		_db.Reminders.Remove(existing);
		await _db.SaveChangesAsync(ct);
		return true;
	}
}
