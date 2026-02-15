using System;
using System.ComponentModel;
using System.Drawing;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Helpers;

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

        /// <summary>
        /// Returns a new StepperStyleConfig with pixel values scaled for DPI.
        /// Defaults: CircleSize=36x36, LineThickness=2, Spacing=20, IconSize=18, NumberFontSize=12, Padding=8
        /// </summary>
        public StepperStyleConfig ScaleForDpi(float dpiScale)
        {
            if (dpiScale <= 0 || Math.Abs(dpiScale - 1f) < 0.001f)
                return this;

            return new StepperStyleConfig
            {
                ControlStyle = ControlStyle,
                RecommendedButtonSize = new Size(
                    DpiScalingHelper.ScaleValue(RecommendedButtonSize.Width, dpiScale),
                    DpiScalingHelper.ScaleValue(RecommendedButtonSize.Height, dpiScale)),
                RecommendedStepSpacing = DpiScalingHelper.ScaleValue(RecommendedStepSpacing, dpiScale),
                RecommendedLabelSpacing = DpiScalingHelper.ScaleValue(RecommendedLabelSpacing, dpiScale),
                RecommendedPadding = DpiScalingHelper.ScaleValue(RecommendedPadding, dpiScale),
                RecommendedConnectorLineWidth = Math.Max(1, DpiScalingHelper.ScaleValue(RecommendedConnectorLineWidth, dpiScale)),
                RecommendedBorderWidth = Math.Max(1, DpiScalingHelper.ScaleValue(RecommendedBorderWidth, dpiScale)),
                RecommendedBorderRadius = DpiScalingHelper.ScaleValue(RecommendedBorderRadius, dpiScale)
            };
        }

        public override string ToString() => $"Style: {ControlStyle}, Button Size: {RecommendedButtonSize}, Spacing: {RecommendedStepSpacing}";
    }
}
