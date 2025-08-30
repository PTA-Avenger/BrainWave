using BrainWave.APP.ViewModels;
using BrainWave.APP.Models;
namespace BrainWave.APP.Views;
public partial class AdminUsersPage : ContentPage
{
    private readonly AdminUsersViewModel _vm;
    public AdminUsersPage(AdminUsersViewModel vm) { InitializeComponent(); BindingContext = _vm = vm; }
    protected override async void OnAppearing() { base.OnAppearing(); await _vm.RefreshAsync(); }
    private async void Search_Clicked(object s, EventArgs e) => await _vm.RefreshAsync();
    private void Edit_Clicked(object s, EventArgs e)
    {
        if ((s as Button)?.CommandParameter is AdminUserDto u)
        {
            _vm.Editing = new AdminUserDto
            {
                UserID = u.UserID,
                F_Name = u.F_Name,
                L_Name = u.L_Name,
                Email = u.Email,
                Role = u.Role,
                Profile_Picture = u.Profile_Picture
            }; BindingContext = null; BindingContext = _vm;
        }
    }
    private async void Save_Clicked(object s, EventArgs e) => await _vm.UpdateAsync();
    private async void Delete_Invoked(object s, EventArgs e)
    { if ((s as SwipeItem)?.BindingContext is AdminUserDto u) await _vm.DeleteAsync(u); }
}