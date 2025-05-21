using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class RetroTheme
    {
        // Color Palette
        public Color PrimaryColor { get; set; } = Color.FromArgb(64, 64, 64);
        public Color SecondaryColor { get; set; } = Color.FromArgb(96, 96, 96);
        public Color AccentColor { get; set; } = Color.FromArgb(255, 165, 0);
        public Color BackgroundColor { get; set; } = Color.FromArgb(32, 32, 32);
        public Color SurfaceColor { get; set; } = Color.FromArgb(48, 48, 48);
        public Color ErrorColor { get; set; } = Color.FromArgb(255, 64, 64);
        public Color WarningColor { get; set; } = Color.FromArgb(255, 192, 0);
        public Color SuccessColor { get; set; } = Color.FromArgb(64, 255, 64);
        public Color OnPrimaryColor { get; set; } = Color.White;
        public Color OnBackgroundColor { get; set; } = Color.FromArgb(192, 192, 192);
    }
}