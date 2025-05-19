using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class MonochromeTheme
    {
        // Color Palette
        public Color PrimaryColor { get; set; } = Color.Black;
        public Color SecondaryColor { get; set; } = Color.DarkGray;
        public Color AccentColor { get; set; } = Color.LightGray;
        public Color BackgroundColor { get; set; } = Color.White;
        public Color SurfaceColor { get; set; } = Color.Gainsboro;
        public Color ErrorColor { get; set; } = Color.DarkRed;
        public Color WarningColor { get; set; } = Color.DarkOrange;
        public Color SuccessColor { get; set; } = Color.DarkGreen;
        public Color OnPrimaryColor { get; set; } = Color.White;
        public Color OnBackgroundColor { get; set; } = Color.Black;
    }
}
