using System.Drawing.Drawing2D;
using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class MidnightTheme
    {
        // Gradient Properties
        public Color GradientStartColor { get; set; } = Color.FromArgb(30, 30, 30);
        public Color GradientEndColor { get; set; } = Color.FromArgb(10, 10, 10);
        public LinearGradientMode GradientDirection { get; set; } = LinearGradientMode.Vertical;
    }
}
