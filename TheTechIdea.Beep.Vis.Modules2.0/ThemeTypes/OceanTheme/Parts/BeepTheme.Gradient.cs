using System.Drawing.Drawing2D;
using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class OceanTheme
    {
        // Gradient Properties
        public Color GradientStartColor { get; set; } = Color.FromArgb(0, 80, 120);
        public Color GradientEndColor { get; set; } = Color.FromArgb(0, 130, 180);
        public LinearGradientMode GradientDirection { get; set; } = LinearGradientMode.Vertical;
    }
}