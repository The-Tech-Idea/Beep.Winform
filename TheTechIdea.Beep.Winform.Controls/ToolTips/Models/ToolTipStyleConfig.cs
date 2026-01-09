using System.ComponentModel;
using TheTechIdea.Beep.Winform.Controls.Common;

namespace TheTechIdea.Beep.Winform.Controls.ToolTips.Models
{
    /// <summary>
    /// Configuration model for tooltip style properties
    /// Defines visual properties for tooltip styling
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class ToolTipStyleConfig
    {
        [Category("Style")]
        [Description("Associated control style")]
        public BeepControlStyle ControlStyle { get; set; } = BeepControlStyle.Material3;

        [Category("Layout")]
        [Description("Recommended padding")]
        public int RecommendedPadding { get; set; } = 12;

        [Category("Layout")]
        [Description("Recommended spacing")]
        public int RecommendedSpacing { get; set; } = 8;

        [Category("Layout")]
        [Description("Recommended offset from target")]
        public int RecommendedOffset { get; set; } = 8;

        [Category("Visual")]
        [Description("Recommended border radius")]
        public int RecommendedBorderRadius { get; set; } = 8;

        [Category("Visual")]
        [Description("Recommended arrow size")]
        public int RecommendedArrowSize { get; set; } = 8;

        [Category("Visual")]
        [Description("Recommended shadow offset")]
        public int RecommendedShadowOffset { get; set; } = 4;

        [Category("Visual")]
        [Description("Recommended minimum width")]
        public int RecommendedMinWidth { get; set; } = 150;

        [Category("Visual")]
        [Description("Recommended maximum width")]
        public int RecommendedMaxWidth { get; set; } = 400;

        [Category("Typography")]
        [Description("Recommended font size")]
        public float RecommendedFontSize { get; set; } = 9.5f;

        [Category("Typography")]
        [Description("Recommended title font size")]
        public float RecommendedTitleFontSize { get; set; } = 10.5f;

        public override string ToString() => $"Style: {ControlStyle}, Padding: {RecommendedPadding}, FontSize: {RecommendedFontSize}";
    }
}
