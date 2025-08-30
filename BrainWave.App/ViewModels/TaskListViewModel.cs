using BrainWave.APP.Models;
using BrainWave.APP.Services;
using System.Collections.ObjectModel;
namespace BrainWave.APP.ViewModels;
public class TasksViewModel(ApiService api) : BaseViewModel
{
    public ObservableCollection<TaskDtos> Items { get; } = new();

    // Filters/Sort
    private string? _priorityFilter; public string? PriorityFilter { get => _priorityFilter; set { Set(ref _priorityFilter, value); _ = RefreshAsync(); } }
    private string? _statusFilter; public string? StatusFilter { get => _statusFilter; set { Set(ref _statusFilter, value); _ = RefreshAsync(); } }
    private string _sortBy = "Due_Date"; public string SortBy { get => _sortBy; set { Set(ref _sortBy, value); ApplySort(); } }

    private List<TaskDtos> _all = new();

    public async Task RefreshAsync()
    {
        if (IsBusy) return; IsBusy = true;
        try
        {
            var filters = new Dictionary<string, string?>
            {
                ["Priority_Level"] = PriorityFilter,
                ["Task_Status"] = StatusFilter
            };
            _all = await api.GetTasksAsync(filters);
            ApplySort();
        }
        finally { IsBusy = false; }
    }

    private void ApplySort()
    {
        IEnumerable<TaskDtos> q = _all;
        q = SortBy switch
        {
            "Priority_Level" => q.OrderBy(t => t.Priority_Level),
            "Task_Status" => q.OrderBy(t => t.Task_Status),
            _ => q.OrderBy(t => t.Due_Date)
        };
        Items.Clear(); foreach (var t in q) Items.Add(t);
    }

    // CRUD
    public TaskDtos Editing { get; set; } = new();

    public async Task CreateAsync()
    {
        if (await api.CreateTaskAsync(Editing)) { await RefreshAsync(); Editing = new(); OnPropertyChanged(nameof(Editing)); }
    }
    public async Task UpdateAsync()
    {
        if (Editing.TaskID == 0) return;
        if (await api.UpdateTaskAsync(Editing)) { await RefreshAsync(); Editing = new(); OnPropertyChanged(nameof(Editing)); }
    }
    public async Task DeleteAsync(TaskDtos item)
    {
        if (await api.DeleteTaskAsync(item.TaskID)) await RefreshAsync();
    }
}
