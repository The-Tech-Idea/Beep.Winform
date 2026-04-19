using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class ForestTheme
    {
        // Stepper Fonts & Colors
        public TypographyStyle StepperTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 14f, FontStyle.Bold);
        public TypographyStyle StepperSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Bold);
        public TypographyStyle StepperUnSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Regular);
        public Color StepperBackColor { get; set; } = Color.FromArgb(34, 139, 34); // ForestGreen
        public Color StepperForeColor { get; set; } = Color.White;
        public Color StepperBorderColor { get; set; } = Color.DarkGreen;
        public Color StepperItemForeColor { get; set; } = Color.LightGreen;
        public TypographyStyle StepperItemFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Regular);
        public TypographyStyle StepperSubTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12f, FontStyle.Italic);
        public Color StepperItemHoverForeColor { get; set; } = Color.WhiteSmoke;
        public Color StepperItemHoverBackColor { get; set; } = Color.FromArgb(46, 139, 87); // MediumSeaGreen
        public Color StepperItemSelectedForeColor { get; set; } = Color.White;
        public Color StepperItemSelectedBackColor { get; set; } = Color.SeaGreen;
        public Color StepperItemSelectedBorderColor { get; set; } = Color.LimeGreen;
        public Color StepperItemBorderColor { get; set; } = Color.DarkOliveGreen;
        public Color StepperItemHoverBorderColor { get; set; } = Color.LightGreen;
        public Color StepperItemCheckedBoxForeColor { get; set; } = Color.ForestGreen;
        public Color StepperItemCheckedBoxBackColor { get; set; } = Color.Honeydew;
        public Color StepperItemCheckedBoxBorderColor { get; set; } = Color.DarkGreen;
    }
}
