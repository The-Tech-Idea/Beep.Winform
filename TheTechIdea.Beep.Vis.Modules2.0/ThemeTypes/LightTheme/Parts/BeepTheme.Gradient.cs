using System.Drawing.Drawing2D;
using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class LightTheme
    {
        // Gradient Properties
        public Color GradientStartColor { get; set; } = Color.White;
        public Color GradientEndColor { get; set; } = Color.LightGray;
        public LinearGradientMode GradientDirection { get; set; } = LinearGradientMode.Vertical;
    }
}
