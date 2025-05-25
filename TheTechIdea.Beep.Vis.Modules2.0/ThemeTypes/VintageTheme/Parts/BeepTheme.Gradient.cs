using System.Drawing.Drawing2D;
using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class VintageTheme
    {
        // Gradient Properties
        public Color GradientStartColor { get; set; } = Color.FromArgb(188, 143, 143);
        public Color GradientEndColor { get; set; } = Color.FromArgb(245, 245, 220);
        public LinearGradientMode GradientDirection { get; set; } = LinearGradientMode.Vertical;
    }
}