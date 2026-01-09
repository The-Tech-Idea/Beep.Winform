using System;
using System.ComponentModel;
using System.Drawing;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;

namespace TheTechIdea.Beep.Winform.Controls.RadioGroup.Models
{
    /// <summary>
    /// Configuration model for radio group style properties
    /// Defines visual properties for each render style
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class RadioGroupStyleConfig
    {
        [Category("Style")]
        [Description("Border radius for radio items")]
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

        [Category("Layout")]
        [Description("Recommended item height")]
        public int RecommendedItemHeight { get; set; } = 40;

        [Category("Layout")]
        [Description("Recommended item spacing")]
        public int RecommendedItemSpacing { get; set; } = 8;

        [Category("Layout")]
        [Description("Recommended padding")]
        public Padding RecommendedPadding { get; set; } = new Padding(8);

        public override string ToString() => $"Radius: {BorderRadius}, Shadow: {ShowShadow}";
    }
}
