using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class ZenTheme
    {
        // Color Palette
        public Color PrimaryColor { get; set; } = Color.FromArgb(34, 34, 34);
        public Color SecondaryColor { get; set; } = Color.FromArgb(64, 64, 64);
        public Color AccentColor { get; set; } = Color.FromArgb(76, 175, 80);
        public Color BackgroundColor { get; set; } = Color.FromArgb(245, 245, 245);
        public Color SurfaceColor { get; set; } = Color.FromArgb(255, 255, 255);
        public Color ErrorColor { get; set; } = Color.FromArgb(244, 67, 54);
        public Color WarningColor { get; set; } = Color.FromArgb(255, 193, 7);
        public Color SuccessColor { get; set; } = Color.FromArgb(76, 175, 80);
        public Color OnPrimaryColor { get; set; } = Color.White;
        public Color OnBackgroundColor { get; set; } = Color.FromArgb(34, 34, 34);
    }
}