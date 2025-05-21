using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    /// <summary>
    /// A theme inspired by cosmic and galaxy hues: deep blues, purples, and bright accents.
    /// </summary>
    public partial class GalaxyTheme
    {
        // Color Palette with sensible galaxy-inspired defaults
        public Color PrimaryColor { get; set; } = Color.FromArgb(0x1A, 0x1A, 0x2E); // #1A1A2E
        public Color SecondaryColor { get; set; } = Color.FromArgb(0x16, 0x21, 0x3E); // #16213E
        public Color AccentColor { get; set; } = Color.FromArgb(0x0F, 0x34, 0x60); // #0F3460
        public Color BackgroundColor { get; set; } = Color.FromArgb(0x05, 0x05, 0x14); // #050514
        public Color SurfaceColor { get; set; } = Color.FromArgb(0x1F, 0x19, 0x39); // #1F1939
        public Color ErrorColor { get; set; } = Color.FromArgb(0xFF, 0x45, 0x60); // #FF4560
        public Color WarningColor { get; set; } = Color.FromArgb(0xF2, 0xA6, 0x00); // #F2A600
        public Color SuccessColor { get; set; } = Color.FromArgb(0x23, 0xB9, 0x5C); // #23B95C
        public Color OnPrimaryColor { get; set; } = Color.White;
        public Color OnBackgroundColor { get; set; } = Color.FromArgb(0xC8, 0xC8, 0xC8); // light gray for text/icons on dark background
    }
}