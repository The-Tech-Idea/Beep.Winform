using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class OceanTheme
    {
        // Color Palette
        public Color PrimaryColor { get; set; } = Color.FromArgb(0, 105, 148);
        public Color SecondaryColor { get; set; } = Color.FromArgb(0, 150, 200);
        public Color AccentColor { get; set; } = Color.FromArgb(0, 180, 230);
        public Color BackgroundColor { get; set; } = Color.FromArgb(240, 245, 250);
        public Color SurfaceColor { get; set; } = Color.FromArgb(255, 255, 255);
        public Color ErrorColor { get; set; } = Color.FromArgb(255, 100, 100);
        public Color WarningColor { get; set; } = Color.FromArgb(255, 200, 0);
        public Color SuccessColor { get; set; } = Color.FromArgb(0, 200, 100);
        public Color OnPrimaryColor { get; set; } = Color.White;
        public Color OnBackgroundColor { get; set; } = Color.FromArgb(0, 80, 120);
    }
}