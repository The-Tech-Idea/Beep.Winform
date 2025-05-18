using System.Drawing.Drawing2D;
using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class FlatDesignTheme
    {
        // Gradient Properties
        public Color GradientStartColor { get; set; } = Color.FromArgb(255, 255, 255); // white
        public Color GradientEndColor { get; set; } = Color.FromArgb(240, 240, 240); // light gray
        public LinearGradientMode GradientDirection { get; set; } = LinearGradientMode.Vertical;
    }
}
