using System.Drawing.Drawing2D;
using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class RoyalTheme
    {
        // Gradient Properties
        public Color GradientStartColor { get; set; } = Color.FromArgb(33, 37, 41);
        public Color GradientEndColor { get; set; } = Color.FromArgb(52, 58, 64);
        public LinearGradientMode GradientDirection { get; set; } = LinearGradientMode.Vertical;
    }
}