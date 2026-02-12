using System.ComponentModel;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Models
{
    /// <summary>
    /// Shared strongly-typed metadata model for widget item records.
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class WidgetMetadata
    {
        public string Key { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Group { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        public bool IsPinned { get; set; }
    }
}
