namespace BrainWave.Api.DTOs
{
    public class AdminLoginDto
    {
        public string Username { get; set; } = "";
        public string Password { get; set; } = "";
    }

    public class AdminUserDto
    {
        public int UserID { get; set; }
        public string F_Name { get; set; } = "";
        public string L_Name { get; set; } = "";
        public string Email { get; set; } = "";
        public string? Role { get; set; }
        public string? Profile_Picture { get; set; }
        public int TaskCount { get; set; }

        public string FullName => $"{F_Name} {L_Name}";
    }

    public class AdminTaskDto
    {
        public int TaskID { get; set; }
        public int UserID { get; set; }
        public string UserName { get; set; } = "";
        public string Title { get; set; } = "";
        public string? Description { get; set; }
        public DateTime? Due_Date { get; set; }
        public string? Task_Status { get; set; }
        public string? Priority_Level { get; set; }

        public string PriorityColor => Priority_Level switch
        {
            "High" => "#FF0000",
            "Medium" => "#FFA500",
            "Low" => "#008000",
            _ => "#808080"
        };

        public string StatusColor => Task_Status switch
        {
            "Completed" => "#008000",
            "InProgress" => "#0000FF",
            "Pending" => "#FFA500",
            "Cancelled" => "#FF0000",
            _ => "#808080"
        };
    }
}