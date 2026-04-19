using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class HighlightTheme
    {
        // Stepper Fonts & Colors
        public TypographyStyle  StepperTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 14f, FontStyle.Bold);
        public TypographyStyle  StepperSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Bold);
        public TypographyStyle  StepperUnSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Regular);
        public Color StepperBackColor { get; set; } = Color.LightSteelBlue;
        public Color StepperForeColor { get; set; } = Color.Black;
        public Color StepperBorderColor { get; set; } = Color.SteelBlue;
        public Color StepperItemForeColor { get; set; } = Color.Black;
        public TypographyStyle  StepperItemFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Regular);
        public TypographyStyle  StepperSubTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12f, FontStyle.Italic);
        public Color StepperItemHoverForeColor { get; set; } = Color.White;
        public Color StepperItemHoverBackColor { get; set; } = Color.RoyalBlue;
        public Color StepperItemSelectedForeColor { get; set; } = Color.White;
        public Color StepperItemSelectedBackColor { get; set; } = Color.DodgerBlue;
        public Color StepperItemSelectedBorderColor { get; set; } = Color.MediumBlue;
        public Color StepperItemBorderColor { get; set; } = Color.LightSteelBlue;
        public Color StepperItemHoverBorderColor { get; set; } = Color.RoyalBlue;
        public Color StepperItemCheckedBoxForeColor { get; set; } = Color.White;
        public Color StepperItemCheckedBoxBackColor { get; set; } = Color.DodgerBlue;
        public Color StepperItemCheckedBoxBorderColor { get; set; } = Color.MediumBlue;
    }
}
