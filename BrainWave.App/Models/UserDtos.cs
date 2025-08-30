namespace BrainWave.APP.Models;
public class UserDtos
{
    public int UserID { get; set; }
    public string F_Name { get; set; } = string.Empty;
    public string L_Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Role { get; set; }
    public string? Profile_Picture { get; set; }
}

public class AdminUserDto : UserDtos
{
    public int TaskCount { get; set; }
    public string FullName => $"{F_Name} {L_Name}";
}