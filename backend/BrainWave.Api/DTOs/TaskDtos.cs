using BrainWave.Api.Models;
using ModelTaskStatus = BrainWave.Api.Models.TaskStatus;

namespace BrainWave.Api.DTOs;

public record TaskResponse(
	Guid Id,
	string Title,
	string? Description,
	DateTime? DueAtUtc,
	ModelTaskStatus Status,
	string OwnerId,
	DateTime CreatedAtUtc,
	DateTime UpdatedAtUtc
);

public record CreateTaskRequest(
	string Title,
	string? Description,
	DateTime? DueAtUtc,
	string OwnerId
);

public record UpdateTaskRequest(
	string Title,
	string? Description,
	DateTime? DueAtUtc,
	ModelTaskStatus Status
);
