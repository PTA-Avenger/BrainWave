using BrainWave.Api.DTOs;
using BrainWave.Api.Models;
using BrainWave.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace BrainWave.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RemindersController : ControllerBase
{
	private readonly IReminderService _reminders;

	public RemindersController(IReminderService reminders)
	{
		_reminders = reminders;
	}

	[HttpGet]
	public async Task<ActionResult<IEnumerable<ReminderResponse>>> GetAll(CancellationToken ct)
	{
		var list = await _reminders.GetAllAsync(ct);
		return Ok(list.Select(r => new ReminderResponse(r.Id, r.TaskId, r.RemindAtUtc, r.Sent)));
	}

	[HttpGet("{id:guid}")]
	public async Task<ActionResult<ReminderResponse>> GetById(Guid id, CancellationToken ct)
	{
		var r = await _reminders.GetByIdAsync(id, ct);
		if (r is null) return NotFound();
		return Ok(new ReminderResponse(r.Id, r.TaskId, r.RemindAtUtc, r.Sent));
	}

	[HttpPost]
	public async Task<ActionResult<ReminderResponse>> Create(CreateReminderRequest request, CancellationToken ct)
	{
		var created = await _reminders.CreateAsync(new Reminder { TaskId = request.TaskId, RemindAtUtc = request.RemindAtUtc, Sent = false }, ct);
		return CreatedAtAction(nameof(GetById), new { id = created.Id }, new ReminderResponse(created.Id, created.TaskId, created.RemindAtUtc, created.Sent));
	}

	[HttpPut("{id:guid}")]
	public async Task<IActionResult> Update(Guid id, UpdateReminderRequest request, CancellationToken ct)
	{
		var ok = await _reminders.UpdateAsync(new Reminder { Id = id, RemindAtUtc = request.RemindAtUtc, Sent = request.Sent }, ct);
		return ok ? NoContent() : NotFound();
	}

	[HttpDelete("{id:guid}")]
	public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
	{
		var ok = await _reminders.DeleteAsync(id, ct);
		return ok ? NoContent() : NotFound();
	}
}
