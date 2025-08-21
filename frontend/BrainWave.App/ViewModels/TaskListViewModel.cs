using System.Collections.ObjectModel;
using System.Windows.Input;
using BrainWave.App.Models;
using BrainWave.App.Services;

namespace BrainWave.App.ViewModels;

public class TaskListViewModel : BindableObject
{
	private readonly ApiClient _api;

	public ObservableCollection<TaskItem> Tasks { get; } = new();

	public ICommand RefreshCommand { get; }
	public ICommand AddTaskCommand { get; }

	public TaskListViewModel(ApiClient api)
	{
		_api = api;
		RefreshCommand = new Command(async () => await LoadAsync());
		AddTaskCommand = new Command(async () => await AddAsync());
		_ = LoadAsync();
	}

	private async Task LoadAsync()
	{
		Tasks.Clear();
		var list = await _api.GetTasksAsync(CancellationToken.None);
		foreach (var t in list)
		{
			Tasks.Add(t);
		}
	}

	private async Task AddAsync()
	{
		var created = await _api.CreateTaskAsync(new TaskItem
		{
			Title = $"New Task {DateTime.Now:HH:mm:ss}",
			OwnerId = "demo-user"
		}, CancellationToken.None);
		if (created is not null)
		{
			Tasks.Insert(0, created);
		}
	}
}
