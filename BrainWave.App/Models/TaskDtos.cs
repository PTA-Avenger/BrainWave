namespace BrainWave.Maui.Models;
public class TaskDtos
{
    public int TaskID { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime? Due_Date { get; set; }
    public string? Task_Status { get; set; }
    public string? Priority_Level { get; set; }
}

public class AdminTaskDto : TaskDtos
{
    public int UserID { get; set; }
    public string UserName { get; set; } = string.Empty;
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