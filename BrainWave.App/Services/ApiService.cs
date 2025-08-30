using System.Net.Http.Headers;
using System.Net.Http.Json;
using BrainWave.APP.Models;
using BrainWave.APP.Helpers;
namespace BrainWave.APP.Services;
public class ApiService
{
    private readonly HttpClient _http = new() { BaseAddress = new Uri(Constants.API_BASE) };

    private async Task SetBearerAsync(bool admin = false)
    {
        var key = admin ? Constants.SECURE_KEY_ADMIN_TOKEN : Constants.SECURE_KEY_USER_TOKEN;
        var token = await SecureStorage.GetAsync(key);
        _http.DefaultRequestHeaders.Authorization = string.IsNullOrWhiteSpace(token)
            ? null
            : new AuthenticationHeaderValue("Bearer", token);
    }

    // ==== Auth
    public record LoginReq(string Email, string Password);
    public record RegisterReq(string Email, string Password, string F_Name, string L_Name);
    public record TokenRes(string Token);
    public record AdminLoginReq(string Username, string Password);
    public record AdminLoginRes(string Token, string Role, string Message);

    public async Task<bool> RegisterAsync(RegisterReq req)
    {
        var res = await _http.PostAsJsonAsync("/api/auth/register", req);
        return res.IsSuccessStatusCode;
    }

    public async Task<bool> LoginAsync(string email, string password)
    {
        var res = await _http.PostAsJsonAsync("/api/auth/login", new LoginReq(email, password));
        if (!res.IsSuccessStatusCode) return false;
        var token = await res.Content.ReadFromJsonAsync<TokenRes>();
        if (token?.Token is null) return false;
        await SecureStorage.SetAsync(Constants.SECURE_KEY_USER_TOKEN, token.Token);
        return true;
    }

    public async Task<bool> AdminLoginAsync(string username, string password)
    {
        var res = await _http.PostAsJsonAsync("/api/admin/login", new AdminLoginReq(username, password));
        if (!res.IsSuccessStatusCode) return false;
        var data = await res.Content.ReadFromJsonAsync<AdminLoginRes>();
        if (string.IsNullOrWhiteSpace(data?.Token)) return false;
        await SecureStorage.SetAsync(Constants.SECURE_KEY_ADMIN_TOKEN, data!.Token);
        return true;
    }

    public Task LogoutAsync() => SecureStorage.SetAsync(Constants.SECURE_KEY_USER_TOKEN, string.Empty);
    public Task AdminLogoutAsync() => SecureStorage.SetAsync(Constants.SECURE_KEY_ADMIN_TOKEN, string.Empty);

    // ==== Users (Admin)
    public async Task<List<AdminUserDto>> AdminGetUsersAsync(Dictionary<string, string?>? filters = null)
    {
        await SetBearerAsync(admin: true);
        var path = "/api/admin/users";
        if (filters != null && filters.Count > 0) path = path.WithQuery(filters);
        return await _http.GetFromJsonAsync<List<AdminUserDto>>(path) ?? new();
    }

    public async Task<bool> AdminUpdateUserAsync(int id, UserDtos dto)
    {
        await SetBearerAsync(admin: true);
        var res = await _http.PutAsJsonAsync($"/api/admin/users/{id}", dto);
        return res.IsSuccessStatusCode;
    }

    public async Task<bool> AdminDeleteUserAsync(int id)
    {
        await SetBearerAsync(admin: true);
        var res = await _http.DeleteAsync($"/api/admin/users/{id}");
        return res.IsSuccessStatusCode;
    }

    // ==== Tasks (User)
    public async Task<List<TaskDtos>> GetTasksAsync(Dictionary<string, string?>? filters = null)
    {
        await SetBearerAsync();
        var path = "/api/tasks";
        if (filters != null && filters.Count > 0) path = path.WithQuery(filters);
        return await _http.GetFromJsonAsync<List<TaskDtos>>(path) ?? new();
    }

    public async Task<bool> CreateTaskAsync(TaskDtos dto)
    {
        await SetBearerAsync();
        var res = await _http.PostAsJsonAsync("/api/tasks", dto);
        return res.IsSuccessStatusCode;
    }

    public async Task<bool> UpdateTaskAsync(TaskDtos dto)
    {
        await SetBearerAsync();
        var res = await _http.PutAsJsonAsync($"/api/tasks/{dto.TaskID}", dto);
        return res.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteTaskAsync(int id)
    {
        await SetBearerAsync();
        var res = await _http.DeleteAsync($"/api/tasks/{id}");
        return res.IsSuccessStatusCode;
    }

    // ==== Tasks (Admin)
    public async Task<List<AdminTaskDto>> AdminGetTasksAsync(Dictionary<string, string?>? filters = null)
    {
        await SetBearerAsync(admin: true);
        var path = "/api/admin/tasks";
        if (filters != null && filters.Count > 0) path = path.WithQuery(filters);
        return await _http.GetFromJsonAsync<List<AdminTaskDto>>(path) ?? new();
    }

    public async Task<bool> AdminUpdateTaskAsync(AdminTaskDto dto)
    {
        await SetBearerAsync(admin: true);
        var res = await _http.PutAsJsonAsync($"/api/admin/tasks/{dto.TaskID}", dto);
        return res.IsSuccessStatusCode;
    }

    public async Task<bool> AdminDeleteTaskAsync(int id)
    {
        await SetBearerAsync(admin: true);
        var res = await _http.DeleteAsync($"/api/admin/tasks/{id}");
        return res.IsSuccessStatusCode;
    }

    // ==== Export (User)
    public async Task<HttpResponseMessage> ExportTaskAsync(int id, string format)
    {
        await SetBearerAsync();
        return await _http.PostAsync($"/api/tasks/{id}/export?format={format}", null);
    }

    // ==== Reminders (User)
    public async Task<List<ReminderModel>> GetRemindersAsync()
    {
        await SetBearerAsync();
        return await _http.GetFromJsonAsync<List<ReminderModel>>("/api/reminders") ?? new();
    }

    public async Task<bool> CreateReminderAsync(ReminderModel model)
    {
        await SetBearerAsync();
        var res = await _http.PostAsJsonAsync("/api/reminders", model);
        return res.IsSuccessStatusCode;
    }

    // ==== Collaboration (User)
    public async Task<List<CollaborationModel>> GetCollaborationsAsync()
    {
        await SetBearerAsync();
        return await _http.GetFromJsonAsync<List<CollaborationModel>>("/api/collaboration") ?? new();
    }
}