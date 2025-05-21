using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class DesertTheme
    {
        // Textbox colors and Fonts
        public Color TextBoxBackColor { get; set; } = Color.FromArgb(255, 245, 230);  // Soft sand
        public Color TextBoxForeColor { get; set; } = Color.FromArgb(102, 51, 0);     // Dark brown
        public Color TextBoxBorderColor { get; set; } = Color.FromArgb(210, 180, 140); // Tan border
        public Color TextBoxHoverBorderColor { get; set; } = Color.FromArgb(244, 164, 96); // Sandy brown
        public Color TextBoxHoverBackColor { get; set; } = Color.FromArgb(255, 250, 240); // Lighter sand
        public Color TextBoxHoverForeColor { get; set; } = Color.FromArgb(102, 51, 0); // Dark brown
        public Color TextBoxSelectedBorderColor { get; set; } = Color.FromArgb(205, 133, 63); // Peru
        public Color TextBoxSelectedBackColor { get; set; } = Color.FromArgb(255, 240, 220); // Warm light sand
        public Color TextBoxSelectedForeColor { get; set; } = Color.FromArgb(102, 51, 0); // Dark brown
        public Color TextBoxPlaceholderColor { get; set; } = Color.FromArgb(160, 82, 45); // Sienna

        public Color TextBoxErrorBorderColor { get; set; } = Color.FromArgb(178, 34, 34); // Firebrick red
        public Color TextBoxErrorBackColor { get; set; } = Color.FromArgb(255, 228, 225); // Misty rose
        public Color TextBoxErrorForeColor { get; set; } = Color.FromArgb(178, 34, 34);   // Firebrick red
        public Color TextBoxErrorTextColor { get; set; } = Color.FromArgb(178, 34, 34);   // Firebrick red
        public Color TextBoxErrorPlaceholderColor { get; set; } = Color.FromArgb(178, 34, 34); // Firebrick red
        public Color TextBoxErrorTextBoxColor { get; set; } = Color.FromArgb(255, 240, 240); // Light pink
        public Color TextBoxErrorTextBoxBorderColor { get; set; } = Color.FromArgb(178, 34, 34); // Firebrick red
        public Color TextBoxErrorTextBoxHoverColor { get; set; } = Color.FromArgb(255, 220, 220); // Light salmon

        public TypographyStyle TextBoxFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12, FontStyle.Regular);
        public TypographyStyle TextBoxHoverFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12, FontStyle.Regular);
        public TypographyStyle TextBoxSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12, FontStyle.Bold);
    }
}
