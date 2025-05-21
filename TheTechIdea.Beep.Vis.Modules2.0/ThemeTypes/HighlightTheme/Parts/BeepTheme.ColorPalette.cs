using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class HighlightTheme
    {
        // Color Palette
        public Color PrimaryColor { get; set; } = Color.DodgerBlue;
        public Color SecondaryColor { get; set; } = Color.LightSkyBlue;
        public Color AccentColor { get; set; } = Color.Orange;
        public Color BackgroundColor { get; set; } = Color.White;
        public Color SurfaceColor { get; set; } = Color.WhiteSmoke;
        public Color ErrorColor { get; set; } = Color.Red;
        public Color WarningColor { get; set; } = Color.Goldenrod;
        public Color SuccessColor { get; set; } = Color.Green;
        public Color OnPrimaryColor { get; set; } = Color.White;
        public Color OnBackgroundColor { get; set; } = Color.Black;
    }
}
