using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class RusticTheme
    {
        // Color Palette
        public Color PrimaryColor { get; set; } = Color.FromArgb(160, 82, 45); // Sienna
        public Color SecondaryColor { get; set; } = Color.FromArgb(205, 133, 63); // Peru
        public Color AccentColor { get; set; } = Color.FromArgb(184, 134, 11); // DarkGoldenrod
        public Color BackgroundColor { get; set; } = Color.FromArgb(245, 245, 220); // Beige
        public Color SurfaceColor { get; set; } = Color.FromArgb(210, 180, 140); // Tan
        public Color ErrorColor { get; set; } = Color.FromArgb(178, 34, 34); // Firebrick
        public Color WarningColor { get; set; } = Color.FromArgb(255, 165, 0); // Orange
        public Color SuccessColor { get; set; } = Color.FromArgb(107, 142, 35); // OliveDrab
        public Color OnPrimaryColor { get; set; } = Color.White;
        public Color OnBackgroundColor { get; set; } = Color.FromArgb(51, 51, 51); // Dark Gray
    }
}