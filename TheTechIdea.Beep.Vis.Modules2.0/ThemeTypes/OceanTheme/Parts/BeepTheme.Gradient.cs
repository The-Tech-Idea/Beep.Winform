using System.Drawing.Drawing2D;
using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class OceanTheme
    {
        // Gradient Properties
        public Color GradientStartColor { get; set; } = Color.FromArgb(10, 25, 47); // Deep navy blue for gradient start
        public Color GradientEndColor { get; set; } = Color.FromArgb(30, 60, 90); // Muted blue for gradient end
        public LinearGradientMode GradientDirection { get; set; } = LinearGradientMode.Vertical; // Vertical for ocean depth effect
    }
}