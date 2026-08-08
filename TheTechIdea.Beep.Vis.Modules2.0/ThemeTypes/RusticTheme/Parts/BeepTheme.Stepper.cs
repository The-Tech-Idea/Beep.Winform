using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class RusticTheme
    {
        // Stepper Fonts & Colors
        public TypographyStyle StepperTitleFont { get; set; }
        public TypographyStyle StepperSelectedFont { get; set; }
        public TypographyStyle StepperUnSelectedFont { get; set; }
        public Color StepperBackColor { get; set; } = Color.FromArgb(245, 245, 220);
        public Color StepperForeColor { get; set; } = Color.FromArgb(51, 51, 51);
        public Color StepperBorderColor { get; set; } = Color.FromArgb(205, 133, 63);
        public Color StepperItemForeColor { get; set; } = Color.FromArgb(51, 51, 51);
        public TypographyStyle StepperItemFont { get; set; }
        public TypographyStyle StepperSubTitleFont { get; set; }
        public Color StepperItemHoverForeColor { get; set; } = Color.FromArgb(62, 39, 35);
        public Color StepperItemHoverBackColor { get; set; } = Color.FromArgb(222, 184, 135);
        public Color StepperItemSelectedForeColor { get; set; } = Color.White;
        public Color StepperItemSelectedBackColor { get; set; } = Color.FromArgb(160, 82, 45);
        public Color StepperItemSelectedBorderColor { get; set; } = Color.FromArgb(160, 82, 45);
        public Color StepperItemBorderColor { get; set; } = Color.FromArgb(205, 133, 63);
        public Color StepperItemHoverBorderColor { get; set; } = Color.FromArgb(160, 82, 45);
        public Color StepperItemCheckedBoxForeColor { get; set; } = Color.White;
        public Color StepperItemCheckedBoxBackColor { get; set; } = Color.FromArgb(160, 82, 45);
        public Color StepperItemCheckedBoxBorderColor { get; set; } = Color.FromArgb(160, 82, 45);
    }
}
