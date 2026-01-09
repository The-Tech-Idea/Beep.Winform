using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Models
{
    /// <summary>
    /// Process step status enum
    /// </summary>
    public enum ProcessStepStatus
    {
        Pending,
        InProgress,
        Completed,
        Skipped,
        Error
    }

    /// <summary>
    /// Navigation item for navigation widgets
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class NavigationItem
    {
        [Category("Data")]
        [Description("Unique identifier for the navigation item")]
        public string Id { get; set; } = string.Empty;

        [Category("Data")]
        [Description("Display label for the navigation item")]
        public string Label { get; set; } = string.Empty;

        [Category("Appearance")]
        [Description("Path to icon image")]
        public string IconPath { get; set; } = string.Empty;

        [Category("Behavior")]
        [Description("Whether this item is currently active/selected")]
        public bool IsActive { get; set; } = false;

        [Category("Data")]
        [Description("Child navigation items (for hierarchical navigation)")]
        public List<NavigationItem> Children { get; set; } = new List<NavigationItem>();

        [Category("Data")]
        [Description("Optional tooltip text")]
        public string Tooltip { get; set; } = string.Empty;

        [Category("Data")]
        [Description("Optional badge count")]
        public int? BadgeCount { get; set; }

        public override string ToString() => Label;
    }

    /// <summary>
    /// Quick action item
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class QuickAction
    {
        [Category("Data")]
        [Description("Unique identifier for the action")]
        public string Id { get; set; } = string.Empty;

        [Category("Data")]
        [Description("Display label for the action")]
        public string Label { get; set; } = string.Empty;

        [Category("Appearance")]
        [Description("Path to icon image")]
        public string IconPath { get; set; } = string.Empty;

        [Category("Data")]
        [Description("Tooltip text for the action")]
        public string Tooltip { get; set; } = string.Empty;

        [Category("Behavior")]
        [Description("Whether the action is enabled")]
        public bool IsEnabled { get; set; } = true;

        public override string ToString() => Label;
    }

    /// <summary>
    /// Process flow step
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class ProcessFlowStep
    {
        [Category("Data")]
        [Description("Unique identifier for the step")]
        public string Id { get; set; } = string.Empty;

        [Category("Data")]
        [Description("Display label for the step")]
        public string Label { get; set; } = string.Empty;

        [Category("Data")]
        [Description("Status of the process step")]
        public ProcessStepStatus Status { get; set; } = ProcessStepStatus.Pending;

        [Category("Behavior")]
        [Description("Whether this step is completed")]
        public bool IsCompleted { get; set; } = false;

        [Category("Data")]
        [Description("Optional description")]
        public string Description { get; set; } = string.Empty;

        public override string ToString() => Label;
    }
}
