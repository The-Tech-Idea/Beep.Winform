using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class WinterTheme
    {
        // Color Palette
        public Color PrimaryColor { get; set; } = Color.FromArgb(27, 62, 92);
        public Color SecondaryColor { get; set; } = Color.FromArgb(45, 85, 120);
        public Color AccentColor { get; set; } = Color.FromArgb(100, 149, 237);
        public Color BackgroundColor { get; set; } = Color.FromArgb(230, 240, 250);
        public Color SurfaceColor { get; set; } = Color.FromArgb(245, 245, 245);
        public Color ErrorColor { get; set; } = Color.FromArgb(255, 99, 99);
        public Color WarningColor { get; set; } = Color.FromArgb(255, 193, 7);
        public Color SuccessColor { get; set; } = Color.FromArgb(77, 182, 172);
        public Color OnPrimaryColor { get; set; } = Color.White;
        public Color OnBackgroundColor { get; set; } = Color.FromArgb(27, 62, 92);
    }
}