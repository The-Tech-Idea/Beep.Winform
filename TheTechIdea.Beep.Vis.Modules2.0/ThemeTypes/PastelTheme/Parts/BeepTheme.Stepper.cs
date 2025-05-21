using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class PastelTheme
    {
        // Stepper Fonts & Colors
        public TypographyStyle StepperTitleFont { get; set; } = new TypographyStyle() { FontSize = 14, FontWeight = FontWeight.Bold, TextColor = Color.FromArgb(80, 80, 80) };
        public TypographyStyle StepperSelectedFont { get; set; } = new TypographyStyle() { FontSize = 12, FontWeight = FontWeight.Medium, TextColor = Color.White };
        public TypographyStyle StepperUnSelectedFont { get; set; } = new TypographyStyle() { FontSize = 12, FontWeight = FontWeight.Regular, TextColor = Color.FromArgb(120, 120, 120) };
        public Color StepperBackColor { get; set; } = Color.FromArgb(255, 245, 247);
        public Color StepperForeColor { get; set; } = Color.FromArgb(120, 120, 120);
        public Color StepperBorderColor { get; set; } = Color.FromArgb(242, 201, 215);
        public Color StepperItemForeColor { get; set; } = Color.FromArgb(120, 120, 120);
        public TypographyStyle StepperItemFont { get; set; } = new TypographyStyle() { FontSize = 12, FontWeight = FontWeight.Regular, TextColor = Color.FromArgb(120, 120, 120) };
        public TypographyStyle StepperSubTitleFont { get; set; } = new TypographyStyle() { FontSize = 12, FontWeight = FontWeight.Medium, TextColor = Color.FromArgb(120, 120, 120) };
        public Color StepperItemHoverForeColor { get; set; } = Color.FromArgb(80, 80, 80);
        public Color StepperItemHoverBackColor { get; set; } = Color.FromArgb(255, 224, 239);
        public Color StepperItemSelectedForeColor { get; set; } = Color.White;
        public Color StepperItemSelectedBackColor { get; set; } = Color.FromArgb(245, 183, 203);
        public Color StepperItemSelectedBorderColor { get; set; } = Color.FromArgb(230, 170, 190);
        public Color StepperItemBorderColor { get; set; } = Color.FromArgb(242, 201, 215);
        public Color StepperItemHoverBorderColor { get; set; } = Color.FromArgb(237, 181, 201);
        public Color StepperItemCheckedBoxForeColor { get; set; } = Color.White;
        public Color StepperItemCheckedBoxBackColor { get; set; } = Color.FromArgb(245, 183, 203);
        public Color StepperItemCheckedBoxBorderColor { get; set; } = Color.FromArgb(230, 170, 190);
    }
}