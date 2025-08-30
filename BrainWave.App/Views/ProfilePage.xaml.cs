using BrainWave.Maui.ViewModels;
namespace BrainWave.Maui.Views;
public partial class ProfilePage : ContentPage
{
    private readonly ProfileViewModel _vm;
    public ProfilePage(ProfileViewModel vm) { InitializeComponent(); BindingContext = _vm = vm; }
    private async void Save_Clicked(object s, EventArgs e)
    {
        if (await _vm.UpdateAsync()) await DisplayAlert("Saved", "Profile updated", "OK");
        else await DisplayAlert("Error", "Failed to update", "OK");
    }
    private async void Logout_Clicked(object s, EventArgs e) { await _vm.LogoutAsync(); await DisplayAlert("Logged out", "", "OK"); }
}