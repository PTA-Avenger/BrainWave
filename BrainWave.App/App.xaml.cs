using Microsoft.Maui.Controls;

namespace BrainWave.APP;
public partial class App : Application
{
    public App()
    {
        InitializeComponent();
        MainPage = new AppShell();
    }
}