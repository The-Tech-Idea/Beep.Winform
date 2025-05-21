using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class GlassmorphismTheme
    {
        // Color Palette
        public Color PrimaryColor { get; set; } = Color.FromArgb(180, 200, 230); // Soft blue
        public Color SecondaryColor { get; set; } = Color.FromArgb(200, 210, 230); // Light bluish
        public Color AccentColor { get; set; } = Color.FromArgb(100, 180, 255); // Sky blue
        public Color BackgroundColor { get; set; } = Color.FromArgb(245, 250, 255); // Very light background
        public Color SurfaceColor { get; set; } = Color.FromArgb(230, 240, 250); // Card surface

        public Color ErrorColor { get; set; } = Color.FromArgb(255, 80, 80);   // Bright red
        public Color WarningColor { get; set; } = Color.FromArgb(255, 193, 7);   // Amber
        public Color SuccessColor { get; set; } = Color.FromArgb(76, 175, 80);   // Green

        public Color OnPrimaryColor { get; set; } = Color.Black;
        public Color OnBackgroundColor { get; set; } = Color.Black;
    }
}
