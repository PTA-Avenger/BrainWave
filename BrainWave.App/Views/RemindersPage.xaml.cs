using BrainWave.APP.ViewModels;
namespace BrainWave.APP.Views;
public partial class RemindersPage : ContentPage
{
    private readonly RemindersViewModel _vm;
    public RemindersPage(Remin dersViewModel vm) { InitializeComponent(); BindingContext = _vm = vm; }
    protected override async void OnAppearing() { base.OnAppearing(); await _vm.LoadAsync(); }
    private async void Add_Clicked(object s, EventArgs e) => await _vm.CreateAsync();
}