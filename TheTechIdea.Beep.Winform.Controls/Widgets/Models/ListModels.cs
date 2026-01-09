using System;
using System.ComponentModel;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Models
{
    /// <summary>
    /// Task priority enum
    /// </summary>
    public enum TaskPriority
    {
        Low,
        Normal,
        High,
        Critical
    }

    /// <summary>
    /// Base list item
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class ListItem
    {
        [Category("Data")]
        [Description("Unique identifier for the item")]
        public string Id { get; set; } = string.Empty;

        [Category("Data")]
        [Description("Title of the list item")]
        public string Title { get; set; } = string.Empty;

        [Category("Data")]
        [Description("Subtitle or secondary text")]
        public string Subtitle { get; set; } = string.Empty;

        [Category("Data")]
        [Description("Status text or indicator")]
        public string Status { get; set; } = string.Empty;

        [Category("Data")]
        [Description("Timestamp associated with the item")]
        public DateTime Timestamp { get; set; } = DateTime.Now;

        [Category("Appearance")]
        [Description("Path to icon image")]
        public string IconPath { get; set; } = string.Empty;

        public override string ToString() => Title;
    }

    /// <summary>
    /// Task item extending ListItem
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class TaskItem : ListItem
    {
        [Category("Behavior")]
        [Description("Whether the task is completed")]
        public bool IsCompleted { get; set; } = false;

        [Category("Data")]
        [Description("Priority level of the task")]
        public TaskPriority Priority { get; set; } = TaskPriority.Normal;

        [Category("Data")]
        [Description("Due date for the task")]
        public DateTime? DueDate { get; set; }

        [Category("Data")]
        [Description("Progress percentage (0-100)")]
        public int Progress { get; set; } = 0;
    }
}
