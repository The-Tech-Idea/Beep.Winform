using System;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Helpers
{
    // Supporting classes for navigation painters
    public class NavigationItem
    {
        public string Text { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string IconName { get; set; } = string.Empty;
        public string Data { get; set; } = string.Empty;
    }

    public class TreeNodeItem
    {
        public string Text { get; set; } = string.Empty;
        public int Level { get; set; }
        public bool IsExpanded { get; set; }
        public bool HasChildren { get; set; }
        public string IconName { get; set; } = string.Empty;
        public string Data { get; set; } = string.Empty;
    }
}
