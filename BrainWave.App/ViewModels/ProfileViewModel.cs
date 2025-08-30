using BrainWave.APP.Models;
using BrainWave.APP.Services;
namespace BrainWave.APP.ViewModels;
public class ProfileViewModel(ApiService api) : BaseViewModel
{
    public UserDtos Me { get; set; } = new();

    // Minimal: Backend doesn't expose a dedicated GET /me in the doc.
    // Option 1: You may be filling Me from login/register response externally.
    // For this starter, expose Update and Delete via Admin endpoints when user is admin; otherwise you'd have user endpoints.

    public async Task<bool> UpdateAsync()
    {
        // If non-admin user update endpoint exists (not in doc), adjust here.
        // Placeholder: try admin update for own id if token has rights.
        return await api.AdminUpdateUserAsync(Me.UserID, Me);
    }

    public Task LogoutAsync() => api.LogoutAsync();
}