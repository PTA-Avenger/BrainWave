namespace BrainWave.API.Entities
{
    public enum BrainWaveTaskPriority
    {
        Low = 1,
        Medium = 2,
        High = 3
    }

    public enum BrainWaveTaskStatus
    {
        Pending,
        InProgress,
        Completed,
        Cancelled
    }
}