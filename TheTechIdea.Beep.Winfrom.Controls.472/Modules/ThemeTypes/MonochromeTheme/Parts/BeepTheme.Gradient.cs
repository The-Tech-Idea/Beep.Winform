using System.Drawing.Drawing2D;
using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class MonochromeTheme
    {
        // Gradient Properties
        public Color GradientStartColor { get; set; } = Color.WhiteSmoke;
        public Color GradientEndColor { get; set; } = Color.Gray;
        public LinearGradientMode GradientDirection { get; set; } = LinearGradientMode.Vertical;
    }
}
