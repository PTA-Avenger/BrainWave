namespace BrainWave.Api.DTOs;

public record ReminderResponse(
	Guid Id,
	Guid TaskId,
	DateTime RemindAtUtc,
	bool Sent
);

public record CreateReminderRequest(
	Guid TaskId,
	DateTime RemindAtUtc
);

public record UpdateReminderRequest(
	DateTime RemindAtUtc,
	bool Sent
);
