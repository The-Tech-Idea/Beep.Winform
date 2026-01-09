using System;
using System.ComponentModel;
using System.Drawing;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;

namespace TheTechIdea.Beep.Winform.Controls.Numerics.Models
{
    /// <summary>
    /// Configuration model for numeric control style properties
    /// Defines visual properties for each control style
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class NumericStyleConfig
    {
        [Category("Style")]
        [Description("Button width in pixels")]
        public int ButtonWidth { get; set; } = 24;

        [Category("Style")]
        [Description("Button height in pixels")]
        public int ButtonHeight { get; set; } = 20;

        [Category("Style")]
        [Description("Show shadow effect")]
        public bool ShowShadow { get; set; } = true;

        [Category("Style")]
        [Description("Shadow color")]
        public Color ShadowColor { get; set; } = Color.FromArgb(20, Color.Black);

        [Category("Style")]
        [Description("Shadow offset/elevation")]
        public Point ShadowOffset { get; set; } = new Point(0, 1);

        [Category("Style")]
        [Description("Associated control style")]
        public BeepControlStyle ControlStyle { get; set; } = BeepControlStyle.Material3;

        [Category("Layout")]
        [Description("Recommended minimum height")]
        public int RecommendedMinimumHeight { get; set; } = 36;

        [Category("Layout")]
        [Description("Recommended minimum width")]
        public int RecommendedMinimumWidth { get; set; } = 100;

        [Category("Layout")]
        [Description("Recommended padding")]
        public Padding RecommendedPadding { get; set; } = new Padding(8, 6, 8, 6);

        [Category("Icons")]
        [Description("Icon size ratio (as percentage of button size)")]
        public float IconSizeRatio { get; set; } = 0.5f;

        public override string ToString() => $"Buttons: {ButtonWidth}x{ButtonHeight}, Shadow: {ShowShadow}";
    }
}
