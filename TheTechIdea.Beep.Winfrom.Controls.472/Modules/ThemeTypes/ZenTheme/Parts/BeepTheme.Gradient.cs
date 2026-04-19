using System.Drawing.Drawing2D;
using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class ZenTheme
    {
        // Gradient Properties
        public Color GradientStartColor { get; set; } = Color.FromArgb(34, 34, 34);
        public Color GradientEndColor { get; set; } = Color.FromArgb(64, 64, 64);
        public LinearGradientMode GradientDirection { get; set; } = LinearGradientMode.Vertical;
    }
}