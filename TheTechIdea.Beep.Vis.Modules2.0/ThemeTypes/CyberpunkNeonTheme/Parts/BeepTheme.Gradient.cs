using System.Drawing.Drawing2D;
using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class CyberpunkNeonTheme
    {
        // Gradient Properties

        public Color GradientStartColor { get; set; } = Color.FromArgb(255, 0, 255);   // Neon Magenta
        public Color GradientEndColor { get; set; } = Color.FromArgb(0, 255, 255);     // Neon Cyan
        public LinearGradientMode GradientDirection { get; set; } = LinearGradientMode.Horizontal;
    }
}
