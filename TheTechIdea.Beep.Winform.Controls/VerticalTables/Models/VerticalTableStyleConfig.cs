using System;
using System.ComponentModel;
using System.Drawing;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;

namespace TheTechIdea.Beep.Winform.Controls.VerticalTables.Models
{
    /// <summary>
    /// Configuration model for vertical table style properties
    /// Defines visual properties for each table style
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class VerticalTableStyleConfig
    {
        [Category("Style")]
        [Description("Border radius for table cards")]
        public int BorderRadius { get; set; } = 12;

        [Category("Style")]
        [Description("Show shadow effect")]
        public bool ShowShadow { get; set; } = true;

        [Category("Style")]
        [Description("Shadow color")]
        public Color ShadowColor { get; set; } = Color.FromArgb(40, Color.Black);

        [Category("Style")]
        [Description("Shadow offset/elevation")]
        public Point ShadowOffset { get; set; } = new Point(0, 4);

        [Category("Style")]
        [Description("Associated control style")]
        public BeepControlStyle ControlStyle { get; set; } = BeepControlStyle.Material3;

        [Category("Style")]
        [Description("Border width")]
        public int BorderWidth { get; set; } = 1;

        [Category("Layout")]
        [Description("Recommended header height")]
        public int RecommendedHeaderHeight { get; set; } = 80;

        [Category("Layout")]
        [Description("Recommended row height")]
        public int RecommendedRowHeight { get; set; } = 40;

        [Category("Layout")]
        [Description("Recommended column width")]
        public int RecommendedColumnWidth { get; set; } = 150;

        public override string ToString() => $"Radius: {BorderRadius}, Shadow: {ShowShadow}";
    }
}
