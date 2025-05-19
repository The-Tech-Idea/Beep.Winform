using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class DefaultTheme
    {
        // Stepper Fonts & Colors
        public TypographyStyle StepperTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 14f, FontStyle.Bold);
        public TypographyStyle StepperSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12f, FontStyle.Bold);
        public TypographyStyle StepperUnSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12f, FontStyle.Regular);
        public Color StepperBackColor { get; set; } = Color.WhiteSmoke;
        public Color StepperForeColor { get; set; } = Color.Black;
        public Color StepperBorderColor { get; set; } = Color.Gray;
        public Color StepperItemForeColor { get; set; } = Color.DimGray;
        public TypographyStyle StepperItemFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 11f, FontStyle.Regular);
        public TypographyStyle StepperSubTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10f, FontStyle.Italic);
        public Color StepperItemHoverForeColor { get; set; } = Color.Black;
        public Color StepperItemHoverBackColor { get; set; } = Color.LightGray;
        public Color StepperItemSelectedForeColor { get; set; } = Color.White;
        public Color StepperItemSelectedBackColor { get; set; } = Color.DodgerBlue;
        public Color StepperItemSelectedBorderColor { get; set; } = Color.RoyalBlue;
        public Color StepperItemBorderColor { get; set; } = Color.Silver;
        public Color StepperItemHoverBorderColor { get; set; } = Color.Gray;
        public Color StepperItemCheckedBoxForeColor { get; set; } = Color.DodgerBlue;
        public Color StepperItemCheckedBoxBackColor { get; set; } = Color.White;
        public Color StepperItemCheckedBoxBorderColor { get; set; } = Color.DodgerBlue;
    }
}
