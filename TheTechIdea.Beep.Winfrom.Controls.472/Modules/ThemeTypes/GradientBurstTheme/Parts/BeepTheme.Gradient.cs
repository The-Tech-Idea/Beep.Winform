using System.Drawing.Drawing2D;
using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class GradientBurstTheme
    {
        // Gradient Properties
        public Color GradientStartColor { get; set; } = Color.FromArgb(255, 255, 140, 0);  // Vibrant Orange
        public Color GradientEndColor { get; set; } = Color.FromArgb(255, 255, 69, 0);     // Deep Orange/Red
        public LinearGradientMode GradientDirection { get; set; } = LinearGradientMode.ForwardDiagonal;
    }
}
