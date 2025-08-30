namespace BrainWave.APP.Services;
public class NavigationService
{
    public Task GoAsync(string route, IDictionary<string, object>? parameters = null)
        => Shell.Current.GoToAsync(route, parameters);
}