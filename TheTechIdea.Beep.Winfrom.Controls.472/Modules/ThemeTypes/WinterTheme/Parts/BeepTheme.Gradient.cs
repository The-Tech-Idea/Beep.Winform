using System.Drawing.Drawing2D;
using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class WinterTheme
    {
        // Gradient Properties
        public Color GradientStartColor { get; set; } = Color.FromArgb(27, 62, 92);
        public Color GradientEndColor { get; set; } = Color.FromArgb(45, 85, 120);
        public LinearGradientMode GradientDirection { get; set; } = LinearGradientMode.Vertical;
    }
}