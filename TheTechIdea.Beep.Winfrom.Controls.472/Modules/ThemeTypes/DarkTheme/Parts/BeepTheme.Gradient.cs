using System.Drawing.Drawing2D;
using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class DarkTheme
    {
        // Gradient Properties
        public Color GradientStartColor { get; set; } = Color.FromArgb(40, 40, 40);
        public Color GradientEndColor { get; set; } = Color.FromArgb(20, 20, 20);
        public LinearGradientMode GradientDirection { get; set; } = LinearGradientMode.Vertical;
    }
}
