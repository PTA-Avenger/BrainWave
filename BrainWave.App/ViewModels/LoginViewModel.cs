using System.Windows.Input;
using BrainWave.APP.Services;
using BrainWave.APP.Models;

namespace BrainWave.APP.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private string email;
        public string Email { get => email; set => SetProperty(ref email, value); }

        private string password;
        public string Password { get => password; set => SetProperty(ref password, value); }

        private string errorMessage;
        public string ErrorMessage { get => errorMessage; set => SetProperty(ref errorMessage, value); }

        public ICommand LoginCommand { get; }
        public ICommand GoToRegisterCommand { get; }

        public LoginViewModel()
        {
            LoginCommand = new Command(async () => await LoginAsync());
            GoToRegisterCommand = new Command(async () =>
                await Shell.Current.GoToAsync("//RegisterPage"));
        }

        private async Task LoginAsync()
        {
            try
            {
                var token = await ApiService.LoginAsync(Email, Password);
                if (!string.IsNullOrEmpty(token))
                {
                    await SecureStorage.SetAsync("auth_token", token);
                    await Shell.Current.GoToAsync("//DashboardPage");
                }
                else
                {
                    ErrorMessage = "Invalid login attempt.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
        }
    }
}
