using System.Windows.Input;
using BrainWave.APP.Services;

namespace BrainWave.APP.ViewModels
{
    public class RegisterViewModel : BaseViewModel
    {
        private string firstName;
        public string FirstName { get => firstName; set => SetProperty(ref firstName, value); }

        private string lastName;
        public string LastName { get => lastName; set => SetProperty(ref lastName, value); }

        private string email;
        public string Email { get => email; set => SetProperty(ref email, value); }

        private string password;
        public string Password { get => password; set => SetProperty(ref password, value); }

        private string errorMessage;
        public string ErrorMessage { get => errorMessage; set => SetProperty(ref errorMessage, value); }

        public ICommand RegisterCommand { get; }
        public ICommand GoToLoginCommand { get; }

        public RegisterViewModel()
        {
            RegisterCommand = new Command(async () => await RegisterAsync());
            GoToLoginCommand = new Command(async () =>
                await Shell.Current.GoToAsync("//LoginPage"));
        }

        private async Task RegisterAsync()
        {
            try
            {
                var result = await ApiService.RegisterAsync(FirstName, LastName, Email, Password);
                if (result)
                {
                    await Application.Current.MainPage.DisplayAlert("Success", "Account created!", "OK");
                    await Shell.Current.GoToAsync("//LoginPage");
                }
                else
                {
                    ErrorMessage = "Registration failed.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
        }
    }
}
