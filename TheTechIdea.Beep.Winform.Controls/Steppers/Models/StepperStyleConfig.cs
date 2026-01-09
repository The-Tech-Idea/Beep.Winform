using System;
using System.ComponentModel;
using System.Drawing;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;

namespace TheTechIdea.Beep.Winform.Controls.Steppers.Models
{
    /// <summary>
    /// Configuration model for stepper style properties
    /// Defines visual properties for each stepper style
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class StepperStyleConfig
    {
        [Category("Style")]
        [Description("Associated control style")]
        public BeepControlStyle ControlStyle { get; set; } = BeepControlStyle.Material3;

        [Category("Layout")]
        [Description("Recommended button size")]
        public Size RecommendedButtonSize { get; set; } = new Size(36, 36);

        [Category("Layout")]
        [Description("Recommended spacing between steps")]
        public int RecommendedStepSpacing { get; set; } = 20;

        [Category("Layout")]
        [Description("Recommended label spacing")]
        public int RecommendedLabelSpacing { get; set; } = 8;

        [Category("Layout")]
        [Description("Recommended padding")]
        public int RecommendedPadding { get; set; } = 12;

        [Category("Visual")]
        [Description("Recommended connector line width")]
        public int RecommendedConnectorLineWidth { get; set; } = 2;

        [Category("Visual")]
        [Description("Recommended border width")]
        public int RecommendedBorderWidth { get; set; } = 2;

        [Category("Visual")]
        [Description("Recommended border radius")]
        public int RecommendedBorderRadius { get; set; } = 18;

        public override string ToString() => $"Style: {ControlStyle}, Button Size: {RecommendedButtonSize}, Spacing: {RecommendedStepSpacing}";
    }
}
