using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class DarkTheme
    {
        // Status Bar Colors
        public Color StatusBarBackColor { get; set; } = Color.FromArgb(45, 45, 48); // Dark gray background
        public Color StatusBarForeColor { get; set; } = Color.WhiteSmoke; // Light foreground text
        public Color StatusBarBorderColor { get; set; } = Color.DimGray; // Subtle border color
        public Color StatusBarHoverBackColor { get; set; } = Color.FromArgb(63, 63, 70); // Slightly lighter on hover
        public Color StatusBarHoverForeColor { get; set; } = Color.White; // Bright on hover
        public Color StatusBarHoverBorderColor { get; set; } = Color.Gray; // Hover border highlight
    }
}
