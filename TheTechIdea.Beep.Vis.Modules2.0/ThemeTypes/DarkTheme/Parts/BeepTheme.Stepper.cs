using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class DarkTheme
    {
        // Stepper Fonts & Colors
        public TypographyStyle StepperTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 14, FontStyle.Bold);
        public TypographyStyle StepperSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12, FontStyle.Bold);
        public TypographyStyle StepperUnSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12, FontStyle.Regular);
        public Color StepperBackColor { get; set; } = Color.FromArgb(30, 30, 30);
        public Color StepperForeColor { get; set; } = Color.WhiteSmoke;
        public Color StepperBorderColor { get; set; } = Color.DimGray;
        public Color StepperItemForeColor { get; set; } = Color.LightGray;
        public TypographyStyle StepperItemFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 11, FontStyle.Regular);
        public TypographyStyle StepperSubTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 9, FontStyle.Italic);
        public Color StepperItemHoverForeColor { get; set; } = Color.White;
        public Color StepperItemHoverBackColor { get; set; } = Color.FromArgb(60, 60, 60);
        public Color StepperItemSelectedForeColor { get; set; } = Color.White;
        public Color StepperItemSelectedBackColor { get; set; } = Color.FromArgb(0, 122, 204); // Blue accent
        public Color StepperItemSelectedBorderColor { get; set; } = Color.DeepSkyBlue;
        public Color StepperItemBorderColor { get; set; } = Color.Gray;
        public Color StepperItemHoverBorderColor { get; set; } = Color.LightBlue;
        public Color StepperItemCheckedBoxForeColor { get; set; } = Color.White;
        public Color StepperItemCheckedBoxBackColor { get; set; } = Color.FromArgb(0, 122, 204);
        public Color StepperItemCheckedBoxBorderColor { get; set; } = Color.DeepSkyBlue;
    }
}
