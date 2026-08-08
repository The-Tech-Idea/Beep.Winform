using System.Drawing.Drawing2D;
using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class RusticTheme
    {
        // Gradient Properties
        public Color GradientStartColor { get; set; } = Color.FromArgb(245, 245, 220);
        public Color GradientEndColor { get; set; } = Color.FromArgb(210, 180, 140);
        public LinearGradientMode GradientDirection { get; set; }
    }
}
