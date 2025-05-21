using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class OceanTheme
    {
        // Stepper Fonts & Colors
        public TypographyStyle StepperTitleFont { get; set; } = new TypographyStyle() { FontSize = 14, FontWeight = FontWeight.Bold, TextColor = Color.White };
        public TypographyStyle StepperSelectedFont { get; set; } = new TypographyStyle() { FontSize = 12, FontWeight = FontWeight.Medium, TextColor = Color.White };
        public TypographyStyle StepperUnSelectedFont { get; set; } = new TypographyStyle() { FontSize = 12, FontWeight = FontWeight.Regular, TextColor = Color.FromArgb(200, 255, 255) };
        public Color StepperBackColor { get; set; } = Color.FromArgb(0, 105, 148);
        public Color StepperForeColor { get; set; } = Color.FromArgb(200, 255, 255);
        public Color StepperBorderColor { get; set; } = Color.FromArgb(0, 120, 170);
        public Color StepperItemForeColor { get; set; } = Color.FromArgb(200, 255, 255);
        public TypographyStyle StepperItemFont { get; set; } = new TypographyStyle() { FontSize = 12, FontWeight = FontWeight.Regular, TextColor = Color.FromArgb(200, 255, 255) };
        public TypographyStyle StepperSubTitleFont { get; set; } = new TypographyStyle() { FontSize = 12, FontWeight = FontWeight.Medium, TextColor = Color.FromArgb(200, 255, 255) };
        public Color StepperItemHoverForeColor { get; set; } = Color.White;
        public Color StepperItemHoverBackColor { get; set; } = Color.FromArgb(0, 160, 210);
        public Color StepperItemSelectedForeColor { get; set; } = Color.White;
        public Color StepperItemSelectedBackColor { get; set; } = Color.FromArgb(0, 180, 230);
        public Color StepperItemSelectedBorderColor { get; set; } = Color.FromArgb(0, 150, 200);
        public Color StepperItemBorderColor { get; set; } = Color.FromArgb(0, 120, 170);
        public Color StepperItemHoverBorderColor { get; set; } = Color.FromArgb(0, 130, 180);
        public Color StepperItemCheckedBoxForeColor { get; set; } = Color.White;
        public Color StepperItemCheckedBoxBackColor { get; set; } = Color.FromArgb(0, 180, 230);
        public Color StepperItemCheckedBoxBorderColor { get; set; } = Color.FromArgb(0, 150, 200);
    }
}