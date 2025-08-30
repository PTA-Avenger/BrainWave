using BrainWave.API.Entities;

namespace BrainWave.Api.DTOs
{
    public class TaskDtos
    {
        public int TaskID { get; set; }
        public int UserID { get; set; }
        public string Title { get; set; } = "";
        public string? Description { get; set; }
        public DateTime? Due_Date { get; set; }
        public string? Task_Status { get; set; } = "Pending";
        public string? Priority_Level { get; set; } = "Medium";

        // Helper properties for enum conversion
        public BrainWaveTaskStatus StatusEnum
        {
            get => Enum.TryParse<BrainWaveTaskStatus>(Task_Status, out var status) ? status : BrainWaveTaskStatus.Pending;
            set => Task_Status = value.ToString();
        }

        public BrainWaveTaskPriority PriorityEnum
        {
            get => Enum.TryParse<BrainWaveTaskPriority>(Priority_Level, out var priority) ? priority : BrainWaveTaskPriority.Medium;
            set => Priority_Level = value.ToString();
        }
    }

    public class TaskFilterDto
    {
        public string? Status { get; set; }
        public string? Priority { get; set; }
        public string? SortBy { get; set; } = "TaskID";
        public bool SortDescending { get; set; } = true;
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}