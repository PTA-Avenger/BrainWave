using BrainWave.Api.Entities;
using System;
using System.Collections.Generic;

namespace BrainWave.API.Entities
{
    public class Tasks
    {
        public int TaskID { get; set; }
        public int UserID { get; set; }
        public string Title { get; set; } = "";
        public string? Description { get; set; }
        public DateTime? Due_Date { get; set; }
        public string? Task_Status { get; set; } // Keep as string for existing DB
        public string? Priority_Level { get; set; } // Keep as string for existing DB

        // Navigation properties
        public User? User { get; set; }

        // Optional single collaboration
        public Collaboration? PrimaryCollaboration { get; set; }

        // Add this property to the Task class
        public ICollection<Collaboration> Collaborations { get; set; }

        public ICollection<Reminder> Reminders { get; set; } = new HashSet<Reminder>();
        public ICollection<Export> Exports { get; set; } = new HashSet<Export>();

        // Helper properties for enum conversion (no DB mapping)
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

        // In the Task class constructor, initialize the collection (optional but recommended)
        public Tasks()
        {
            Collaborations = new HashSet<Collaboration>();
        }
    }
}