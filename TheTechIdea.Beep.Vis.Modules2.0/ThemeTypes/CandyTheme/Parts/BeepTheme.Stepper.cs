using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class CandyTheme
    {
        // Stepper Fonts & Colors

        public TypographyStyle StepperTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Comic Sans MS", 12f, FontStyle.Bold);
        public TypographyStyle StepperSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Comic Sans MS", 11f, FontStyle.Bold);
        public TypographyStyle StepperUnSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10.5f, FontStyle.Regular);

        public Color StepperBackColor { get; set; } = Color.FromArgb(255, 224, 235);        // Pastel Pink
        public Color StepperForeColor { get; set; } = Color.FromArgb(44, 62, 80);           // Navy
        public Color StepperBorderColor { get; set; } = Color.FromArgb(127, 255, 212);      // Mint

        public Color StepperItemForeColor { get; set; } = Color.FromArgb(44, 62, 80);       // Navy
        public TypographyStyle StepperItemFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10.5f, FontStyle.Regular);
        public TypographyStyle StepperSubTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10f, FontStyle.Italic);

        // Hover: baby blue background, candy pink text, lemon border
        public Color StepperItemHoverForeColor { get; set; } = Color.FromArgb(240, 100, 180); // Candy Pink
        public Color StepperItemHoverBackColor { get; set; } = Color.FromArgb(210, 235, 255); // Baby Blue

        // Selected: lemon yellow on mint, candy pink border
        public Color StepperItemSelectedForeColor { get; set; } = Color.FromArgb(255, 223, 93); // Lemon Yellow
        public Color StepperItemSelectedBackColor { get; set; } = Color.FromArgb(204, 255, 240); // Mint
        public Color StepperItemSelectedBorderColor { get; set; } = Color.FromArgb(240, 100, 180); // Candy Pink

        // Item borders
        public Color StepperItemBorderColor { get; set; } = Color.FromArgb(206, 183, 255);      // Pastel Lavender
        public Color StepperItemHoverBorderColor { get; set; } = Color.FromArgb(255, 223, 93);  // Lemon

        // Checkbox (for step complete/check)
        public Color StepperItemCheckedBoxForeColor { get; set; } = Color.FromArgb(127, 255, 212); // Mint
        public Color StepperItemCheckedBoxBackColor { get; set; } = Color.FromArgb(255, 253, 194); // Lemon Yellow
        public Color StepperItemCheckedBoxBorderColor { get; set; } = Color.FromArgb(240, 100, 180); // Candy Pink
    }
}
