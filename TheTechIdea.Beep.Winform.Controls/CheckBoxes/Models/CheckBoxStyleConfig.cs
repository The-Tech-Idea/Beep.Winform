using System;
using System.ComponentModel;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.CheckBoxes.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.CheckBoxes.Models
{
    /// <summary>
    /// Configuration model for checkbox style properties
    /// Defines visual properties for each checkbox style
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class CheckBoxStyleConfig
    {
        [Category("Style")]
        [Description("Checkbox style")]
        public CheckBoxStyle CheckBoxStyle { get; set; } = CheckBoxStyle.Material3;

        [Category("Style")]
        [Description("Associated control style")]
        public BeepControlStyle ControlStyle { get; set; } = BeepControlStyle.Material3;

        [Category("Layout")]
        [Description("Recommended checkbox size")]
        public int RecommendedCheckBoxSize { get; set; } = 20;

        [Category("Layout")]
        [Description("Recommended spacing between checkbox and text")]
        public int RecommendedSpacing { get; set; } = 8;

        [Category("Layout")]
        [Description("Recommended padding")]
        public int RecommendedPadding { get; set; } = 4;

        [Category("Visual")]
        [Description("Recommended border radius")]
        public int RecommendedBorderRadius { get; set; } = 4;

        [Category("Visual")]
        [Description("Recommended border width")]
        public int RecommendedBorderWidth { get; set; } = 2;

        [Category("Visual")]
        [Description("Recommended check mark thickness")]
        public int RecommendedCheckMarkThickness { get; set; } = 2;

        public override string ToString() => $"Style: {CheckBoxStyle}, Size: {RecommendedCheckBoxSize}, Spacing: {RecommendedSpacing}";
    }
}
