using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class VintageTheme
    {
        // Color Palette
        public Color PrimaryColor { get; set; } = Color.FromArgb(160, 82, 45);
        public Color SecondaryColor { get; set; } = Color.FromArgb(188, 143, 143);
        public Color AccentColor { get; set; } = Color.FromArgb(205, 133, 63);
        public Color BackgroundColor { get; set; } = Color.FromArgb(245, 245, 220);
        public Color SurfaceColor { get; set; } = Color.FromArgb(240, 235, 215);
        public Color ErrorColor { get; set; } = Color.FromArgb(178, 34, 34);
        public Color WarningColor { get; set; } = Color.FromArgb(204, 85, 0);
        public Color SuccessColor { get; set; } = Color.FromArgb(107, 142, 35);
        public Color OnPrimaryColor { get; set; } = Color.FromArgb(255, 245, 238);
        public Color OnBackgroundColor { get; set; } = Color.FromArgb(51, 25, 0);
    }
}