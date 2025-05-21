using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class FlatDesignTheme
    {
        // Stepper Fonts & Colors
        public TypographyStyle StepperTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 14, FontStyle.Bold);
        public TypographyStyle StepperSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12, FontStyle.Bold);
        public TypographyStyle StepperUnSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12, FontStyle.Regular);
        public Color StepperBackColor { get; set; } = Color.White;
        public Color StepperForeColor { get; set; } = Color.Black;
        public Color StepperBorderColor { get; set; } = Color.LightGray;
        public Color StepperItemForeColor { get; set; } = Color.DarkSlateGray;
        public TypographyStyle StepperItemFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 11, FontStyle.Regular);
        public TypographyStyle StepperSubTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10, FontStyle.Italic);
        public Color StepperItemHoverForeColor { get; set; } = Color.Black;
        public Color StepperItemHoverBackColor { get; set; } = Color.FromArgb(220, 220, 220);
        public Color StepperItemSelectedForeColor { get; set; } = Color.White;
        public Color StepperItemSelectedBackColor { get; set; } = Color.FromArgb(0, 120, 215); // Blue highlight
        public Color StepperItemSelectedBorderColor { get; set; } = Color.FromArgb(0, 84, 153);
        public Color StepperItemBorderColor { get; set; } = Color.LightGray;
        public Color StepperItemHoverBorderColor { get; set; } = Color.Gray;
        public Color StepperItemCheckedBoxForeColor { get; set; } = Color.White;
        public Color StepperItemCheckedBoxBackColor { get; set; } = Color.FromArgb(0, 120, 215);
        public Color StepperItemCheckedBoxBorderColor { get; set; } = Color.FromArgb(0, 84, 153);
    }
}
