namespace BrainWave.Api.Models;

public enum UserRole
{
	User = 0,
	Admin = 1
}

public class User
{
	public string Id { get; set; } = Guid.NewGuid().ToString("n");
	public string Email { get; set; } = string.Empty;
	public string DisplayName { get; set; } = string.Empty;
	public UserRole Role { get; set; } = UserRole.User;
}
