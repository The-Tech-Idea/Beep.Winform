using System;
using System.ComponentModel;
using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.Steppers.Models
{
    /// <summary>
    /// Color configuration model for stepper control
    /// Stores all color properties for theme and customization
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class StepperColorConfig
    {
        [Category("Step Colors")]
        [Description("Completed step color")]
        public Color CompletedStepColor { get; set; } = Color.FromArgb(34, 197, 94);

        [Category("Step Colors")]
        [Description("Active step color")]
        public Color ActiveStepColor { get; set; } = Color.FromArgb(59, 130, 246);

        [Category("Step Colors")]
        [Description("Pending step color")]
        public Color PendingStepColor { get; set; } = Color.FromArgb(156, 163, 175);

        [Category("Step Colors")]
        [Description("Error step color")]
        public Color ErrorStepColor { get; set; } = Color.FromArgb(239, 68, 68);

        [Category("Step Colors")]
        [Description("Warning step color")]
        public Color WarningStepColor { get; set; } = Color.FromArgb(245, 158, 11);

        [Category("Text Colors")]
        [Description("Step text color")]
        public Color StepTextColor { get; set; } = Color.White;

        [Category("Text Colors")]
        [Description("Step label color")]
        public Color StepLabelColor { get; set; } = Color.Black;

        [Category("Connector Colors")]
        [Description("Completed connector line color")]
        public Color ConnectorCompletedColor { get; set; } = Color.FromArgb(34, 197, 94);

        [Category("Connector Colors")]
        [Description("Pending connector line color")]
        public Color ConnectorPendingColor { get; set; } = Color.FromArgb(156, 163, 175);

        [Category("Background")]
        [Description("Stepper background color")]
        public Color BackgroundColor { get; set; } = Color.Transparent;

        [Category("Border")]
        [Description("Step border color")]
        public Color BorderColor { get; set; } = Color.White;

        public override string ToString() => $"Active: {ActiveStepColor}, Completed: {CompletedStepColor}, Pending: {PendingStepColor}";
    }
}
