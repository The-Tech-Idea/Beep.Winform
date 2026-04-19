using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class MonochromeTheme
    {
        // Stepper Fonts & Colors
        public TypographyStyle StepperTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 14f, FontStyle.Bold);
        public TypographyStyle StepperSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Bold);
        public TypographyStyle StepperUnSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Regular);
        public Color StepperBackColor { get; set; } = Color.Black;
        public Color StepperForeColor { get; set; } = Color.WhiteSmoke;
        public Color StepperBorderColor { get; set; } = Color.Gray;
        public Color StepperItemForeColor { get; set; } = Color.White;
        public TypographyStyle StepperItemFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Regular);
        public TypographyStyle StepperSubTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12f, FontStyle.Italic);
        public Color StepperItemHoverForeColor { get; set; } = Color.WhiteSmoke;
        public Color StepperItemHoverBackColor { get; set; } = Color.DimGray;
        public Color StepperItemSelectedForeColor { get; set; } = Color.Black;
        public Color StepperItemSelectedBackColor { get; set; } = Color.WhiteSmoke;
        public Color StepperItemSelectedBorderColor { get; set; } = Color.Silver;
        public Color StepperItemBorderColor { get; set; } = Color.Gray;
        public Color StepperItemHoverBorderColor { get; set; } = Color.LightGray;
        public Color StepperItemCheckedBoxForeColor { get; set; } = Color.White;
        public Color StepperItemCheckedBoxBackColor { get; set; } = Color.Black;
        public Color StepperItemCheckedBoxBorderColor { get; set; } = Color.Gray;
    }
}
