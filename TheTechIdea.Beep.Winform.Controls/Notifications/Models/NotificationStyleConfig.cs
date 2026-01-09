using System.ComponentModel;
using TheTechIdea.Beep.Winform.Controls.Common;

namespace TheTechIdea.Beep.Winform.Controls.Notifications.Models
{
    /// <summary>
    /// Configuration model for notification style properties
    /// Defines visual properties for notification styling
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class NotificationStyleConfig
    {
        [Category("Style")]
        [Description("Notification layout style")]
        public NotificationLayout Layout { get; set; } = NotificationLayout.Standard;

        [Category("Style")]
        [Description("Associated control style")]
        public BeepControlStyle ControlStyle { get; set; } = BeepControlStyle.Material3;

        [Category("Layout")]
        [Description("Recommended padding")]
        public int RecommendedPadding { get; set; } = 12;

        [Category("Layout")]
        [Description("Recommended icon size")]
        public int RecommendedIconSize { get; set; } = 24;

        [Category("Layout")]
        [Description("Recommended spacing")]
        public int RecommendedSpacing { get; set; } = 8;

        [Category("Visual")]
        [Description("Recommended border radius")]
        public int RecommendedBorderRadius { get; set; } = 8;

        [Category("Visual")]
        [Description("Recommended minimum width")]
        public int RecommendedMinWidth { get; set; } = 280;

        [Category("Visual")]
        [Description("Recommended maximum width")]
        public int RecommendedMaxWidth { get; set; } = 420;

        public override string ToString() => $"Layout: {Layout}, Padding: {RecommendedPadding}, IconSize: {RecommendedIconSize}";
    }
}
