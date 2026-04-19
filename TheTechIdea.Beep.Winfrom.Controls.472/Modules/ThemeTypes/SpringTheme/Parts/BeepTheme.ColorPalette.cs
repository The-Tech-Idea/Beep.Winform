using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class SpringTheme
    {
        // Color Palette
        public Color PrimaryColor { get; set; } = Color.FromArgb(60, 179, 113);
        public Color SecondaryColor { get; set; } = Color.FromArgb(135, 206, 250);
        public Color AccentColor { get; set; } = Color.FromArgb(255, 215, 0);
        public Color BackgroundColor { get; set; } = Color.FromArgb(240, 248, 255);
        public Color SurfaceColor { get; set; } = Color.FromArgb(245, 245, 245);
        public Color ErrorColor { get; set; } = Color.FromArgb(255, 99, 71);
        public Color WarningColor { get; set; } = Color.FromArgb(255, 165, 0);
        public Color SuccessColor { get; set; } = Color.FromArgb(50, 205, 50);
        public Color OnPrimaryColor { get; set; } = Color.White;
        public Color OnBackgroundColor { get; set; } = Color.FromArgb(50, 50, 50);
    }
}