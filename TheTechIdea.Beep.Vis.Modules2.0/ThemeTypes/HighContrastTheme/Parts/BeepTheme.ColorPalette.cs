using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class HighContrastTheme
    {
        // Color Palette
        public Color PrimaryColor { get; set; } = Color.Black;
        public Color SecondaryColor { get; set; } = Color.White;
        public Color AccentColor { get; set; } = Color.Yellow;
        public Color BackgroundColor { get; set; } = Color.Black;
        public Color SurfaceColor { get; set; } = Color.DarkGray;
        public Color ErrorColor { get; set; } = Color.Red;
        public Color WarningColor { get; set; } = Color.Orange;
        public Color SuccessColor { get; set; } = Color.Lime;
        public Color OnPrimaryColor { get; set; } = Color.White;
        public Color OnBackgroundColor { get; set; } = Color.White;
    }
}
