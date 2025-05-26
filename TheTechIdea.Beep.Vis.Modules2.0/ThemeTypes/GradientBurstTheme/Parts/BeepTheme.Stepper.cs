using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class GradientBurstTheme
    {
        // Stepper Fonts & Colors
//<<<<<<< HEAD
        public TypographyStyle  StepperTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 16, FontStyle.Bold);
        public TypographyStyle  StepperSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 14, FontStyle.Bold);
        public TypographyStyle  StepperUnSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 14, FontStyle.Regular);

        public Color StepperBackColor { get; set; } = Color.FromArgb(250, 250, 255);
        public Color StepperForeColor { get; set; } = Color.FromArgb(33, 33, 33);
        public Color StepperBorderColor { get; set; } = Color.FromArgb(204, 204, 204);

        public Color StepperItemForeColor { get; set; } = Color.FromArgb(45, 45, 45);
        public TypographyStyle  StepperItemFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12, FontStyle.Regular);
        public TypographyStyle  StepperSubTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 11, FontStyle.Italic);

        public Color StepperItemHoverForeColor { get; set; } = Color.FromArgb(0, 120, 215);
        public Color StepperItemHoverBackColor { get; set; } = Color.FromArgb(230, 244, 255);

        public Color StepperItemSelectedForeColor { get; set; } = Color.White;
        public Color StepperItemSelectedBackColor { get; set; } = Color.FromArgb(0, 120, 215);
        public Color StepperItemSelectedBorderColor { get; set; } = Color.FromArgb(0, 90, 180);

        public Color StepperItemBorderColor { get; set; } = Color.FromArgb(200, 200, 200);
        public Color StepperItemHoverBorderColor { get; set; } = Color.FromArgb(0, 120, 215);

        public Color StepperItemCheckedBoxForeColor { get; set; } = Color.White;
        public Color StepperItemCheckedBoxBackColor { get; set; } = Color.FromArgb(0, 120, 215);
        public Color StepperItemCheckedBoxBorderColor { get; set; } = Color.FromArgb(0, 90, 180);
    }
}
