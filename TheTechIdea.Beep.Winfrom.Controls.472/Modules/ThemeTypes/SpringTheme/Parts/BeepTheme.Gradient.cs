using System.Drawing.Drawing2D;
using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class SpringTheme
    {
        // Gradient Properties
        public Color GradientStartColor { get; set; } = Color.FromArgb(173, 216, 230);
        public Color GradientEndColor { get; set; } = Color.FromArgb(144, 238, 144);
        public LinearGradientMode GradientDirection { get; set; } = LinearGradientMode.Vertical;
    }
}