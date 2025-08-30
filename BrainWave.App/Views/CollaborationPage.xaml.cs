using BrainWave.Maui.ViewModels;
namespace BrainWave.Maui.Views;
public partial class CollaborationPage : ContentPage
{
    private readonly CollaborationViewModel _vm;
    public CollaborationPage(CollaborationViewModel vm) { InitializeComponent(); BindingContext = _vm = vm; }
    protected override async void OnAppearing() { base.OnAppearing(); await _vm.LoadAsync(); }
}