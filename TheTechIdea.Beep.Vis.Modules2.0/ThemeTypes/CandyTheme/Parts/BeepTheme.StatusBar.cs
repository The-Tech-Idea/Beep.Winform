using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class CandyTheme
    {
        // Status Bar Colors

        // Default: pastel pink with navy text and mint border
        public Color StatusBarBackColor { get; set; } = Color.FromArgb(255, 224, 235);      // Pastel Pink
        public Color StatusBarForeColor { get; set; } = Color.FromArgb(44, 62, 80);         // Navy
        public Color StatusBarBorderColor { get; set; } = Color.FromArgb(127, 255, 212);    // Mint

        // Hover: baby blue background, candy pink text, lemon border
        public Color StatusBarHoverBackColor { get; set; } = Color.FromArgb(210, 235, 255); // Baby Blue
        public Color StatusBarHoverForeColor { get; set; } = Color.FromArgb(240, 100, 180); // Candy Pink
        public Color StatusBarHoverBorderColor { get; set; } = Color.FromArgb(255, 223, 93); // Lemon Yellow
    }
}
