using System.Drawing.Drawing2D;
using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class GlassmorphismTheme
    {
        // Gradient Properties
        public Color GradientStartColor { get; set; } = Color.FromArgb(230, 240, 250);
        public Color GradientEndColor { get; set; } = Color.FromArgb(200, 220, 240);
        public LinearGradientMode GradientDirection { get; set; } = LinearGradientMode.Vertical;
    }
}
