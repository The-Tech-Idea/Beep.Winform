using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class DesertTheme
    {
        // RadioButton properties
        public Color RadioButtonBackColor { get; set; } = Color.FromArgb(250, 240, 230); // Soft beige background
        public Color RadioButtonForeColor { get; set; } = Color.FromArgb(139, 69, 19);  // SaddleBrown text
        public Color RadioButtonBorderColor { get; set; } = Color.FromArgb(210, 180, 140); // Tan border

        public Color RadioButtonCheckedBackColor { get; set; } = Color.FromArgb(244, 164, 96); // SandyBrown fill for checked
        public Color RadioButtonCheckedForeColor { get; set; } = Color.White; // White text for checked
        public Color RadioButtonCheckedBorderColor { get; set; } = Color.FromArgb(160, 82, 45); // Sienna border checked

        public Color RadioButtonHoverBackColor { get; set; } = Color.FromArgb(255, 228, 196); // Bisque hover background
        public Color RadioButtonHoverForeColor { get; set; } = Color.FromArgb(139, 69, 19); // SaddleBrown hover text
        public Color RadioButtonHoverBorderColor { get; set; } = Color.FromArgb(210, 180, 140); // Tan hover border

        public TypographyStyle RadioButtonFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10, FontStyle.Regular);
        public TypographyStyle RadioButtonCheckedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10, FontStyle.Bold);

        public Color RadioButtonSelectedForeColor { get; set; } = Color.White;
        public Color RadioButtonSelectedBackColor { get; set; } = Color.FromArgb(210, 105, 30); // Chocolate selected background
    }
}
