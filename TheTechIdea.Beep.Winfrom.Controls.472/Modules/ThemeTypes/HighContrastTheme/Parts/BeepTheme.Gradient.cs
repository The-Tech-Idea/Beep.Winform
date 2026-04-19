using System.Drawing.Drawing2D;
using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class HighContrastTheme
    {
        // Gradient Properties
        public Color GradientStartColor { get; set; } = Color.Black;
        public Color GradientEndColor { get; set; } = Color.White;
        public LinearGradientMode GradientDirection { get; set; } = LinearGradientMode.Vertical;
    }
}
