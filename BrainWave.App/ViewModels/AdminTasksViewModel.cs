using BrainWave.APP.Models;
using BrainWave.APP.Services;
using System.Collections.ObjectModel;
namespace BrainWave.APP.ViewModels;
public class AdminTasksViewModel(ApiService api) : BaseViewModel
{
    public ObservableCollection<AdminTaskDto> Items { get; } = new();

    // Filters
    public string? UserID { get; set; }
    public string? Title { get; set; }
    public string? Task_Status { get; set; }
    public string? Priority_Level { get; set; }

    public AdminTaskDto Editing { get; set; } = new();

    public async Task RefreshAsync()
    {
        var filters = new Dictionary<string, string?>
        {
            ["UserID"] = UserID,
            ["Title"] = Title,
            ["Task_Status"] = Task_Status,
            ["Priority_Level"] = Priority_Level
        };
        Items.Clear(); foreach (var t in await api.AdminGetTasksAsync(filters)) Items.Add(t);
    }

    public async Task UpdateAsync()
    {
        if (await api.AdminUpdateTaskAsync(Editing)) await RefreshAsync();
    }

    public async Task DeleteAsync(AdminTaskDto t)
    {
        if (await api.AdminDeleteTaskAsync(t.TaskID)) await RefreshAsync();
    }
}