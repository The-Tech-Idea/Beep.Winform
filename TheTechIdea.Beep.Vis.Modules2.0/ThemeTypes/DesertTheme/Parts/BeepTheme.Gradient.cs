using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class DesertTheme
    {
        // Gradient Properties
        public Color GradientStartColor { get; set; } = Color.FromArgb(250, 214, 165); // Warm sand
        public Color GradientEndColor { get; set; } = Color.FromArgb(198, 135, 62); // Desert orange
        public LinearGradientMode GradientDirection { get; set; } = LinearGradientMode.Vertical;
    }
}
