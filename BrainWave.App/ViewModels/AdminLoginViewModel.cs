using BrainWave.APP.Services;
namespace BrainWave.APP.ViewModels;
public class AdminLoginViewModel(ApiService api, NavigationService nav) : BaseViewModel
{
    public string Username { get; set; } = "admin"; // hardcoded creds allowed by backend
    public string Password { get; set; } = "admin123!";
    public async Task LoginAsync()
    {
        if (await api.AdminLoginAsync(Username, Password))
            await nav.GoAsync("//admin-users");
    }
}