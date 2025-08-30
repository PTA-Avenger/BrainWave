using BrainWave.Maui.Services;
using BrainWave.Maui.ViewModels;
using BrainWave.Maui.Views;
namespace BrainWave.Maui;
public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // Services
        builder.Services.AddSingleton<ApiService>();
        builder.Services.AddSingleton<NavigationService>();

        // VMs
        builder.Services.AddTransient<DashboardViewModel>();
        builder.Services.AddTransient<TasksViewModel>();
        builder.Services.AddTransient<ProfileViewModel>();
        builder.Services.AddTransient<RemindersViewModel>();
        builder.Services.AddTransient<CollaborationViewModel>();
        builder.Services.AddTransient<AdminLoginViewModel>();
        builder.Services.AddTransient<AdminUsersViewModel>();
        builder.Services.AddTransient<AdminTasksViewModel>();

        // Views
        builder.Services.AddTransient<DashboardPage>();
        builder.Services.AddTransient<TasksPage>();
        builder.Services.AddTransient<ProfilePage>();
        builder.Services.AddTransient<RemindersPage>();
        builder.Services.AddTransient<CollaborationPage>();
        builder.Services.AddTransient<AdminLoginPage>();
        builder.Services.AddTransient<AdminUsersPage>();
        builder.Services.AddTransient<AdminTasksPage>();

        return builder.Build();
    }
}
