using System.Net.Http.Json;
using BrainWave.App.Models;

namespace BrainWave.App.Services;

public class ApiClient
{
	private readonly HttpClient _http;

	public ApiClient(HttpClient http)
	{
		_http = http;
		_http.BaseAddress ??= new Uri(Environment.GetEnvironmentVariable("BRAINWAVE_API") ?? "http://localhost:5000/");
	}

	public async Task<List<TaskItem>> GetTasksAsync(CancellationToken ct) =>
		await _http.GetFromJsonAsync<List<TaskItem>>("api/tasks", cancellationToken: ct) ?? new();

	public async Task<TaskItem?> CreateTaskAsync(TaskItem task, CancellationToken ct)
	{
		var resp = await _http.PostAsJsonAsync("api/tasks", new
		{
			Title = task.Title,
			Description = task.Description,
			DueAtUtc = task.DueAtUtc,
			OwnerId = task.OwnerId
		}, ct);
		resp.EnsureSuccessStatusCode();
		return await resp.Content.ReadFromJsonAsync<TaskItem>(cancellationToken: ct);
	}
}
