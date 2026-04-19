using System.Drawing.Drawing2D;
using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class CandyTheme
    {
        // Gradient Properties

        public Color GradientStartColor { get; set; } = Color.FromArgb(255, 224, 235); // Pastel Pink (top/left)
        public Color GradientEndColor { get; set; } = Color.FromArgb(204, 255, 240);   // Pastel Mint (bottom/right)
        public LinearGradientMode GradientDirection { get; set; } = LinearGradientMode.Horizontal;
    }
}
