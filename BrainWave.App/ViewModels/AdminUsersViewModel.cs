using BrainWave.APP.Models;
using BrainWave.APP.Services;
using System.Collections.ObjectModel;
namespace BrainWave.APP.ViewModels;
public class AdminUsersViewModel(ApiService api) : BaseViewModel
{
    public ObservableCollection<AdminUserDto> Items { get; } = new();

    // Filters (all fields supported by backend filters via query params)
    public string? FName { get; set; }
    public string? LName { get; set; }
    public string? Email { get; set; }
    public string? Role { get; set; }

    public AdminUserDto Editing { get; set; } = new();

    public async Task RefreshAsync()
    {
        var filters = new Dictionary<string, string?>
        {
            ["F_Name"] = FName,
            ["L_Name"] = LName,
            ["Email"] = Email,
            ["Role"] = Role
        };
        Items.Clear();
        foreach (var u in await api.AdminGetUsersAsync(filters)) Items.Add(u);
    }

    public async Task UpdateAsync()
    {
        if (await api.AdminUpdateUserAsync(Editing.UserID, Editing)) await RefreshAsync();
    }

    public async Task DeleteAsync(AdminUserDto user)
    {
        if (await api.AdminDeleteUserAsync(user.UserID)) await RefreshAsync();
    }
}