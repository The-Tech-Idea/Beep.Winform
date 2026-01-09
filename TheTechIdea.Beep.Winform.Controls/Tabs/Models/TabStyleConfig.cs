using System;
using System.ComponentModel;
using System.Drawing;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;

namespace TheTechIdea.Beep.Winform.Controls.Tabs.Models
{
    /// <summary>
    /// Configuration model for tab style properties
    /// Defines visual properties for each tab style
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class TabStyleConfig
    {
        [Category("Style")]
        [Description("Border radius for tabs")]
        public int BorderRadius { get; set; } = 4;

        [Category("Style")]
        [Description("Show shadow effect")]
        public bool ShowShadow { get; set; } = false;

        [Category("Style")]
        [Description("Shadow color")]
        public Color ShadowColor { get; set; } = Color.FromArgb(20, Color.Black);

        [Category("Style")]
        [Description("Shadow offset/elevation")]
        public Point ShadowOffset { get; set; } = new Point(0, 2);

        [Category("Style")]
        [Description("Associated control style")]
        public BeepControlStyle ControlStyle { get; set; } = BeepControlStyle.Material3;

        [Category("Style")]
        [Description("Border width")]
        public int BorderWidth { get; set; } = 1;

        [Category("Style")]
        [Description("Indicator/underline thickness")]
        public int IndicatorThickness { get; set; } = 2;

        [Category("Layout")]
        [Description("Recommended header height")]
        public int RecommendedHeaderHeight { get; set; } = 30;

        [Category("Layout")]
        [Description("Recommended tab padding")]
        public Padding RecommendedTabPadding { get; set; } = new Padding(12, 8, 12, 8);

        public override string ToString() => $"Radius: {BorderRadius}, Indicator: {IndicatorThickness}";
    }
}
