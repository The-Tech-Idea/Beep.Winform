using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class PastelTheme
    {
        // Status Bar Colors
        public Color StatusBarBackColor { get; set; } = Color.FromArgb(235, 203, 217); // Soft pastel pink for status bar background
        public Color StatusBarForeColor { get; set; } = Color.FromArgb(60, 60, 60); // Dark gray for status bar text
        public Color StatusBarBorderColor { get; set; } = Color.FromArgb(180, 200, 220); // Pastel lavender for status bar border
        public Color StatusBarHoverBackColor { get; set; } = Color.FromArgb(200, 220, 240); // Light pastel blue for hover background
        public Color StatusBarHoverForeColor { get; set; } = Color.FromArgb(120, 160, 190); // Pastel blue for hover text
        public Color StatusBarHoverBorderColor { get; set; } = Color.FromArgb(120, 160, 190); // Pastel blue for hover border
    }
}