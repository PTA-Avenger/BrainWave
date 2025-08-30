using BrainWave.APP.Models;
using BrainWave.APP.Services;
using System.Collections.ObjectModel;
namespace BrainWave.APP.ViewModels;
public class RemindersViewModel(ApiService api) : BaseViewModel
{
    public ObservableCollection<ReminderModel> Items { get; } = new();
    public ReminderModel Editing { get; set; } = new() { ReminderDate = DateTime.Today, ReminderTime = new TimeSpan(9, 0, 0) };

    public async Task LoadAsync()
    {
        Items.Clear(); foreach (var r in await api.GetRemindersAsync()) Items.Add(r);
    }

    public async Task CreateAsync()
    {
        if (await api.CreateReminderAsync(Editing)) { Editing = new() { ReminderDate = DateTime.Today, ReminderTime = new(9, 0, 0) }; OnPropertyChanged(nameof(Editing)); await LoadAsync(); }
    }
}