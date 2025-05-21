using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class PastelTheme
    {
        // Color Palette
        public Color PrimaryColor { get; set; } = Color.FromArgb(242, 201, 215);
        public Color SecondaryColor { get; set; } = Color.FromArgb(245, 183, 203);
        public Color AccentColor { get; set; } = Color.FromArgb(255, 204, 221);
        public Color BackgroundColor { get; set; } = Color.FromArgb(255, 245, 247);
        public Color SurfaceColor { get; set; } = Color.FromArgb(255, 255, 255);
        public Color ErrorColor { get; set; } = Color.FromArgb(255, 182, 182);
        public Color WarningColor { get; set; } = Color.FromArgb(255, 230, 180);
        public Color SuccessColor { get; set; } = Color.FromArgb(180, 255, 180);
        public Color OnPrimaryColor { get; set; } = Color.FromArgb(80, 80, 80);
        public Color OnBackgroundColor { get; set; } = Color.FromArgb(80, 80, 80);
    }
}