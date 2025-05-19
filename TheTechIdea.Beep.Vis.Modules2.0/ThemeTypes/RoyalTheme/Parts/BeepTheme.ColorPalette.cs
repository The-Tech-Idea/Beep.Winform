using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class RoyalTheme
    {
        // Color Palette
        public Color PrimaryColor { get; set; } = Color.FromArgb(33, 37, 41);
        public Color SecondaryColor { get; set; } = Color.FromArgb(52, 58, 64);
        public Color AccentColor { get; set; } = Color.FromArgb(255, 193, 7);
        public Color BackgroundColor { get; set; } = Color.FromArgb(22, 26, 30);
        public Color SurfaceColor { get; set; } = Color.FromArgb(44, 48, 52);
        public Color ErrorColor { get; set; } = Color.FromArgb(255, 77, 77);
        public Color WarningColor { get; set; } = Color.FromArgb(255, 159, 64);
        public Color SuccessColor { get; set; } = Color.FromArgb(75, 192, 192);
        public Color OnPrimaryColor { get; set; } = Color.White;
        public Color OnBackgroundColor { get; set; } = Color.FromArgb(200, 200, 200);
    }
}