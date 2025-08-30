using BrainWave.APP.Models;
using BrainWave.APP.Services;
using System.Collections.ObjectModel;
namespace BrainWave.APP.ViewModels;
public class DashboardViewModel(ApiService api, NavigationService nav) : BaseViewModel
{
    public ObservableCollection<TaskDtos> Upcoming { get; } = new();

    public async Task LoadAsync()
    {
        if (IsBusy) return; IsBusy = true;
        try
        {
            var tasks = await api.GetTasksAsync();
            var now = DateTime.UtcNow;
            foreach (var t in tasks
                .Where(t => t.Due_Date.HasValue)
                .OrderBy(t => t.Due_Date)
                .ThenBy(t => t.Priority_Level))
            {
                // Only show future or today
                if (t.Due_Date!.Value.ToUniversalTime() >= now)
                    Upcoming.Add(t);
            }
        }
        finally { IsBusy = false; }
    }

    public Task GoProfile() => nav.GoAsync("//profile");
    public Task GoTasks() => nav.GoAsync("//tasks");
    public Task GoReminders() => nav.GoAsync("//reminders");
    public Task GoCollab() => nav.GoAsync("//collab");
}