using BrainWave.APP.ViewModels;
namespace BrainWave.APP.Views;
public partial class DashboardPage : ContentPage
{
    private readonly DashboardViewModel _vm;
    public DashboardPage(DashboardViewModel vm)
    { InitializeComponent(); BindingContext = _vm = vm; }
    protected override async void OnAppearing() { base.OnAppearing(); if (!_vm.Upcoming.Any()) await _vm.LoadAsync(); }
    private async void Profile_Clicked(object s, EventArgs e) => await _vm.GoProfile();
    private async void Tasks_Clicked(object s, EventArgs e) => await _vm.GoTasks();
    private async void Collab_Clicked(object s, EventArgs e) => await _vm.GoCollab();
    private async void Reminders_Clicked(object s, EventArgs e) => await _vm.GoReminders();
}