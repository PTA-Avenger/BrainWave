using BrainWave.App.Services;
using BrainWave.App.ViewModels;
using BrainWave.App.Views;

namespace BrainWave.App;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>();

		builder.Services.AddSingleton(new HttpClient());
		builder.Services.AddSingleton<ApiClient>();
		builder.Services.AddTransient<TaskListViewModel>();
		builder.Services.AddTransient<TaskListPage>();

		return builder.Build();
	}
}
