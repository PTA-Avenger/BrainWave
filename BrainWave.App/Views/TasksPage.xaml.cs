using BrainWave.Maui.ViewModels;
using BrainWave.Maui.Models;
namespace BrainWave.Maui.Views;
public partial class TasksPage : ContentPage
{
    private readonly TasksViewModel _vm;
    public TasksPage(TasksViewModel vm) { InitializeComponent(); BindingContext = _vm = vm; }
    protected override async void OnAppearing() { base.OnAppearing(); await _vm.RefreshAsync(); }
    private async void Refresh_Clicked(object s, EventArgs e) => await _vm.RefreshAsync();
    private void Edit_Clicked(object s, EventArgs e)
    {
        if ((s as Button)?.CommandParameter is TaskDtos item) _vm.Editing = new TaskDtos
        {
            TaskID = item.TaskID,
            Title = item.Title,
            Description = item.Description,
            Due_Date = item.Due_Date,
            Priority_Level = item.Priority_Level,
            Task_Status = item.Task_Status
        }; BindingContext = null; BindingContext = _vm; // refresh
    }
    private async void Create_Clicked(object s, EventArgs e) => await _vm.CreateAsync();
    private async void Update_Clicked(object s, EventArgs e) => await _vm.UpdateAsync();
    private async void SwipeItem_Invoked(object s, EventArgs e)
    {
        if ((s as SwipeItem)?.BindingContext is TaskDtos item)
            await _vm.DeleteAsync(item);
    }
}
