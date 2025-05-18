using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class DefaultTheme
    {
        // Gradient Properties
        public Color GradientStartColor { get; set; } = Color.FromArgb(255, 255, 255); // White
        public Color GradientEndColor { get; set; } = Color.FromArgb(230, 230, 230);   // Light Gray
        public LinearGradientMode GradientDirection { get; set; } = LinearGradientMode.Vertical;
    }
}
