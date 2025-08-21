using BrainWave.Api.DTOs;
using BrainWave.Api.Models;
using BrainWave.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace BrainWave.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TasksController : ControllerBase
{
	private readonly ITaskService _tasks;

	public TasksController(ITaskService tasks)
	{
		_tasks = tasks;
	}

	[HttpGet]
	public async Task<ActionResult<IEnumerable<TaskResponse>>> GetAll(CancellationToken ct)
	{
		var list = await _tasks.GetAllAsync(ct);
		return Ok(list.Select(ToResponse));
	}

	[HttpGet("{id:guid}")]
	public async Task<ActionResult<TaskResponse>> GetById(Guid id, CancellationToken ct)
	{
		var entity = await _tasks.GetByIdAsync(id, ct);
		if (entity is null) return NotFound();
		return Ok(ToResponse(entity));
	}

	[HttpPost]
	public async Task<ActionResult<TaskResponse>> Create(CreateTaskRequest request, CancellationToken ct)
	{
		var created = await _tasks.CreateAsync(new TaskItem
		{
			Title = request.Title,
			Description = request.Description,
			DueAtUtc = request.DueAtUtc,
			OwnerId = request.OwnerId
		}, ct);
		return CreatedAtAction(nameof(GetById), new { id = created.Id }, ToResponse(created));
	}

	[HttpPut("{id:guid}")]
	public async Task<IActionResult> Update(Guid id, UpdateTaskRequest request, CancellationToken ct)
	{
		var ok = await _tasks.UpdateAsync(new TaskItem
		{
			Id = id,
			Title = request.Title,
			Description = request.Description,
			DueAtUtc = request.DueAtUtc,
			Status = request.Status
		}, ct);
		return ok ? NoContent() : NotFound();
	}

	[HttpDelete("{id:guid}")]
	public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
	{
		var ok = await _tasks.DeleteAsync(id, ct);
		return ok ? NoContent() : NotFound();
	}

	private static TaskResponse ToResponse(TaskItem t) => new(
		t.Id,
		t.Title,
		t.Description,
		t.DueAtUtc,
		t.Status,
		t.OwnerId,
		t.CreatedAtUtc,
		t.UpdatedAtUtc
	);
}
