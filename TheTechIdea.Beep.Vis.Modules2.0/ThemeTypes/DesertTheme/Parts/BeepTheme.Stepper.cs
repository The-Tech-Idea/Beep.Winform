using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class DesertTheme
    {
        // Stepper Fonts & Colors
        public TypographyStyle StepperTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 16, FontStyle.Bold);
        public TypographyStyle StepperSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 14, FontStyle.Bold);
        public TypographyStyle StepperUnSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 14, FontStyle.Regular);
        public Color StepperBackColor { get; set; } = Color.FromArgb(245, 222, 179); // Wheat
        public Color StepperForeColor { get; set; } = Color.FromArgb(101, 67, 33); // Dark Brown
        public Color StepperBorderColor { get; set; } = Color.FromArgb(160, 82, 45); // Sienna
        public Color StepperItemForeColor { get; set; } = Color.FromArgb(139, 69, 19); // Saddle Brown
        public TypographyStyle StepperItemFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12, FontStyle.Regular);
        public TypographyStyle StepperSubTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10, FontStyle.Italic);
        public Color StepperItemHoverForeColor { get; set; } = Color.FromArgb(222, 184, 135); // BurlyWood
        public Color StepperItemHoverBackColor { get; set; } = Color.FromArgb(255, 228, 181); // Moccasin
        public Color StepperItemSelectedForeColor { get; set; } = Color.FromArgb(255, 140, 0); // DarkOrange
        public Color StepperItemSelectedBackColor { get; set; } = Color.FromArgb(255, 218, 185); // PeachPuff
        public Color StepperItemSelectedBorderColor { get; set; } = Color.FromArgb(205, 133, 63); // Peru
        public Color StepperItemBorderColor { get; set; } = Color.FromArgb(210, 180, 140); // Tan
        public Color StepperItemHoverBorderColor { get; set; } = Color.FromArgb(244, 164, 96); // SandyBrown
        public Color StepperItemCheckedBoxForeColor { get; set; } = Color.FromArgb(139, 69, 19); // Saddle Brown
        public Color StepperItemCheckedBoxBackColor { get; set; } = Color.FromArgb(255, 228, 181); // Moccasin
        public Color StepperItemCheckedBoxBorderColor { get; set; } = Color.FromArgb(160, 82, 45); // Sienna
    }
}
